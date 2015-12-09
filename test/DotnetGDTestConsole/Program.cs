using System;
using System.IO;
using DotnetGD;
using DotnetGD.Formatters;

namespace DotnetGDTestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (var img = new Image(-20, 10))
                {
                    var png = new PngImageFormatter();
                    using (var fs = File.Open("test.png", FileMode.Create))
                    {
                        png.WriteImageToStream(img, fs);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
