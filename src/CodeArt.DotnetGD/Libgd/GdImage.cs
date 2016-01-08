using System;

namespace CodeArt.DotnetGD.Libgd
{
    /// <summary>
    /// libgd image structure
    /// </summary>
    public unsafe struct GdImage
    {
        /// <summary>
        /// repersents 4 integers.
        /// LibGD Image structure contains int XXXX[256] members
        /// Using int[] and marshal as size const in .NET will make it impossible to obtain a pointer to GdImage,
        /// So this hack is implemented
        /// </summary>
        private struct Colors4
        {
            private int _a1;
            private int _a2;
            private int _a3;
            private int _a4;

            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 1:
                            return _a1;
                        case 2:
                            return _a2;
                        case 3:
                            return _a3;
                        case 4:
                            return _a4;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 1:
                            _a1 = value;
                            break;
                        case 2:
                            _a2 = value;
                            break;
                        case 3:
                            _a3 = value;
                            break;
                        case 4:
                            _a4 = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// repersents 16 integers.
        /// LibGD Image structure contains int XXXX[256] members
        /// Using int[] and marshal as size const in .NET will make it impossible to obtain a pointer to GdImage,
        /// So this hack is implemented
        /// </summary>
        private struct Colors16
        {
            private Colors4 _a1;
            private Colors4 _a2;
            private Colors4 _a3;
            private Colors4 _a4;

