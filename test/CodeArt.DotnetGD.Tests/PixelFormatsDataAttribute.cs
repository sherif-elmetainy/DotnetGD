// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArt.DotnetGD.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class PixelFormatsDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { PixelFormat.Format8BppIndexed};
            yield return new object[] { PixelFormat.Format32BppArgb};
        }
    }
}