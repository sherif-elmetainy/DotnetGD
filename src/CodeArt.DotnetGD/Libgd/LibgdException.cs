// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD.Libgd
{
    /// <summary>
    /// An error occuring in libgd itself
    /// </summary>
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
