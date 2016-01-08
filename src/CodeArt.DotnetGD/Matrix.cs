// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public sealed unsafe class Matrix : IEquatable<Matrix>
    {
        internal readonly double[] Data = new double[6];

        private Matrix()
        {
            
        }

        public bool Equals(Matrix other)
        {
            if (other == null)
                return false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < Data.Length; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Data[i] != other.Data[i]) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Matrix;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < Data.Length; i++)
                {
                    hash = hash * 31 + Data[i].GetHashCode();
                }
                return hash;
            }
        }

        public Matrix Invert()
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                fixed (double* src = Data)
                {
                    NativeWrappers.gdAffineInvert(dst, src);
                }
            }
            return m;
        }

        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data, s1 = m1.Data, s2 = m2.Data)
            {
                NativeWrappers.gdAffineConcat(dst, s1, s2);
            }
            return m;
        }

        public static Matrix operator *(Matrix m1, Matrix m2) => Multiply(m1, m2);

        public static Matrix Identity { get; } = InitIdentity();
        public static Matrix Zero { get; } = new Matrix();

        private static Matrix InitIdentity()
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineIdentity(dst);
            }
            return m;
        }

       

        private static Matrix CreateScale(double scaleX, double scaleY)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineScale(dst, scaleX, scaleY);
            }
            return m;
        }

        private static Matrix CreateRotate(double angle)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineRotate(dst, angle);
            }
            return m;
        }

        private static Matrix CreateShearHorizontal(double angle)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineShearHorizontal(dst, angle);
            }
            return m;
        }

        private static Matrix CreateShearVertical(double angle)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineShearVertical(dst, angle);
            }
            return m;
        }

        private static Matrix CreateTranslate(double offsetX, double offsetY)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineTranslate(dst, offsetX, offsetY);
            }
            return m;
        }

        public double Expansion
        {
            get
            {
                fixed (double* src = Data)
                {
                    return NativeWrappers.gdAffineExpansion(src);
                }
            }
        }

        public bool IsRectilinear
        {
            get
            {
                fixed (double* src = Data)
                {
                    return NativeWrappers.gdAffineRectilinear(src) != 0;
                }
            }
        }

        public static bool AreSimilar(Matrix m1, Matrix m2)
        {
            fixed (double* s1 = m1.Data, s2 = m2.Data)
            {
                return NativeWrappers.gdAffineEqual(s1, s2) != 0;
            }
        }

        public Matrix Flip(bool flipHorizontal, bool flipVertical)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data, src = Data)
            {
                NativeWrappers.gdAffineFlip(dst, src, flipHorizontal ? 1 : 0, flipVertical ? 1 : 0);
            }
            return m;
        }

        public Matrix Rotate(double angle, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateRotate(angle), matrixOrder);
        }

        public Matrix ShearVertical(double angle, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateShearVertical(angle), matrixOrder);
        }

        public Matrix ShearHorizontal(double angle, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateShearHorizontal(angle), matrixOrder);
        }

        public Matrix Scale(double scaleX, double scaleY, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateScale(scaleX, scaleY), matrixOrder);
        }

        public Matrix Translate(double offsetX, double offsetY, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateTranslate(offsetX, offsetY), matrixOrder);
        }

        private Matrix Apply(Matrix other, MatrixOrder matrixOrder)
        {
            return matrixOrder != MatrixOrder.Prepend ?
                Multiply(this, other) :
                Multiply(other, this);
        }

        public PointF Transform(PointF point)
        {
            fixed (double* s = Data)
            {
                var p = new PointF();
                PointF* dst = &p, src = &point;
                NativeWrappers.gdAffineApplyToPointF(dst, src, s);
                return p;
            }
        }

        public PointF[] Transform(PointF[] point)
        {
            var result = new PointF[point.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = Transform(point[i]);
            }
            return result;
        }

    }
}
