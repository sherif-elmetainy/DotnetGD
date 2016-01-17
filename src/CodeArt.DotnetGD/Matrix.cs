// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// A 2D transformation matrix. note this is an immutable type and thread safe. Matrix operations return a new matrix.
    /// </summary>
    public sealed unsafe class Matrix : IEquatable<Matrix>
    {
        internal readonly double[] Data = new double[6];

        /// <summary>
        /// constructor
        /// </summary>
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
                // Using exact equality here so that we don't violate the .net rule that equal objects should have same hashcode.
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Data[i] != other.Data[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Compares  with another object for equality
        /// </summary>
        /// <param name="obj">object to compare</param>
        /// <returns>true if the obj is of type <see cref="Matrix"/> and has same value.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Matrix;
            return other != null && Equals(other);
        }

        /// <summary>
        /// Calculates the 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a new matrix that is the inverse of the speficied matrix.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Multiplies 2 matrices
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data, s1 = m1.Data, s2 = m2.Data)
            {
                NativeWrappers.gdAffineConcat(dst, s1, s2);
            }
            return m;
        }

        /// <summary>
        /// Multiplies 2 matrices
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix m1, Matrix m2) => Multiply(m1, m2);

        /// <summary>
        /// Identity matrix
        /// </summary>
        public static Matrix Identity { get; } = InitIdentity();

        /// <summary>
        /// Zero matrix
        /// </summary>
        public static Matrix Zero { get; } = new Matrix();

        /// <summary>
        /// Initializes identity matrix.
        /// </summary>
        /// <returns></returns>
        private static Matrix InitIdentity()
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineIdentity(dst);
            }
            return m;
        }
        
        /// <summary>
        /// Create a matrix that applies vertical and horizontal scale
        /// </summary>
        /// <param name="scaleX">horizontal scale</param>
        /// <param name="scaleY">vertical scale</param>
        /// <returns></returns>
        private static Matrix CreateScale(double scaleX, double scaleY)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineScale(dst, scaleX, scaleY);
            }
            return m;
        }

        /// <summary>
        /// Create a matrix that applies a clockwise rotation
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <returns></returns>
        private static Matrix CreateRotate(double angle)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineRotate(dst, angle);
            }
            return m;
        }

        /// <summary>
        /// Creates a matrix that applies horizontal shear
        /// </summary>
        /// <param name="angle">shear angle</param>
        /// <returns></returns>
        private static Matrix CreateShearHorizontal(double angle)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineShearHorizontal(dst, angle);
            }
            return m;
        }

        /// <summary>
        /// Creates a matrix that applies vertical shear
        /// </summary>
        /// <param name="angle">shear angle</param>
        /// <returns></returns>
        private static Matrix CreateShearVertical(double angle)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineShearVertical(dst, angle);
            }
            return m;
        }

        /// <summary>
        /// Creates a matrix that applies a translate
        /// </summary>
        /// <param name="offsetX">horizontal offset</param>
        /// <param name="offsetY">vertical offset</param>
        /// <returns></returns>
        private static Matrix CreateTranslate(double offsetX, double offsetY)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data)
            {
                NativeWrappers.gdAffineTranslate(dst, offsetX, offsetY);
            }
            return m;
        }

        /// <summary>
        /// Calculates the matrix laplace expansion https://en.wikipedia.org/wiki/Laplace_expansion
        /// </summary>
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

        /// <summary>
        /// whether the matrix is axis aligned.
        /// </summary>
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

        /// <summary>
        /// Whether the matrixes are equal. This differs from the <see cref="Equals(CodeArt.DotnetGD.Matrix)"/> method 
        /// in that <see cref="double"/> values are compared to be with in 1e-6 from each other rather than exactly equal.
        /// That is the matrixes are aproximately equal
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static bool AreSimilar(Matrix m1, Matrix m2)
        {
            fixed (double* s1 = m1.Data, s2 = m2.Data)
            {
                return NativeWrappers.gdAffineEqual(s1, s2) != 0;
            }
        }

        /// <summary>
        /// Creates a matrix that applies a horizontal or vertical flip
        /// </summary>
        /// <param name="flipHorizontal">whether to flip horizontaly</param>
        /// <param name="flipVertical">whether to flip vertically</param>
        /// <returns></returns>
        public Matrix Flip(bool flipHorizontal, bool flipVertical)
        {
            var m = new Matrix();
            fixed (double* dst = m.Data, src = Data)
            {
                NativeWrappers.gdAffineFlip(dst, src, flipHorizontal ? 1 : 0, flipVertical ? 1 : 0);
            }
            return m;
        }

        /// <summary>
        /// Applies a rotate transformation to the matrix and returns a new matrix.
        /// </summary>
        /// <param name="angle">clockwise rotation angle in degrees.</param>
        /// <param name="matrixOrder">Matrix order append means transformation applied after the existing transformation, prepend means before</param>
        /// <returns></returns>
        public Matrix Rotate(double angle, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateRotate(angle), matrixOrder);
        }

        /// <summary>
        /// Applies a vertical shear tranformation and returns a new matrix
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <param name="matrixOrder">Matrix order append means transformation applied after the existing transformation, prepend means before</param>
        /// <returns></returns>
        public Matrix ShearVertical(double angle, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateShearVertical(angle), matrixOrder);
        }

        /// <summary>
        /// applies a horizontal shear and returns a new matrix
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <param name="matrixOrder">Matrix order append means transformation applied after the existing transformation, prepend means before</param>
        /// <returns></returns>
        public Matrix ShearHorizontal(double angle, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateShearHorizontal(angle), matrixOrder);
        }

        /// <summary>
        /// Applies a scale tranformation and returns a new matrix
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="matrixOrder"></param>
        /// <returns></returns>
        public Matrix Scale(double scaleX, double scaleY, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateScale(scaleX, scaleY), matrixOrder);
        }

        /// <summary>
        /// applies a translate (move) transformation and returns a new matrix.
        /// </summary>
        /// <param name="offsetX">horizontal offset</param>
        /// <param name="offsetY">vertical offset</param>
        /// <param name="matrixOrder">Matrix order append means transformation applied after the existing transformation, prepend means before</param>
        /// <returns></returns>
        public Matrix Translate(double offsetX, double offsetY, MatrixOrder matrixOrder = MatrixOrder.Prepend)
        {
            return Apply(CreateTranslate(offsetX, offsetY), matrixOrder);
        }

        /// <summary>
        /// applies a transformation and returns a new matrix
        /// </summary>
        /// <param name="other"></param>
        /// <param name="matrixOrder">Matrix order append means transformation applied after the existing transformation, prepend means before</param>
        /// <returns></returns>
        private Matrix Apply(Matrix other, MatrixOrder matrixOrder)
        {
            return matrixOrder != MatrixOrder.Prepend ?
                Multiply(this, other) :
                Multiply(other, this);
        }

        /// <summary>
        /// Trasform a point and returns a new poinst
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Trasforms an array of points and returns a new array of transformed points.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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
