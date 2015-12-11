using System;
using DotnetGD;
using DotnetGD.Formatters;
using Xunit;

namespace DotnetGDTests
{
    public class DrawTextTests
    {
        [Fact]
        public void DrawText()
        {
            using (var image = new Image(400, 400))
            {
                var red = new Color(0xff, 0, 0);
                

                image.DrawString("Hello world!", new Point(40, 40), "Arial", 12, 0, red);
                
                      
            }
        }

        [Fact]
        public void DrawArabicText()
        {
            using (var image = new Image(400, 400))
            {
                var red = new Color(0xff, 0, 0);


                image.DrawString("مرحبا بالعالم!"
                        +Environment.NewLine
                        +"مرحبا بالعالم!"
                    , new Point(40, 40), "Arial", 12, 0, red);

                var png = new PngImageFormatter();
                png.WriteImageToFile(image, "fi.png");
            }
        }
    }
}
