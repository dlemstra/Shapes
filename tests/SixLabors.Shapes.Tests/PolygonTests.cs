﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SixLabors.Shapes.Tests
{
    using System.Numerics;

    public class PolygonTests
    {
        public static TheoryData<TestPoint[], TestPoint, bool> PointInPolygonTheoryData =
            new TheoryData<TestPoint[], TestPoint, bool>
            {
                {
                    new TestPoint[] {new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10)},
                    // loc
                    new Vector2(10, 10), // test
                    true
                }, //corner is inside
                {
                    new TestPoint[] {new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10)},
                    // loc
                    new Vector2(10, 11), // test
                    true
                }, //on line
                {
                    new TestPoint[] {new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10)},
                    // loc
                    new Vector2(9, 9), // test
                    false
                }, //corner is inside
            };

        [Theory]
        [MemberData(nameof(PointInPolygonTheoryData))]
        public void PointInPolygon(TestPoint[] controlPoints, TestPoint point, bool isInside)
        {
            var shape = new Polygon(new LinearLineSegment(controlPoints.Select(x => (Vector2)x).ToArray()));
            Assert.Equal(isInside, shape.Contains(point));
        }

        public static TheoryData<TestPoint[], TestPoint, float> DistanceTheoryData =
           new TheoryData<TestPoint[], TestPoint, float>
           {
                {
                    new TestPoint[] {new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10)},
                    new Vector2(10, 10),
                    0

                },
               {
                   new TestPoint[] { new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10) },
                   new Vector2(10, 11), 0
               },
               {
                   new TestPoint[] { new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10) },
                   new Vector2(11, 11), 0
                },
               {
                   new TestPoint[] { new Vector2(10, 10), new Vector2(10, 100), new Vector2(100, 100), new Vector2(100, 10) },
                   new Vector2(9, 10), 1
                },
           };

        [Theory]
        [MemberData(nameof(DistanceTheoryData))]
        public void Distance(TestPoint[] controlPoints, TestPoint point, float expected)
        {
            var shape = new Polygon(new LinearLineSegment(controlPoints.Select(x => (Vector2)x).ToArray()));
            Assert.Equal(expected, shape.Distance(point));
        }

        public static TheoryData<TestPoint, float, float> PathDistanceTheoryData =
            new TheoryData<TestPoint, float, float>
                {
                    { new Vector2(0, 0), 0f, 0f },
                    { new Vector2(1,  0), 0f, 1f },
                    { new Vector2(9,  0), 0f, 9f },
                    { new Vector2(10,  0), 0f, 10f },
                    { new Vector2(10, 1), 0f, 11f },
                    { new Vector2(10,  9), 0f, 19f },
                    { new Vector2(10,  10), 0f, 20f },
                    { new Vector2(9,  10), 0f, 21f },
                    { new Vector2(1,  10), 0f, 29f },
                    { new Vector2(0,  10), 0f, 30f },
                    { new Vector2(0,  1), 0f, 39f },
                    { new Vector2(4,  3), 3f, 4f },
                    { new Vector2(3, 4), 3f, 36f },
                    { new Vector2(-1,  0), 1f, 0f },
                    { new Vector2(1,  -1), 1f, 1f },
                    { new Vector2(9,  -1), 1f, 9f },
                    { new Vector2(11,  0), 1f, 10f },
                    { new Vector2(11, 1), 1f, 11f },
                    { new Vector2(11,  9), 1f, 19f },
                    { new Vector2(11,  10), 1f, 20f },
                    { new Vector2(9,  11), 1f, 21f },
                    { new Vector2(1,  11), 1f, 29f }
                };

        [Theory]
        [MemberData(nameof(PathDistanceTheoryData))]
        public void DistanceFromPath_Path(TestPoint point, float expectedDistance, float alongPath)
        {
            IPath path = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(10, 0), new Vector2(10, 10), new Vector2(0, 10))).AsPath();
            var info = path.Distance(point);
            Assert.Equal(expectedDistance, info.DistanceFromPath);
            Assert.Equal(alongPath, info.DistanceAlongPath);
        }

        [Fact]
        public void AsSimpleLinearPath()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10), new Vector2(5, 5)));
            var paths = poly.Flatten();
            Assert.Equal(3, paths.Length);
            Assert.Equal(new Vector2(0, 0), paths[0]);
            Assert.Equal(new Vector2(0, 10), paths[1]);
            Assert.Equal(new Vector2(5, 5), paths[2]);
        }

        [Fact]
        public void FindIntersectionsBuffer()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10), new Vector2(10, 10), new Vector2(10, 0)));
            var buffer = new Vector2[2];

            var hits = poly.FindIntersections(new Vector2(5, -5), new Vector2(5, 15), buffer, 2, 0);
            Assert.Equal(2, hits);
            Assert.Contains(new Vector2(5, 10), buffer);
            Assert.Contains(new Vector2(5, 0), buffer);
        }

        [Fact]
        public void FindIntersectionsCollection()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10), new Vector2(10, 10), new Vector2(10, 0)));

            var buffer = poly.FindIntersections(new Vector2(5, -5), new Vector2(5, 15)).ToArray();
            Assert.Equal(2, buffer.Length);
            Assert.Contains(new Vector2(5, 10), buffer);
            Assert.Contains(new Vector2(5, 0), buffer);
        }

        [Fact]
        public void ReturnsWrapperOfSelfASOwnPath_SingleSegment()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10), new Vector2(5, 5)));
            var paths = poly.Paths;
            Assert.Equal(1, paths.Length);
            Assert.Equal(poly, paths[0].AsShape());
        }

        [Fact]
        public void ReturnsWrapperOfSelfASOwnPath_MultiSegment()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10)), new LinearLineSegment(new Vector2(2, 5), new Vector2(5, 5)));
            var paths = poly.Paths;
            Assert.Equal(1, paths.Length);
            Assert.Equal(poly, paths[0].AsShape());
        }

        [Fact]
        public void Bounds()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10), new Vector2(5, 5)));
            var bounds = poly.Bounds;
            Assert.Equal(0, bounds.Left);
            Assert.Equal(0, bounds.Top);
            Assert.Equal(5, bounds.Right);
            Assert.Equal(10, bounds.Bottom);
        }

        [Fact]
        public void Length()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10))).AsPath();
            Assert.Equal(20, poly.Length);
        }

        [Fact]
        public void MaxIntersections()
        {
            var poly = new Polygon(new LinearLineSegment(new Vector2(0, 0), new Vector2(0, 10)));

            // with linear polygons its the number of points the segments have
            Assert.Equal(2, poly.MaxIntersections);
        }

        [Fact]
        public void FindBothIntersections()
        {
            var poly = new Polygon(new LinearLineSegment(
                            new Vector2(10, 10),
                            new Vector2(200, 150),
                            new Vector2(50, 300)));
            var intersections = poly.FindIntersections(new Vector2(float.MinValue, 55), new Vector2(float.MaxValue, 55));
            Assert.Equal(2, intersections.Count());
        }


        [Fact]
        public void HandleClippingInnerCorner()
        {
            var simplePath = new Polygon(new LinearLineSegment(
                             new Vector2(10, 10),
                             new Vector2(200, 150),
                             new Vector2(50, 300)));

            var hole1 = new Polygon(new LinearLineSegment(
                            new Vector2(37, 85),
                            new Vector2(130, 40),
                            new Vector2(65, 137)));

            var poly = simplePath.Clip(hole1);

            var intersections = poly.FindIntersections(new Vector2(float.MinValue, 137), new Vector2(float.MaxValue, 137));

            // returns an even number of points
            Assert.Equal(2, intersections.Count());
        }


        [Fact]
        public void CrossingCorner()
        {
            var simplePath = new Polygon(new LinearLineSegment(
                             new Vector2(10, 10),
                             new Vector2(200, 150),
                             new Vector2(50, 300)));

            var intersections = simplePath.FindIntersections(new Vector2(float.MinValue, 150), new Vector2(float.MaxValue, 150));

            // returns an even number of points
            Assert.Equal(2, intersections.Count());
        }


        [Fact]
        public void ClippingEdgefromInside()
        {
            var simplePath = new Rectangle(10, 10, 100, 100).Clip(new Rectangle(20, 0, 20, 20));

            var intersections = simplePath.FindIntersections(new Vector2(float.MinValue, 20), new Vector2(float.MaxValue, 20));

            // returns an even number of points
            Assert.Equal(2, intersections.Count());
        }

        [Fact]
        public void ClippingEdgeFromOutside()
        {
            var simplePath = new Polygon(new LinearLineSegment(
                             new Vector2(10, 10),
                             new Vector2(100, 10),
                             new Vector2(50, 300)));

            var intersections = simplePath.FindIntersections(new Vector2(float.MinValue, 10), new Vector2(float.MaxValue, 10));

            // returns an even number of points
            Assert.Equal(2, intersections.Count());
        }

        [Fact]
        public void HandleClippingOutterCorner()
        {
            var simplePath = new Polygon(new LinearLineSegment(
                             new Vector2(10, 10),
                             new Vector2(200, 150),
                             new Vector2(50, 300)));

            var hole1 = new Polygon(new LinearLineSegment(
                            new Vector2(37, 85),
                            new Vector2(130, 40),
                            new Vector2(65, 137)));

            var poly = simplePath.Clip(hole1);

            var intersections = poly.FindIntersections(new Vector2(float.MinValue, 300), new Vector2(float.MaxValue, 300));

            // returns an even number of points
            Assert.Equal(0, intersections.Count());
        }

        [Fact]
        public void MissingIntersection()
        {
            var simplePath = new Polygon(new LinearLineSegment(
                             new Vector2(10, 10),
                             new Vector2(200, 150),
                             new Vector2(50, 300)));

            var hole1 = new Polygon(new LinearLineSegment(
                            new Vector2(37, 85),
                            new Vector2(130, 40),
                            new Vector2(65, 137)));

            var poly = simplePath.Clip(hole1);

            var intersections = poly.FindIntersections(new Vector2(float.MinValue, 85), new Vector2(float.MaxValue, 85));

            // returns an even number of points
            Assert.Equal(4, intersections.Count());
        }

        [Theory]
        [InlineData(243)]
        [InlineData(341)]
        [InlineData(199)]
        public void BezierPolygonReturning2Points(int y)
        {
            // missing bands in test from ImageSharp
            Vector2[] simplePath = new[] {
                        new Vector2(10, 400),
                        new Vector2(30, 10),
                        new Vector2(240, 30),
                        new Vector2(300, 400)
            };

            var poly = new Polygon(new BezierLineSegment(simplePath));

            var points = poly.FindIntersections(new Vector2(float.MinValue, y), new Vector2(float.MaxValue, y)).ToList();

            Assert.Equal(2, points.Count());
        }
    }
}
