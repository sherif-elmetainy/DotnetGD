// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Pen style for drawing lines, rectangles and polygons. This class is immutable and thread safe.
    /// </summary>
    public sealed class Pen : IEquatable<Pen>
    {
        /// <summary>
        /// Creates a new instance of <see cref="Pen"/>.
        /// </summary>
        /// <param name="thickness">line thickness</param>
        /// <param name="dashColors">dash colors. If it contains a single color the line is solid, otherwise it's styled using the specified colors.</param>
        public Pen(int thickness, IEnumerable<Color> dashColors)
        {
            if (dashColors == null) throw new ArgumentNullException(nameof(dashColors));
            
            var ar = dashColors.ToArray();
            if (ar.Length == 0)
                throw new ArgumentException("Dash colors collection cannot be empty.", nameof(DashColors));
            Thickness = thickness;
            DashColors = ar;
        }

        /// <summary>
        /// Creates a new instance of solid color <see cref="Pen"/>.
        /// </summary>
        /// <param name="thickness">thickness</param>
        /// <param name="color">color</param>
        public Pen(int thickness, Color color)
        {
            if (thickness < 1)
                throw new ArgumentOutOfRangeException(nameof(thickness), thickness, "Pen thickness must be positive.");
            Thickness = thickness;
            DashColors = new [] { color };
        }

        /// <summary>
        /// Create an thickness 1 solid color pen
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="antiAlias">whether to use anti-alias drawing algorithm</param>
        public Pen(Color color, bool antiAlias = false) : this(1, color)
        {
            AntiAlias = antiAlias;
        }

        /// <summary>
        /// Create a default pen (thickness 1, solid color black)
        /// </summary>
        public Pen() : this (Color.Black)
        {
            
        }

        /// <summary>
        /// Whether anti alias algorithm should be used
        /// </summary>
        public bool AntiAlias { get; }

        /// <summary>
        /// Thickness
        /// </summary>
        public int Thickness { get; }

        /// <summary>
        /// Dash styled colors
        /// </summary>
        public Color[] DashColors
        {
            get; 
        }

        /// <summary>
        /// Solid color
        /// </summary>
        public Color Color => DashColors[0];
        

        /// <summary>
        /// Compares two pens for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Pen other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;

            return Thickness == other.Thickness
                   && ArrayEqualityComparer<Color>.Default.Equals(DashColors, other.DashColors)
                   && AntiAlias == other.AntiAlias;
        }

        /// <summary>
        /// compares the pen to another object for equality
        /// </summary>
        /// <param name="obj">object to compare</param>
        /// <returns>true if the obj is a pen and equals to this pen</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Pen);
        }

        /// <summary>
        /// Gets the hash code for a pen.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + Thickness;
                hash = hash * 31 + ArrayEqualityComparer<Color>.Default.GetHashCode(DashColors);
                hash = hash * 31 + AntiAlias.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Compares two pens for equality
        /// </summary>
        /// <param name="pen1"></param>
        /// <param name="pen2"></param>
        /// <returns></returns>
        public static bool Equals(Pen pen1, Pen pen2)
        {
            if (ReferenceEquals(pen1, pen2)) return true;
            if (pen1 == null || pen2 == null) return false;
            return pen1.Equals(pen2);
        }
    }
}
