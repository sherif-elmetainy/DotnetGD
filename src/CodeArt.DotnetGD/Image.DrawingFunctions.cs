// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.IO;
using System.Text;
using CodeArt.Bidi;
using CodeArt.DotnetGD.Libgd;
using Microsoft.Extensions.PlatformAbstractions;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// Gets the color or a pixel at given point
        /// </summary>
        /// <param name="p">Point to get pixel at</param>
        /// <returns>color of the pixel</returns>
        public Color GetPixel(Point p)
        {
            CheckObjectDisposed();
            if (!Bounds.Contains(p)) throw new ArgumentException("Point is outside image bounds.", nameof(p));
            var color = NativeWrappers.gdImageGetTrueColorPixel(ImagePtr, p.X, p.Y);
            return GdTrueColorToColor(color);
        }

        /// <summary>
        /// Sets the color of pixel at a given point
        /// </summary>
        /// <param name="p">Point to set color at</param>
        /// <param name="color">color</param>
        public void SetPixel(Point p, Color color)
        {
            CheckObjectDisposed();
            if (!Bounds.Contains(p)) throw new ArgumentException("Point is outside image bounds.", nameof(p));
            var colorIndex = ResolveColor(color);
            NativeWrappers.gdImageSetPixel(ImagePtr, p.X, p.Y, colorIndex);
        }

        /// <summary>
        /// Replace all pixels of one color with another
        /// </summary>
        /// <param name="oldValue">old color</param>
        /// <param name="newValue">new color</param>
        /// <returns>number of pixels replaced</returns>
        public int ReplacePixels(Color oldValue, Color newValue)
        {
            CheckObjectDisposed();
            var c1 = ResolveColor(oldValue);
            var c2 = ResolveColor(newValue);
            return NativeWrappers.gdImageColorReplace(ImagePtr, c1, c2);
        }

        /// <summary>
        /// Replace all pixels of one color with another
        /// </summary>
        /// <param name="oldValue">old color</param>
        /// <param name="newValue">new color</param>
        /// <param name="threshold">threshold representing the maximum distance between searched color and matched color as a percentage. The higher the threshold the more likely the match.</param>
        /// <returns>number of pixels replaced</returns>
        public int ReplacePixels(Color oldValue, Color newValue, float threshold)
        {
            CheckObjectDisposed();
            var c1 = ResolveColor(oldValue);
            var c2 = ResolveColor(newValue);
            return NativeWrappers.gdImageColorReplaceThreshold(ImagePtr, c1, c2, threshold);
        }

        /// <summary>
        /// Replace all pixels of one color with another
        /// </summary>
        /// <param name="oldValues">old colors array must be of same length as newValues array</param>
        /// <param name="newValues">new color array must be of same length as oldValues array</param>
        /// <returns>number of pixels replaced</returns>
        public int ReplacePixels(Color[] oldValues, Color[] newValues)
        {
            CheckObjectDisposed();
            if (oldValues == null) throw new ArgumentNullException(nameof(oldValues));
            if (newValues == null) throw new ArgumentNullException(nameof(newValues));
            if (oldValues.Length != newValues.Length) throw new ArgumentException($"Array {nameof(oldValues)} length ({oldValues.Length} does not equal array {nameof(newValues)} length ({newValues.Length}).", nameof(newValues));
            if (oldValues.Length == 0) return 0;
                
            var c1 = new int[oldValues.Length];
            var c2 = new int[newValues.Length];
            for (var i = 0; i < oldValues.Length; i++)
            {
                c1[i] = ResolveColor(oldValues[i]);
                c2[i] = ResolveColor(newValues[i]);
            }
            fixed (int* p1 = c1)
            {
                fixed (int* p2 = c2)
                {
                    return NativeWrappers.gdImageColorReplaceArray(ImagePtr, c1.Length, p1, p2);
                }
            }
        }

        #region DrawLine overloads

        /// <summary>
        /// Draws a straight line between 2 points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="color"></param>
        public void DrawLine(Point p1, Point p2, Color color) => DrawLine(p1, p2, new Pen(color));
        
        /// <summary>
        /// Draws a straight line between 2 points using a pen
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="pen"></param>
        public void DrawLine(Point p1, Point p2, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            NativeWrappers.gdImageLine(ImagePtr, p1.X, p1.Y, p2.X, p2.Y, GetPenColor(pen));
        }

        /// <summary>
        /// Draws a straight line between 2 points using a brush
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="brush"></param>
        public void DrawLine(Point p1, Point p2, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            NativeWrappers.gdImageLine(ImagePtr, p1.X, p1.Y, p2.X, p2.Y, GdBrushed);
        }
        #endregion

        #region DrawRecangle overloads

        /// <summary>
        /// Draws a straight rectangle between 2 points
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        public void DrawRectangle(Rectangle rectangle, Color color) => DrawRectangle(rectangle, new Pen(color));
        
        /// <summary>
        /// Draws a straight rectangle using a pen
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="pen"></param>
        public void DrawRectangle(Rectangle rectangle, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            NativeWrappers.gdImageRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, GetPenColor(pen));
        }

        /// <summary>
        /// Draws a rectangle using a brush
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="brush"></param>
        public void DrawRectangle(Rectangle rectangle, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            NativeWrappers.gdImageRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, GdBrushed);
        }
        #endregion

        #region DrawFilledRectangle overloads

        /// <summary>
        /// Draws a filled rectangle using solid color
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        public void DrawFilledRectangle(Rectangle rectangle, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageFilledRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, resolvedColor);
        }

        /// <summary>
        /// Draws a filled rectangle using a tile
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="tile"></param>
        public void DrawFilledRectangle(Rectangle rectangle, Image tile)
        {
            CheckObjectDisposed();
            SetTile(tile);
            NativeWrappers.gdImageFilledRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, GdTiled);
        }
        #endregion DrawFilledRectangle overloads

        #region DrawEllipse overloads

        /// <summary>
        /// Draws an ellipse
        /// </summary>
        /// <param name="center">center of the ellipse</param>
        /// <param name="size">width and height of the ellipse (i.e. radii * 2)</param>
        /// <param name="color">color</param>
        public void DrawEllipse(Point center, Size size, Color color)
        {
            CheckObjectDisposed();
            SetPen(null);
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, resolvedColor);
        }

        // Removed because style works with line drawing functions (gdImageLine, gdImageRectangle, gdImagePolygon, etc)
        //public void DrawEllipse(Point center, Size size, Pen pen)
        //{
        //    CheckObjectDisposed();
        //    SetPen(pen);
        //    NativeWrappers.gdImageEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, GdStyled);
        //}

        /// <summary>
        /// Draws an ellipse using a brush
        /// </summary>
        /// <param name="center">center of the ellipse</param>
        /// <param name="size">width and height of the ellipse (i.e. radii * 2)</param>
        /// <param name="brush">color</param>
        public void DrawEllipse(Point center, Size size, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            NativeWrappers.gdImageEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, GdBrushed);
        }
        #endregion

        #region DrawFilledEllipse overloads

        /// <summary>
        /// Draws a filled ellipse with solid color
        /// </summary>
        /// <param name="center">center of the ellipse</param>
        /// <param name="size">width and height of the ellipse (i.e. radii * 2)</param>
        /// <param name="color">color</param>
        public void DrawFilledEllipse(Point center, Size size, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageFilledEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, resolvedColor);
        }

        /// <summary>
        /// Draws a filled ellipse using a tile
        /// </summary>
        /// <param name="center">center of the ellipse</param>
        /// <param name="size">width and height of the ellipse (i.e. radii * 2)</param>
        /// <param name="tile"></param>
        public void DrawFilledEllipse(Point center, Size size, Image tile)
        {
            CheckObjectDisposed();
            SetTile(tile);
            NativeWrappers.gdImageFilledEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, GdTiled);
        }
        #endregion

        #region Polygon methods

        /// <summary>
        /// Draws a polygon
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="color"></param>
        public void DrawPolygon(Point[] points, Color color) => DrawPolygon(points, new Pen(color));
        
        /// <summary>
        /// Draws a polygon using a pen
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="pen"></param>
        public void DrawPolygon(Point[] points, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            fixed (Point* ptr = points)
            {
                NativeWrappers.gdImagePolygon(ImagePtr, ptr, points.Length, GetPenColor(pen));
            }
        }

        /// <summary>
        /// Draws a polygon using a brush
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="brush"></param>
        public void DrawPolygon(Point[] points, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            fixed (Point* ptr = points)
            {
                NativeWrappers.gdImagePolygon(ImagePtr, ptr, points.Length, GdBrushed);
            }
        }

        /// <summary>
        /// Draws an open polygon 
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="color"></param>
        public void DrawOpenPolygon(Point[] points, Color color) => DrawOpenPolygon(points, new Pen(color));

        public void DrawOpenPolygon(Point[] points, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            fixed (Point* ptr = points)
            {
                NativeWrappers.gdImageOpenPolygon(ImagePtr, ptr, points.Length, GetPenColor(pen));
            }
        }

        /// <summary>
        /// Draws an open polygon using a brush
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="brush"></param>
        public void DrawOpenPolygon(Point[] points, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            fixed (Point* ptr = points)
            {
                NativeWrappers.gdImageOpenPolygon(ImagePtr, ptr, points.Length, GdBrushed);
            }
        }

        /// <summary>
        /// Draws a filled polygon using solid color
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="color"></param>
        public void DrawFilledPolygon(Point[] points, Color color)
        {
            CheckObjectDisposed();
            fixed (Point* ptr = points)
            {
                NativeWrappers.gdImageFilledPolygon(ImagePtr, ptr, points.Length, ResolveColor(color));
            }
        }

        /// <summary>
        /// Draws a filled polygon using a tile color
        /// </summary>
        /// <param name="points">array of vertixes</param>
        /// <param name="tile"></param>
        public void DrawFilledPolygon(Point[] points, Image tile)
        {
            CheckObjectDisposed();
            SetTile(tile);
            fixed (Point* ptr = points)
            {
                NativeWrappers.gdImageFilledPolygon(ImagePtr, ptr, points.Length, GdTiled);
            }
        }
        #endregion

        /// <summary>
        /// Draws a text
        /// </summary>
        /// <param name="text">text to right</param>
        /// <param name="point">The top left corner of the rectangle to write text to</param>
        /// <param name="fontName">Name of the font.
        /// Name of the font can either be a full path of a font file, or a relative path relative too ApplicationBasePath.
        /// If the font name has no extension, .ttf extension is assumed.
        /// If the file doesn't exist a <see cref="FileNotFoundException"/> is thrown.
        /// Libgd can open system fonts, but until I figure out a way to prevent libgd from crashing when an invalid font name is passed, I will limit it to font files.
        /// </param>
        /// <param name="fontSize">Font size</param>
        /// <param name="angle">angle in degrees</param>
        /// <param name="color">color of the string</param>
        /// <param name="flags">Processing flags for right to left languages such as arabic. 
        /// Default flag does no processing which will cause text in such languages to be rendered incorrectly unless parameter passed to text was already processed before calling DrawString.
        /// </param>
        public void DrawString(string text, Point point, string fontName, double fontSize, double angle, Color color, DrawStringFlags flags = DrawStringFlags.Default)
        {
            CheckObjectDisposed();
            text = FormatString(text, flags);
            var utf8 = Encoding.UTF8.GetBytes(text);
            NativeWrappers.gdImageStringFT(ImagePtr, null, ResolveColor(color), GetFont(fontName), fontSize, angle, point.X, point.Y, utf8);
        }

        /// <summary>
        /// Gets a font by font name
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        private static string GetFont(string fontName)
        {
            // TODO: Handle the case of loading system fonts without libgd crashing when an invalid font name is passed.

            if (string.IsNullOrWhiteSpace(fontName))
                throw new ArgumentNullException(nameof(fontName));
            if (!Path.IsPathRooted(fontName))
            {
                fontName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, fontName);
            }
            if (!Path.HasExtension(fontName))
            {
                fontName = Path.ChangeExtension(fontName, ".ttf");
            }
            if (!File.Exists(fontName))
                throw new FileNotFoundException("Font file was not found.", fontName);
            
            return fontName;
        }

        /// <summary>
        /// Format string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static string FormatString(string text, DrawStringFlags flags)
        {
            if ((flags & DrawStringFlags.ArabicShaping) != 0)
            {
                var options = ArabicShapingOptions.LettersShape
                              | ArabicShapingOptions.TextDirectionLogical;
                if ((flags & DrawStringFlags.ShapeArabicNumbers) != 0)
                {
                    options |= ArabicShapingOptions.DigitsEN2AN;
                }
                if ((flags & DrawStringFlags.RemoveArabicTashkeel) != 0)
                {
                    options |= ArabicShapingOptions.TashkeelResize;
                }
                var arabicShaping = new ArabicShaping(options);
                text = arabicShaping.Shape(text);
            }
            // ReSharper disable once InvertIf
            if ((flags & DrawStringFlags.RunBidi) != 0)
            {
                var dir = (flags & DrawStringFlags.IsLtr) != 0
                    ? ParagraphDirection.Left
                    : ((flags & DrawStringFlags.IsRtl) != 0 ? ParagraphDirection.Right : ParagraphDirection.Default);
                text = BidiHelper.FormatString(text, dir);
            }
            return text;
        }

        /// <summary>
        /// Converts a 31 bit signed int libgd color to 32bit color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Color GdTrueColorToColor(int color)
        {
            var alpha = unchecked(((uint)color & 0xff000000) >> 24);
            alpha = alpha == 127 ? 0 : 255 - alpha * 2;
            return new Color(unchecked(((uint)color & 0x00ffffff)) | (alpha << 24));
        }

        /// <summary>
        /// Converts 32 bit color to libgd 31 bit signed int color (note that there is a little loss in alpha resolution)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private int ResolveColor(Color color)
        {
            var res = NativeWrappers.gdImageColorResolveAlpha(ImagePtr, color.R, color.G, color.B, (255 - color.A) / 2);
            return res;
        }

        private int GetPenColor(Pen pen)
        {
            if (pen == null)
            {
                return 0;
            }
            if (pen.AntiAlias)
            {
                return GdAntiAlias;
            }
            if (pen.DashColors.Length > 1)
            {
                return GdStyled;
            }
            return ResolveColor(pen.Color);
        }
    }
}
