// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.Bidi
{
    /// <summary>
    /// Exception thrown when ArabicShaping failes
    /// </summary>
    public class ArabicShapingException : Exception
    {
        public ArabicShapingException(string msg) : base(msg)
        {

        }
    }
}