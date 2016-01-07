using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (thickness < 1)
                throw new ArgumentOutOfRangeException(nameof(thickness), thickness, "Pen thickness must be positive.");
            var list = dashColors.ToList();
            if (list.Count == 0)
                throw new ArgumentException("Dash colors collection cannot be empty.", nameof(DashColors));
            Thickness = thickness;
            DashColors = new ReadOnlyCollection<Color>(list);
        }

        public Pen(int thickness) : this(thickness, null)
        {
            
        }

        public Pen() : this(1, null)
        {
            
        }

        public int Thickness { get; }

        public IReadOnlyCollection<Color> DashColors
        {
            get; 
        }

        public bool Equals(Pen other)
        {
            if (other == null)
                return false;
            return Thickness == other.Thickness
                   && CollectionEqualityComparer<Color>.Default.Equals(DashColors, other.DashColors);
            
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
                hash = hash * 31 + CollectionEqualityComparer<Color>.Default.GetHashCode(DashColors);
                return hash;
            }
        }
    }
}
