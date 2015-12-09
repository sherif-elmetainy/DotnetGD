using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotnetGD;
using DotnetGD.Formatters;
using Xunit;
using Xunit.Sdk;

namespace DotnetGDTests
{
    /// <summary>
    /// Test class for classes implementing ImageFormatter
    /// </summary>
    public class FormatterTests
    {
        /// <summary>
        /// Attribute to get the formats data
        /// </summary>
        private class ImageFormmatersDataAttribute : DataAttribute
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
            formatter.WriteImageToFile(null, "test" + formatter.DefaultExtension);
        }

        private static Task WriteNullImageToFileAsync(IImageFormatter formatter)
        {
            return formatter.WriteImageToFileAsync(null, "test" + formatter.DefaultExtension);
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

        /// <summary>
        /// test reading and writing images
        /// </summary>
        /// <param name="formatterType"></param>
        [Theory]
        [ImageFormmatersData]
        public void TestFormattererImage2Image(Type formatterType)
        {
            var formatter = (IImageFormatter)Activator.CreateInstance(formatterType);
            if (!formatter.CanEncode) return;
            using (var image = new Image(100, 100))
            {
                var red = new Color(0xff, 0, 0);
                var green = new Color(0, 0xff, 0);
                var blue = new Color(0, 0, 0xff);

                image.DrawFilledRecangle(new Rectangle(0, 0, 99, 99), red);
                image.DrawRecangle(new Rectangle(20, 20, 59, 59), green);
                image.DrawEllipse(new Rectangle(70, 25, 30, 20), blue);

                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n") + "." + formatter.DefaultExtension);
                try
                {
                    formatter.WriteImageToFile(image, path);
                    var encoded = formatter.EncodeImage(image);

                    if (!formatter.CanDecode) return;

                    const ImageCompareResult expectedDifference = ImageCompareResult.Image | ImageCompareResult.Color
                                                                  | ImageCompareResult.TrueColor | ImageCompareResult.TransparentColor | ImageCompareResult.NumberOfColors;


                    using (var decoded = formatter.DecodeImage(encoded))
                    {
                        var result = image.CompareTo(decoded);
                        Assert.Equal(ImageCompareResult.Similar, result & (~expectedDifference));
                    }
                    using (var decoded = formatter.ReadImageFromFile(path))
                    {
                        var result = image.CompareTo(decoded);
                        Assert.Equal(ImageCompareResult.Similar, result & (~expectedDifference));
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
        [ImageFormmatersData]
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
        [ImageFormmatersData]
        public async Task TestNullParametersAsync(Type formatterType, Func<IImageFormatter, Task> func)
        {
            var formatter = (IImageFormatter)Activator.CreateInstance(formatterType);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await func(formatter);
            });
        }
    }
}
