// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.IO;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
{
    internal unsafe class GifAnimationContext : IAnimationContext
    {
        private Stream _stream;
        
        public GifAnimationContext(Stream stream, Image image, bool globalColorMap, int loops)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (image == null) throw new ArgumentNullException(nameof(image));

            _stream = stream;
            var io = new GdIoCtx(_stream);
            try
            {
                NativeWrappers.gdImageGifAnimBeginCtx(image.ImagePtr, &io, globalColorMap ? 1 : 0, loops);
            }
            finally 
            {
                io.Dispose();    
            }
        }

        public void Dispose()
        {
            if (_stream == null)
                return;
            EndAnimation();
        }

        public void AddImage(Image image, bool localColorMap, int leftOffset, int topOffset, int delay, int disposal)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (_stream  == null)
                throw new ObjectDisposedException(nameof(GifAnimationContext));

            var io = new GdIoCtx(_stream);
            try
            {
                NativeWrappers.gdImageGifAnimAddCtx(image.ImagePtr, &io, localColorMap? 1 : 0, leftOffset, topOffset, delay, disposal, null);
            }
            finally
            {
                io.Dispose();
            }
        }

        public void EndAnimation()
        {
            if (_stream == null)
                throw new ObjectDisposedException(nameof(GifAnimationContext));
            var io = new GdIoCtx(_stream);
            try
            {
                NativeWrappers.gdImageGifAnimEndCtx(&io);
            }
            finally
            {
                io.Dispose();
            }
            _stream = null;
        }
    }
}
