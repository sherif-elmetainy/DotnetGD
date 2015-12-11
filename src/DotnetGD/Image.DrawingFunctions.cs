﻿using System;
using System.Text;
using DotnetGD.Libgd;

namespace DotnetGD
{
    public unsafe partial class Image
    {
        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x), x, "Value outside image bounds.");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Value outside image bounds.");
            CheckObjectDisposed();
            var color = NativeWrappers.gdImageGetTrueColorPixel(ImagePtr, x, y);
            return GdTrueColorToColor(color);
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x), x, "Value outside image bounds.");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Value outside image bounds.");
            CheckObjectDisposed();
            var colorIndex = ResolveColor(color);
            NativeWrappers.gdImageSetPixel(ImagePtr, x, y, colorIndex);
        }

        #region DrawLine overloads
        public void DrawLine(Point p1, Point p2, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageLine(ImagePtr, p1.X, p1.Y, p2.X, p2.Y, resolvedColor);
        }

        public void DrawLine(Point p1, Point p2, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            NativeWrappers.gdImageLine(ImagePtr, p1.X, p1.Y, p2.X, p2.Y, GdStyled);
        }
        public void DrawLine(Point p1, Point p2, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            NativeWrappers.gdImageLine(ImagePtr, p1.X, p1.Y, p2.X, p2.Y, GdBrushed);
        }
        #endregion

        #region DrawRecangle overloads
        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, resolvedColor);
        }

        public void DrawRectangle(Rectangle rectangle, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            NativeWrappers.gdImageRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, GdStyled);
        }

        public void DrawRectangle(Rectangle rectangle, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            NativeWrappers.gdImageRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, GdBrushed);
        }
        #endregion

        #region DrawFilledRectangle overloads
        public void DrawFilledRectangle(Rectangle rectangle, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageFilledRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, resolvedColor);
        }

        public void DrawFilledRectangle(Rectangle rectangle, Image tile)
        {
            CheckObjectDisposed();
            SetTile(tile);
            NativeWrappers.gdImageFilledRectangle(ImagePtr, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, GdTiled);
        }
        #endregion DrawFilledRectangle overloads

        #region DrawEllipse overloads
        public void DrawEllipse(Point center, Size size, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, resolvedColor);
        }

        public void DrawEllipse(Point center, Size size, Pen pen)
        {
            CheckObjectDisposed();
            SetPen(pen);
            NativeWrappers.gdImageEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, GdStyled);
        }

        public void DrawEllipse(Point center, Size size, Image brush)
        {
            CheckObjectDisposed();
            SetBrush(brush);
            NativeWrappers.gdImageEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, GdBrushed);
        }
        #endregion

        #region DrawFilledEllipse overloads
        public void DrawFilledEllipse(Point center, Size size, Color color)
        {
            CheckObjectDisposed();
            var resolvedColor = ResolveColor(color);
            NativeWrappers.gdImageFilledEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, resolvedColor);
        }

        public void DrawFilledEllipse(Point center, Size size, Image tile)
        {
            CheckObjectDisposed();
            SetTile(tile);
            NativeWrappers.gdImageFilledEllipse(ImagePtr, center.X, center.Y, size.Width, size.Height, GdTiled);
        }
        #endregion

        public void DrawString(string text, Point point, string fontName, double fontSize, double angle, Color color)
        {
            var utf8 = Encoding.UTF8.GetBytes(text);
            NativeWrappers.gdImageStringFT(ImagePtr, null, ResolveColor(color), fontName, fontSize, angle, point.X, point.Y, utf8);
        }

        private static Color GdTrueColorToColor(int color)
        {
            var alpha = unchecked(((uint)color & 0xff000000) >> 24);
            alpha = alpha == 127 ? 0 : 255 - alpha * 2;
            return new Color(unchecked(((uint)color & 0x00ffffff)) | (alpha << 24));
        }

        private int ResolveColor(Color color)
        {
            SetPen(null);
            var res = NativeWrappers.gdImageColorResolveAlpha(ImagePtr, color.R, color.G, color.B, (255 - color.A) / 2);
            return res;
        }
    }
}
