using System;

namespace DotnetGD
{
    public class LibgdException : Exception
    {
        public LibgdException(string message) : base(message)
        {
        }

        public LibgdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal static string GetErrorMessage(string methodName, string message)
        {
            return $"LIBGD Error: Method ${methodName} failed: ${message}";
        }
    }
}
