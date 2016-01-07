using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeArt.DotnetGD.Formatters
{
    public static class ImageFormatter
    {
        public static IEnumerable<IImageFormatter> GetImageFormatters(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetServices<IImageFormatter>();
        }

        public static IImageFormatter Default { get; } = new PngImageFormatter();

        private static bool HasPublicDefaultConstructor(this Type type)
            => type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract && type.GetTypeInfo().DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0);
        

        public static IReadOnlyCollection<IImageFormatter> DefaultFormatters { get; }
            = new ReadOnlyCollection<IImageFormatter>(typeof(IImageFormatter).GetTypeInfo().Assembly.GetTypes()
                .Where(t => typeof(IImageFormatter).IsAssignableFrom(t)
                    && t.HasPublicDefaultConstructor())
            .Select(t => (IImageFormatter)Activator.CreateInstance(t)).ToList());

        public static IImageFormatter GetImageFormatterForFile(this IServiceProvider serviceProvider, string fileName)
            => GetFormatterForFile(serviceProvider.GetImageFormatters(), fileName);

        public static IImageFormatter GetFormatterForFile(string fileName)
            => GetFormatterForFile(DefaultFormatters, fileName);

        private static IImageFormatter GetFormatterForFile(IEnumerable<IImageFormatter> imageFormatters, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var formatter = imageFormatters.FirstOrDefault(imf => imf.IsSupportedExtension(extension));
            if (formatter == null)
                throw new ArgumentException($"No supported formatter for file '{fileName}' .", nameof(fileName));
            return formatter;
        }

        public static void WriteImageToFile(Image image, string fileName)
             => GetFormatterForFile(fileName).WriteImageToFile(image, fileName);


        public static Task WriteImageToFileAsync(Image image, string fileName)
            => GetFormatterForFile(fileName).WriteImageToFileAsync(image, fileName);


        public static Image ReadImageFromFile(string fileName)
            => GetFormatterForFile(fileName).ReadImageFromFile(fileName);

        public static Task<Image> ReadImageFromFileAsync(string fileName)
            => GetFormatterForFile(fileName).ReadImageFromFileAsync(fileName);
    }
}