            public int this[int index]
            {
                get
                {
                    var i1 = index & 0x3;
                    var i2 = (index & 0xffffffffc) >> 2;

                    switch (i2)
                    {
                        case 1:
                            return _a1[i1];
                        case 2:
                            return _a2[i1];
                        case 3:
                            return _a3[i1];
                        case 4:
                            return _a4[i1];
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    var i1 = index & 0x3;
                    var i2 = index & 0xfffffffc0;

                    switch (i2)
                    {
                        case 1:
                            _a1[i1] = value;
                            break;
                        case 2:
                            _a2[i1] = value;
                            break;
                        case 3:
                            _a3[i1] = value;
                            break;
                        case 4:
                            _a4[i1] = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// repersents 64 integers.
        /// LibGD Image structure contains int XXXX[256] members
        /// Using int[] and marshal as size const in .NET will make it impossible to obtain a pointer to GdImage,
        /// So this hack is implemented
        /// </summary>
        private struct Colors64
        {
            private Colors16 _a1;
            private Colors16 _a2;
            private Colors16 _a3;
            private Colors16 _a4;

            public int this[int index]
            {
                get
                {
                    var i1 = index & 0xf;
                    var i2 = (index & 0xfffffff30) >> 4;

                    switch (i2)
                    {
                        case 1:
                            return _a1[i1];
                        case 2:
                            return _a2[i1];
                        case 3:
                            return _a3[i1];
                        case 4:
                            return _a4[i1];
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    var i1 = index & 0xf;
                    var i2 = (index & 0xfffffff30) >> 4;

                    switch (i2)
                    {
                        case 1:
                            _a1[i1] = value;
                            break;
                        case 2:
                            _a2[i1] = value;
                            break;
                        case 3:
                            _a3[i1] = value;
                            break;
                        case 4:
                            _a4[i1] = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// repersents 256 integers.
        /// LibGD Image structure contains int XXXX[256] members
        /// Using int[] and marshal as size const in .NET will make it impossible to obtain a pointer to GdImage,
        /// So this hack is implemented
        /// </summary>
        public struct Colors256
        {
            private Colors64 _a1;
            private Colors64 _a2;
            private Colors64 _a3;
            private Colors64 _a4;

            public int this[int index]
            {
                get
                {
                    var i1 = index & 0x3f;
                    var i2 = (index & 0xfffffffc0) >> 6;

                    switch (i2)
                    {
                        case 1:
                            return _a1[i1];
                        case 2:
                            return _a2[i1];
                        case 3:
                            return _a3[i1];
                        case 4:
                            return _a4[i1];
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    var i1 = index & 0x3f;
                    var i2 = (index & 0xfffffffc0) >> 6;

                    switch (i2)
                    {
                        case 1:
                            _a1[i1] = value;
                            break;
                        case 2:
                            _a2[i1] = value;
                            break;
                        case 3:
                            _a3[i1] = value;
                            break;
                        case 4:
                            _a4[i1] = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }
        public byte** Pixels;
        public int Width;
        public int Height;
        /* These are valid in palette images only. See also
           'alpha', which appears later in the structure to
           preserve binary backwards compatibility */
        public int ColorsTotal;
        public Colors256 Red;
        public Colors256 Green;
        public Colors256 Blue;
        public Colors256 Open;
        /* For backwards compatibility, this is set to the
           first palette entry with 100% transparency,
           and is also set and reset by the
           gdImageColorTransparent function. Newer
           applications can allocate palette entries
           with any desired level of transparency; however,
           bear in mind that many viewers, notably
           many web browsers, fail to implement
           full alpha channel for PNG and provide
           support for full opacity or transparency only. */
        public int Transparent;
        public int* PolyInts;
        public int PolyAllocated;
        public GdImage* Brush;
        public GdImage* Tile;
        public Colors256 BrushColorMap;
        public Colors256 TileColorMap;
        public int StyleLength;
        public int StylePos;
        public int* Style;
        public int Interlace;
        /* New in 2.0: thickness of line. Initialized to 1. */
        public int Thick;
        /* New in 2.0: alpha channel for palettes. Note that only
           Macintosh Internet Explorer and (possibly) Netscape 6
           really support multiple levels of transparency in
           palettes, to my knowledge, as of 2/15/01. Most
           common browsers will display 100% opaque and
           100% Transparent correctly, and do something
           unpredictable and/or undesirable for levels
           in between. TBB */
        public Colors256 Alpha;
        /* Truecolor flag and Pixels. New 2.0 fields appear here at the
           end to minimize breakage of existing object code. */
        public int TrueColor;
        public int** TruecolorPixels;
        /* Should alpha channel be copied, or applied, each time a
           pixel is drawn? This applies to truecolor images only.
           No attempt is made to alpha-blend in palette images,
           even if semitransparent palette entries exist.
           To do that, build your image as a truecolor image,
           then quantize down to 8 bits. */
        public DrawingEffect AlphaBlendingFlag;
        /* Should the alpha channel of the image be saved? This affects
           PNG at the moment; other future formats may also
           have that capability. JPEG doesn't. */
        public int SaveAlphaFlag;

        /* There should NEVER BE ACCESSOR MACROS FOR ITEMS BELOW HERE, so this
           part of the structure can be safely changed in new releases. */

        /* 2.0.12: anti-aliased globals. 2.0.26: just a few vestiges after
          switching to the fast, memory-cheap implementation from PHP-gd. */
        public int AA;
        public int AAColor;
        public int AADontBlend;

        /* 2.0.12: simple clipping rectangle. These values
          must be checked for safety when set; please use
          gdImageSetClip */
        public int ClipX1;
        public int ClipY1;
        public int ClipX2;
        public int ClipY2;

        /* 2.1.0: allows to specify resolution in dpi */
        public uint ResolutionX;
        public uint ResolutionY;

        /* Selects quantization method, see gdImageTrueColorToPaletteSetMethod() and gdPaletteQuantizationMethod enum. */
        public PaletteQuantizationMethod PaletteQuantizationMethod;
        /* speed/quality trade-off. 1 = best quality, 10 = best speed. 0 = method-specific default.
           Applicable to GD_QUANT_LIQ and GD_QUANT_NEUQUANT. */
        public int PaletteQuantizationSpeed;
        /* Image will remain true-color if conversion to palette cannot achieve given quality.
           Value from 1 to 100, 1 = ugly, 100 = perfect. Applicable to GD_QUANT_LIQ.*/
        public int PaletteQuantizationMinQuality;
        /* Image will use minimum number of palette colors needed to achieve given quality. Must be higher than paletteQuantizationMinQuality
           Value from 1 to 100, 1 = ugly, 100 = perfect. Applicable to GD_QUANT_LIQ.*/
        public int PaletteQuantizationMaxQuality;

        public InterpolationMethod InterpolationId;
#pragma warning disable 169
        private IntPtr _interpolation;
#pragma warning restore 169
    }
}
