using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeArt.DotnetGD.Formatters;
using CodeArt.DotnetGD.Libgd;
using Xunit;
using Xunit.Sdk;

namespace CodeArt.DotnetGD.Tests
{
    /// <summary>
    /// Test class for classes implementing ImageFormatter
    /// </summary>
    public class FormatterTests
    {
        /// <summary>
        /// Attribute to get the formats data
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        private class ImageFormattersDataAttribute : DataAttribute
        {
            private static readonly Action<IImageFormatter>[] NullTestActions = {
                DecodeNullImage,
                EncodeNullImage,
                ReadNullFile,
                ReadNullStream,
                WriteNullImageToFile,
                WriteNullImageToStream,
                WriteImageToNullFile,
                WriteImageToNullStream
            };

            private static readonly Func<IImageFormatter, Task>[] NullTestFunctions = {
                ReadNullFileAsync,
                ReadNullStreamAsync,
                WriteNullImageToFileAsync,
                WriteNullImageToStreamAsync,
                WriteImageToNullFileAsync,
                WriteImageToNullStreamAsync
            };

            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                // Get all concrete formatter implementations that have a default constructor
                var types = typeof(IImageFormatter).GetTypeInfo().Assembly
                    .GetTypes()
                    .Where(t => typeof(IImageFormatter).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
                                && !t.GetTypeInfo().IsAbstract
                                && t.GetConstructor(Type.EmptyTypes) != null
                    );
                switch (testMethod.Name)
                {
                    case nameof(TestFormattererImage2Image):
                        foreach (var type in types)
                        {
                            yield return new object[] { type, PixelFormat.Format32BppArgb };
                            yield return new object[] { type, PixelFormat.Format8BppIndexed };
                        }
                        break;
                    case nameof(TestEmptyStream):
                        foreach (var type in types)
                        {
                            yield return new object[] { type };
                        }
                        break;
                    case nameof(TestNullParameters):
                        foreach (var type in types)
                        {
                            foreach (var nullTestAction in NullTestActions)
                            {
                                yield return new object[] { type, nullTestAction };
                            }
                        }
                        break;
                    case nameof(TestNullParametersAsync):
                        foreach (var type in types)
                        {
                            foreach (var nullTestAction in NullTestFunctions)
                            {
                                yield return new object[] { type, nullTestAction };
                            }
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unknown test: ${testMethod.Name}.", nameof(testMethod));
                }
            }
        }

        private static void DecodeNullImage(IImageFormatter formatter)
        {
            formatter.DecodeImage(null);
        }

        private static void EncodeNullImage(IImageFormatter formatter)
        {
            formatter.EncodeImage(null);
        }

        private static void ReadNullFile(IImageFormatter formatter)
        {
            formatter.ReadImageFromFile(null);
        }

        private static Task ReadNullFileAsync(IImageFormatter formatter)
        {
            return formatter.ReadImageFromFileAsync(null);
        }

        private static void ReadNullStream(IImageFormatter formatter)
        {
            formatter.ReadImageFromStream(null);
        }

        private static Task ReadNullStreamAsync(IImageFormatter formatter)
        {
            return formatter.ReadImageFromStreamAsync(null);
        }

        private static void WriteNullImageToFile(IImageFormatter formatter)
        {
            formatter.WriteImageToFile(null, Path.ChangeExtension("test", formatter.DefaultExtension));
        }

        private static Task WriteNullImageToFileAsync(IImageFormatter formatter)
        {
            return formatter.WriteImageToFileAsync(null, Path.ChangeExtension("test", formatter.DefaultExtension));
        }

        private static void WriteNullImageToStream(IImageFormatter formatter)
        {
            using (var ms = new MemoryStream())
            {
                formatter.WriteImageToStream(null, ms);
            }
        }

        private static async Task WriteNullImageToStreamAsync(IImageFormatter formatter)
        {
            using (var ms = new MemoryStream())
            {
                await formatter.WriteImageToStreamAsync(null, ms);
            }
        }

        private static void WriteImageToNullFile(IImageFormatter formatter)
        {
            using (var image = new Image(20, 40))
            {
                formatter.WriteImageToFile(image, null);
            }
        }

        private static async Task WriteImageToNullFileAsync(IImageFormatter formatter)
        {
            using (var image = new Image(20, 40))
            {
                await formatter.WriteImageToFileAsync(image, null);
            }
        }

        private static void WriteImageToNullStream(IImageFormatter formatter)
        {
            using (var image = new Image(20, 40))
            {
                formatter.WriteImageToStream(image, null);
            }
        }

