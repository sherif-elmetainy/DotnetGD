using System;
using System.Collections.Generic;
using System.Reflection;
using CodeArt.DotnetGD;
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