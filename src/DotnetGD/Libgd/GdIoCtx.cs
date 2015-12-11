using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace DotnetGD.Libgd
{
    internal unsafe struct GdIoCtx : IDisposable
    {
        private const CallingConvention CallbackConvention = CallingConvention.Cdecl;

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
        private delegate long TellDelegate(GdIoCtx* ioCtx);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate void GdFreeDelegate(GdIoCtx* ioCtx);

        public GdIoCtx(Stream stream)
        {
            _getC = Marshal.GetFunctionPointerForDelegate<GetCDelelegate>(GetC);
            _getBuff = Marshal.GetFunctionPointerForDelegate<GetBufDelegate>(GetBuf);
            _putC = Marshal.GetFunctionPointerForDelegate<PutCDelegate>(PutC);
            _putBuf = Marshal.GetFunctionPointerForDelegate<PutBufDelegate>(PutBuf);
            _seek = Marshal.GetFunctionPointerForDelegate<SeekDelegate>(Seek);
            _tell = Marshal.GetFunctionPointerForDelegate<TellDelegate>(Tell);
            _gdFree = Marshal.GetFunctionPointerForDelegate<GdFreeDelegate>(GdFree);
            _data = IntPtr.Zero;
            _key = Interlocked.Increment(ref _currentKey);
            lock (Streams)
            {
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
        //private Stream _data;
        // ReSharper restore NotAccessedField.Local

        private readonly long _key;
        private static long _currentKey;
        private static readonly Dictionary<long, Stream> Streams = new Dictionary<long, Stream>();

        private Stream Stream
        {
            get
            {
                lock (Streams)
                {
                    return Streams[_key];
                }
            }
        }
        



        private static int GetC(GdIoCtx* ioCtx)
        {

            var stream = (*ioCtx).Stream;
            return stream.ReadByte();
        }

        private static int GetBuf(GdIoCtx* ioCtx, IntPtr buff, int size)
        {
            if (size <= 0)
                return 0;
            var stream = (*ioCtx).Stream;
            var managedBuff = new byte[size];
            var res = stream.Read(managedBuff, 0, size);
            Marshal.Copy(managedBuff, 0, buff, res);
            return res;
        }

        private static void PutC(GdIoCtx* ioCtx, int ch)
        {
            var stream = (*ioCtx).Stream;
            stream.WriteByte(unchecked((byte)ch));
        }

        private static int PutBuf(GdIoCtx* ioCtx, IntPtr buff, int size)
        {
            if (size <= 0)
                return 0;
            var stream = (*ioCtx).Stream;
            var managedBuff = new byte[size];
            Marshal.Copy(buff, managedBuff, 0, size);
            stream.Write(managedBuff, 0, size);
            return size;
        }

        private static int Seek(GdIoCtx* ioCtx, int offset)
        {
            var stream = (*ioCtx).Stream;
            stream.Seek(offset, SeekOrigin.Begin);
            return 1;
        }

        private static long Tell(GdIoCtx* ioCtx)
        {
            var stream = (*ioCtx).Stream;
            return stream.Position;
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
