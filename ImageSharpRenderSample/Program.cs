using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Drawing.Brushes;
using ImageSharp.Drawing;
using System.IO;

namespace ImageSharpRenderSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var img = new Image<Color>(100, 100);

            var brush = new SolidBrush(Color.HotPink);

            var rectangle = new Shaper2D.Rectangle(10, 10, 40, 40);
            img.BackgroundColor(Color.BlueViolet);

            img.Apply(new FillShapeProcessor<Color>(brush, rectangle, GraphicsOptions.Default));

            using (var fs = File.OpenWrite("test.bmp"))
            {
                img.SaveAsBmp(fs);
            }
        }
    }
}
