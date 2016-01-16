// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace CodeArt.DotnetGD.Libgd
{
    /// <summary>
    ///     A GdIoCtx structure to support passing streams to LibGD
    /// </summary>
    internal unsafe struct GdIoCtx : IDisposable
    {
        private const CallingConvention CallbackConvention = CallingConvention.Cdecl;

        // Delegate types used by call backs
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int GetCDelelegate(GdIoCtx* ioCtx);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int GetBufDelegate(GdIoCtx* ioCtx, IntPtr buff, int size);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate void PutCDelegate(GdIoCtx* ioCtx, int ch);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int PutBufDelegate(GdIoCtx* ioCtx, IntPtr buff, int size);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int SeekDelegate(GdIoCtx* ioCtx, int offset);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int TellDelegate(GdIoCtx* ioCtx);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate void GdFreeDelegate(GdIoCtx* ioCtx);
        
        private static readonly GetCDelelegate GetCDel = GetC;
        private static readonly GetBufDelegate GetBufDel = GetBuf;
        private static readonly PutCDelegate PutCDel = PutC;
        private static readonly PutBufDelegate PutBufDel = PutBuf;
        private static readonly SeekDelegate SeekDel = Seek;
        private static readonly TellDelegate TellDel = Tell;
        private static readonly GdFreeDelegate GdFreeDel = GdFree;
        
        private static readonly IntPtr GetCPtr = Marshal.GetFunctionPointerForDelegate(GetCDel);
        private static readonly IntPtr GetBufPtr = Marshal.GetFunctionPointerForDelegate(GetBufDel);
        private static readonly IntPtr PutCPtr = Marshal.GetFunctionPointerForDelegate(PutCDel);
        private static readonly IntPtr PutBufPtr = Marshal.GetFunctionPointerForDelegate(PutBufDel);
        private static readonly IntPtr SeekPtr = Marshal.GetFunctionPointerForDelegate(SeekDel);
        private static readonly IntPtr TellPtr = Marshal.GetFunctionPointerForDelegate(TellDel);
        private static readonly IntPtr GdFreePtr = Marshal.GetFunctionPointerForDelegate(GdFreeDel);

        public GdIoCtx(Stream stream)
        {
            // Setup callbacks called by libgd to read from or write to the stream
            _getC = GetCPtr;
            _getBuff = GetBufPtr;
            _putC = PutCPtr;
            _putBuf = PutBufPtr;
            _seek = SeekPtr;
            _tell = TellPtr;
            _gdFree = GdFreePtr;


            _data = IntPtr.Zero;
            lock (Streams)
            {
                _key = _currentKey++;
                Streams.Add(_key, stream);
            }
        }

        // ReSharper disable NotAccessedField.Local
        private readonly IntPtr _getC;
        private readonly IntPtr _getBuff;
        private readonly IntPtr _putC;
        private readonly IntPtr _putBuf;
        private readonly IntPtr _seek;
        private readonly IntPtr _tell;
        private readonly IntPtr _gdFree;
        private readonly IntPtr _data;
        // ReSharper restore NotAccessedField.Local

        // Since it's not possible to have an instance member managed reference and obtain a pointer to the GdIoCtx
        // We store the stream object in a static dictionary and have the key as an instance member
        private readonly long _key;
        private static long _currentKey;
        private static readonly Dictionary<long, Stream> Streams = new Dictionary<long, Stream>();

        private static Stream GetStream(GdIoCtx* ioCtx)
        {
            if (ioCtx == null)
            {
                throw new ArgumentNullException(nameof(ioCtx));
            }
            var key = ioCtx->_key;
            lock (Streams)
            {
                return Streams[key];
            }
        }


        private static int GetC(GdIoCtx* ioCtx)
        {
            var stream = GetStream(ioCtx);
            return stream.ReadByte();
        }

        private static int GetBuf(GdIoCtx* ioCtx, IntPtr buff, int size)
        {
            if (size <= 0)
                return 0;
            var stream = GetStream(ioCtx);
            var managedBuff = new byte[size];
            var res = stream.Read(managedBuff, 0, size);
            Marshal.Copy(managedBuff, 0, buff, res);
            return res;
        }

        private static void PutC(GdIoCtx* ioCtx, int ch)
        {
            var stream = GetStream(ioCtx);
            stream.WriteByte(unchecked((byte)ch));
        }

        private static int PutBuf(GdIoCtx* ioCtx, IntPtr buff, int size)
        {
            if (size <= 0)
                return 0;
            var stream = GetStream(ioCtx);
            var managedBuff = new byte[size];
            Marshal.Copy(buff, managedBuff, 0, size);
            stream.Write(managedBuff, 0, size);
            return size;
        }

        private static int Seek(GdIoCtx* ioCtx, int offset)
        {
            var stream = GetStream(ioCtx);
            stream.Seek(offset, SeekOrigin.Begin);
            return 1;
        }

        private static int Tell(GdIoCtx* ioCtx)
        {
            var stream = GetStream(ioCtx);
            return (int)stream.Position;
        }

        private static void GdFree(GdIoCtx* ioCtx)
        {
            // There is nothing to free
            // Do Nothing
        }

        public void Dispose()
        {
            lock (Streams)
            {
                Streams.Remove(_key);
            }
        }

    }


}
