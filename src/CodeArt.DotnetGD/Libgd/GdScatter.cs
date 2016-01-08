// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.DotnetGD.Libgd
{
    internal unsafe struct GdScatter
    {
        public int Sub;
        public int Plus;
        public uint NumberOfColors;
        public int* Colors;
        public uint Seed;
    }
}
