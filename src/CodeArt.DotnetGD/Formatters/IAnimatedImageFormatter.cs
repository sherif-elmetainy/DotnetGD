// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.IO;

namespace CodeArt.DotnetGD.Formatters
{
    public interface IAnimatedImageFormatter
    {
        IAnimationContext BeginAnimation(Image image, Stream outStream, bool globalColorMap, int loops);
    }
}
