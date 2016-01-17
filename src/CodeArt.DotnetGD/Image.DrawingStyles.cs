// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// Currently used pen
        /// </summary>
        private Pen _pen;
        /// <summary>
        /// Currently used brush
        /// </summary>
        private Image _brush;
        /// <summary>
        /// Currently used tile
        /// </summary>
        private Image _tile;

        /// <summary>
        /// Const passed to libgd to draw styled lines (dashed)
        /// </summary>
        private const int GdStyled = -2;
        /// <summary>
        /// Const passed to libgd to draw brushed lines (dashed)
        /// </summary>
        private const int GdBrushed = -3;

        /// <summary>
        /// Const passed to libgd to fill figures with a tile
        /// </summary>
        private const int GdTiled = -5;

        /// <summary>
        /// Const passed to libgd to draw anti-alias lines
        /// </summary>
        private const int GdAntiAlias = -7;

        /// <summary>
        /// Gets or sets the drawing effect used for drawing functions
        /// </summary>
        public DrawingEffect DrawingEffect
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->AlphaBlendingFlag;
            }
            set
            {
                if (value < DrawingEffect.Replace && value >= DrawingEffect.Invalid)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Drawing effect.");
                ImagePtr->AlphaBlendingFlag = value;
            }
        }

        /// <summary>
        /// Sets the brush used for drawing lines, rectangles and polygons.
        /// </summary>
        /// <param name="brush">image to use as brush.</param>
        private void SetBrush(Image brush)
        {
            if (ReferenceEquals(brush, _brush)) return;
            if (ReferenceEquals(this, brush)) throw new ArgumentException("Image cannot be used as brush for itself.", nameof(brush));
            _brush?.Dispose();
            if (brush != null)
            {
                brush.AddReference();
                NativeWrappers.gdImageSetBrush(ImagePtr, brush.ImagePtr);
            }
            else
            {
                ImagePtr->Brush = null;
            }
            _brush = brush;
            SetPen(null);
        }

        /// <summary>
        /// Sets the tile to use for filling figures.
        /// </summary>
        /// <param name="tile"></param>
        private void SetTile(Image tile)
        {
            if (ReferenceEquals(tile, _tile)) return;
            if (ReferenceEquals(this, tile)) throw new ArgumentException("Image cannot be used as tile for itself.", nameof(tile));
            _tile?.Dispose();
            if (tile != null)
            {
                tile.AddReference();
                NativeWrappers.gdImageSetTile(ImagePtr, tile.ImagePtr);
            }
            else
            {
                ImagePtr->Tile = null;
            }
            _tile = tile;
            SetPen(null);
        }

        /// <summary>
        /// Set the pen to use for drawing lines, arcs, etc
        /// </summary>
        /// <param name="value"></param>
        private void SetPen(Pen value)
        {
            if (Pen.Equals(_pen, value))
                return;
            if (value == null)
            {
                ImagePtr->Thick = 1;
                ClearOldLineStyle();
                ImagePtr->AA = 0;
                ImagePtr->AADontBlend = -1;
                ImagePtr->AAColor = 0;
            }
            else
            {
                ImagePtr->Thick = value.Thickness;
                if (value.DashColors.Length == 1)
                {
                    if (value.AntiAlias)
                    {
                        ImagePtr->AA = 1;
                        ImagePtr->AAColor = ResolveColor(value.Color);
                        ImagePtr->AADontBlend = -1;
                    }
                    else
                    {
                        ImagePtr->AA = 0;
                    }
                    ClearOldLineStyle();
                }
                else
                {
                    var colors = value.DashColors.Select(ResolveColor).ToArray();
                    var ptr = Marshal.AllocHGlobal(sizeof (int)*colors.Length);
                    try
                    {
                        Marshal.Copy(colors, 0, ptr, colors.Length);
                        // when calling gdImageSetStyle old style is freed by libgd
                        // No need to call ClearOldLineStyle()
                        NativeWrappers.gdImageSetStyle(ImagePtr, ptr, colors.Length);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }
            _pen = value;
        }

        /// <summary>
        /// Clears the old line style 
        /// </summary>
        private void ClearOldLineStyle()
        {
            // when calling gdImageSetStyle old style is freed by libgd
            if (ImagePtr->Style == null) return;
            NativeWrappers.gdFree((IntPtr)ImagePtr->Style);
            ImagePtr->Style = null;
            ImagePtr->StyleLength = 0;
            ImagePtr->StylePos = 0;
        }
    }
}
