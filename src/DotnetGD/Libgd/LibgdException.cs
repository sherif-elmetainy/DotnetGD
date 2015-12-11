using System;

namespace DotnetGD.Libgd
{
    public class LibgdException : Exception
    {
        public LibgdException(string message) : base(message)
        {
        }

        public LibgdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        
    }
}
