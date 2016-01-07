using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
{
    public abstract class BaseImageFormatter : IImageFormatter
    {
        public virtual void WriteImageToFile(Image image, string fileName)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            using (var fs = File.Create(fileName))
            {
                WriteImageToStream(image, fs);
            }
        }

        public virtual async Task WriteImageToFileAsync(Image image, string fileName)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            using (var fs = File.Create(fileName))
            {
                await WriteImageToStreamAsync(image, fs).ConfigureAwait(false);
            }
        }

        internal abstract unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx);


        public virtual unsafe void WriteImageToStream(Image image, Stream stream)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException("Stream must be writeable.", nameof(stream));
            var io = new GdIoCtx(stream);
            try
            {
                WriteImageToGdIoCtx(image.ImagePtr, &io);
            }
            finally
            {
                io.Dispose();
            }
        }

        public virtual Task WriteImageToStreamAsync(Image image, Stream stream)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException("Stream must be writeable.", nameof(stream));
            return Task.Run(() => WriteImageToStream(image, stream));
        }
        internal abstract unsafe IntPtr ImageToPtr(GdImage* img, out int size);

        public virtual unsafe byte[] EncodeImage(Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            int size;
            var res = ImageToPtr(image.ImagePtr, out size);
            try
            {
                var managedBytes = new byte[size];
                Marshal.Copy(res, managedBytes, 0, size);
                return managedBytes;
            }
            finally
            {
                NativeWrappers.gdFree(res);
            }
        }

        public virtual Image ReadImageFromFile(string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            {
                return ReadImageFromStream(fs);
            }
        }

        public virtual async Task<Image> ReadImageFromFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            using (var fs = File.OpenRead(fileName))
            {
                return await ReadImageFromStreamAsync(fs).ConfigureAwait(false);
            }
        }

        internal abstract unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx);


        public virtual unsafe Image ReadImageFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Stream must be readable.", nameof(stream));

            var io = new GdIoCtx(stream);
            try
            {
                var res = ImageCreateFromCtx(&io);
                return new Image(res);
            }
            finally
            {
                io.Dispose();
            }
        }

        public virtual Task<Image> ReadImageFromStreamAsync(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return Task.Run(() => ReadImageFromStream(stream));
        }

        internal abstract unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr);

        public virtual unsafe Image DecodeImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) throw new ArgumentNullException(nameof(byteArray));
            var ptr = Marshal.AllocHGlobal(byteArray.Length);

            try
            {
                Marshal.Copy(byteArray, 0, ptr, byteArray.Length);
                var res = ImageCreateFromPtr(byteArray.Length, ptr);
                return new Image(res);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public virtual bool SupportsAnimation => false;

        public virtual bool CanEncode => true;

        public virtual bool CanDecode => true;

        public abstract IReadOnlyList<string> SupportedExtensions { get; }

        public virtual string DefaultExtension => SupportedExtensions.First();

        public abstract string MimeType { get; }
        public abstract bool IsLossy { get; }


    }
}
