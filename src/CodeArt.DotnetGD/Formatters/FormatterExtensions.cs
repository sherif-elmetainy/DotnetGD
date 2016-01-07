using System;
using System.Linq;

namespace CodeArt.DotnetGD.Formatters
{
    public static class ImageFormatterExtensions
    {
        public static bool IsSupportedExtension(this IImageFormatter formatter, string extension)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (string.IsNullOrWhiteSpace(extension)) throw new ArgumentNullException(nameof(extension));

            return formatter.SupportedExtensions.Any(e => CompareExtensions(e, extension));
        }

        private static bool CompareExtensions(string ext1, string ext2)
        {
            if (string.IsNullOrWhiteSpace(ext1)) return string.IsNullOrWhiteSpace(ext2);
            if (string.IsNullOrWhiteSpace(ext2)) return false;

            var index1 = ext1[0] == '.' ? 1 : 0;
            var index2 = ext2[0] == '.' ? 1 : 0;
            if (ext1.Length - index1 != ext2.Length - index2) return false;
            return string.Compare(ext1, index1, ext2, index2, ext1.Length - index1, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
