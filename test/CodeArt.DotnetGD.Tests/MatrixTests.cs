// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class MatrixTests
    {
        [Theory]
        [InlineData("Test_1.png")]
        public void ScaleAndRotate(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                var matrix = Matrix.Identity.Scale(0.5, 2).Rotate(45);
                using (var target = img.Transform(img.Bounds, matrix))
                {
                    target.CompareToReferenceImage(fileName);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void RotateAndScale(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                var matrix = Matrix.Identity.Scale(0.5, 2, MatrixOrder.Append).Rotate(45, MatrixOrder.Append);
                using (var target = img.Transform(img.Bounds, matrix))
                {
                    target.CompareToReferenceImage(fileName);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void ShearVertical(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                var matrix = Matrix.Identity.ShearVertical(45);
                using (var target = img.Transform(img.Bounds, matrix))
                {
                    target.CompareToReferenceImage(fileName);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void ShearHorizontal(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                var matrix = Matrix.Identity.ShearHorizontal(45);
                using (var target = img.Transform(img.Bounds, matrix))
                {
                    target.CompareToReferenceImage(fileName);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void Flip(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                var matrix = Matrix.Identity.Flip(true, true);
                using (var target = img.Transform(img.Bounds, matrix))
                {
                    target.CompareToReferenceImage(fileName);
                }
            }
        }


        [Theory]
        [InlineData("Test_1.png")]
        public void DrawTransform(string fileName)
        {
            using (var img = TestCommon.GetTestImage(fileName))
            {
                var matrix = Matrix.Identity.Rotate(90, MatrixOrder.Append).Translate(img.Height, 0, MatrixOrder.Append);
                using (var target = new Image(img.Height, img.Width))
                {
                    target.DrawImage(img, new Point(), img.Bounds, matrix);
                    target.CompareToReferenceImage(fileName);
                }
            }
        }

    }
}