        private static async Task WriteImageToNullStreamAsync(IImageFormatter formatter)
        {
            using (var image = new Image(20, 40))
            {
                await formatter.WriteImageToStreamAsync(image, null);
            }
        }

        public static bool AreImagesSimilar(Image im1, Image im2, int threshold)
        {
            var res = Image.CompareImages(im1, im2);
            if (res == ImageCompareResult.Similar)
                return true;
            return !res.HasFlag(ImageCompareResult.DifferentSize);
        }
        
        [Theory]
        [ImageFormattersData]
        public void TestFormattererImage2Image(Type formatterType, PixelFormat pixelFormat)
        {
            var formatter = (IImageFormatter)Activator.CreateInstance(formatterType);
            if (!formatter.CanEncode) return;
            using (var image = new Image(100, 100, pixelFormat))
            {
                var red = new Color(0xff, 0, 0);
                var green = new Color(0, 0xff, 0);
                var blue = new Color(0, 0, 0xff);

                image.DrawFilledRectangle(new Rectangle(0, 0, 99, 99), red);
                image.DrawRectangle(new Rectangle(20, 20, 59, 59), green);
                image.DrawEllipse(new Point(70, 25), new Size(30, 20), blue);

                var path = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString("n"), formatter.DefaultExtension));
                try
                {
                    formatter.WriteImageToFile(image, path);
                    var encoded = formatter.EncodeImage(image);

                    if (!formatter.CanDecode) return;

                    using (var decoded = formatter.DecodeImage(encoded))
                    {
                        var result = AreImagesSimilar(decoded, image, 128);
                        Assert.True(result);
                    }
                    using (var decoded = formatter.ReadImageFromFile(path))
                    {
                        var result = AreImagesSimilar(decoded, image, 128);
                        Assert.True(result);
                    }
                }
                finally
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
            }
        }

        /// <summary>
        /// Test passing null parameters
        /// </summary>
        /// <param name="formatterType"></param>
        /// <param name="action"></param>
        [Theory]
        [ImageFormattersData]
        public void TestNullParameters(Type formatterType, Action<IImageFormatter> action)
        {
            var formatter = (IImageFormatter)Activator.CreateInstance(formatterType);
            Assert.Throws<ArgumentNullException>(() =>
            {
                action(formatter);
            });
        }

        /// <summary>
        /// Test passing null paramters to async methods
        /// </summary>
        /// <param name="formatterType"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        [Theory]
        [ImageFormattersData]
        public async Task TestNullParametersAsync(Type formatterType, Func<IImageFormatter, Task> func)
        {
            var formatter = (IImageFormatter)Activator.CreateInstance(formatterType);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await func(formatter);
            });
        }

        [Theory]
        [ImageFormattersData]
        public void TestEmptyStream(Type formatterType)
        {
            var formatter = (IImageFormatter)Activator.CreateInstance(formatterType);

            Assert.Throws<LibgdException>(() =>
            {
                using (var ms = new MemoryStream())
                {
                    using (formatter.ReadImageFromStream(ms))
                    {

                    }
                }
            });
        }

        [Fact]
        public void TestAnimatedGif()
        {
            var gif = new GifImageFormatter();

            using (var image = new Image(100, 100, PixelFormat.Format8BppIndexed))
            {
                var red = new Color(0xff, 0, 0);
                var green = new Color(0, 0xff, 0);
                var blue = new Color(0, 0, 0xff);

                image.DrawFilledRectangle(new Rectangle(0, 0, 99, 99), red);
                image.DrawRectangle(new Rectangle(20, 20, 59, 59), green);
                image.DrawEllipse(new Point(70, 25), new Size(30, 20), blue);
                var path = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString("n"), gif.DefaultExtension));
                try
                {
                    using (var fs = File.Create(path))
                    {
                        using (var context = gif.BeginAnimation(image, fs, true, -1))
                        {

                            for (var i = 0; i < 10; i++)
                            {
                                using (var addedImage = new Image(image.Width, image.Height, PixelFormat.Format8BppIndexed))
                                {
                                    addedImage.DrawFilledRectangle(new Rectangle(0, 0, 99, 99), red);
                                    addedImage.DrawRectangle(new Rectangle(21 + i, 20 + i, 59, 59), green);
                                    addedImage.DrawEllipse(new Point(70 - i, 25 + i), new Size(30 + i, 20), blue);
                                    context.AddImage(addedImage, false, 0, 0, 20, 0);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                
                
            }
                
        }
    }
}
