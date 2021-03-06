﻿// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.IO;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class ImageCopyTests
    {

        [Theory]
        [InlineData("Test_1.png")]
        public void Clone(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                using (var clone = img.Clone())
                {
                    Assert.NotSame(clone, img);
                    var result = clone.CompareTo(img);
                    Assert.Equal(ImageCompareResult.Similar, result);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void TrueColorToPalleteDefault(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                using (var clone = img.Copy(PixelFormat.Format8BppIndexed))
                {
                    Assert.Equal(PixelFormat.Format8BppIndexed, clone.PixelFormat);
                    clone.CompareToReferenceImage(Path.ChangeExtension(fileName, null));
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void TrueColorToPallete64Colors(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                using (var clone = img.Copy(PixelFormat.Format8BppIndexed, numberOfColorsWanted: 64))
                {
                    Assert.Equal(PixelFormat.Format8BppIndexed, clone.PixelFormat);
                    clone.CompareToReferenceImage(Path.ChangeExtension(fileName, null));
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void TrueColorToPalleteDither(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                using (var clone = img.Copy(PixelFormat.Format8BppIndexed, dither: true))
                {
                    Assert.Equal(PixelFormat.Format8BppIndexed, clone.PixelFormat);
                    clone.CompareToReferenceImage(Path.ChangeExtension(fileName, null));
                }
            }
        }

        //[Theory]
        //[InlineData("Test_1.png")]
        //public void ColorMatch(string fileName)
        //{
        //    using (var img = TestCommon.GetTestImage(fileName))
        //    {
        //        using (var clone256 = img.Copy(PixelFormat.Format8BppIndexed))
        //        {
        //            using (var clone64 = img.Copy(PixelFormat.Format8BppIndexed, numberOfColorsWanted: 64))
        //            {
        //                var result = clone64.CompareTo(clone256);
        //                Assert.Equal(ImageCompareResult.Color | ImageCompareResult.Image | ImageCompareResult.NumberOfColors, result);

        //                clone64.ColorMatch(clone256);
        //                result = clone64.CompareTo(clone256);
        //                Assert.Equal(ImageCompareResult.Similar, result);
        //            }
        //        }
        //    }
        //}

        [Theory]
        [InlineData("Test_2.png")]
        public void PalleteToTrueColor(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                using (var clone = img.Copy(PixelFormat.Format32BppArgb, dither: true))
                {
                    Assert.Equal(PixelFormat.Format32BppArgb, clone.PixelFormat);
                    var result = clone.CompareTo(img);
                    Assert.Equal(ImageCompareResult.TrueColor, result);
                }
            }
        }


    }
}
