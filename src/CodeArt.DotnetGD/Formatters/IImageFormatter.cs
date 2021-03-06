﻿// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeArt.DotnetGD.Formatters
{
    public interface IImageFormatter
    {
        void WriteImageToFile(Image image, string fileName);
        
        Task WriteImageToFileAsync(Image image, string fileName);

        void WriteImageToStream(Image image, Stream stream);
        Task WriteImageToStreamAsync(Image image, Stream stream);

        byte[] EncodeImage(Image image);

        Image ReadImageFromFile(string fileName);

        Task<Image> ReadImageFromFileAsync(string fileName);

        Image ReadImageFromStream(Stream stream);

        Task<Image> ReadImageFromStreamAsync(Stream stream);

        Image DecodeImage(byte[] byteArray);

        bool SupportsAnimation { get; }
        bool CanEncode { get; }
        bool CanDecode { get; }

        IReadOnlyList<string> SupportedExtensions { get; }

        string DefaultExtension { get; }

        string MimeType { get; }

        bool IsLossy { get; }
    }
}

