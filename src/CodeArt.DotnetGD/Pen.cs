// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Pen style for drawing lines, rectangles and polygons
    /// </summary>
    public sealed class Pen : IEquatable<Pen>
    {
        public Pen(int thickness, IEnumerable<Color> dashColors)
        {
            if (dashColors == null) throw new ArgumentNullException(nameof(dashColors));
            
            var ar = dashColors.ToArray();
            if (ar.Length == 0)
                throw new ArgumentException("Dash colors collection cannot be empty.", nameof(DashColors));
            Thickness = thickness;
            DashColors = ar;
        }

        public Pen(int thickness, Color color)
        {
            if (thickness < 1)
                throw new ArgumentOutOfRangeException(nameof(thickness), thickness, "Pen thickness must be positive.");
            Thickness = thickness;
            DashColors = new [] { color };
        }

        public Pen(Color color, bool antiAlias = false) : this(1, color)
        {
            AntiAlias = antiAlias;
        }

        public Pen() : this (Color.Black)
        {
            
        }

        public bool AntiAlias { get; }


        public int Thickness { get; }

        public Color[] DashColors
        {
            get; 
        }

        public Color Color => DashColors[0];
        

        public bool Equals(Pen other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;

            return Thickness == other.Thickness
                   && ArrayEqualityComparer<Color>.Default.Equals(DashColors, other.DashColors)
                   && AntiAlias == other.AntiAlias;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Pen);
        }

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

        public static bool Equals(Pen pen1, Pen pen2)
        {
            if (ReferenceEquals(pen1, pen2)) return true;
            if (pen1 == null || pen2 == null) return false;
            return pen1.Equals(pen2);
        }

    }
}
