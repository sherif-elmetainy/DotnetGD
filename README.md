# CodeArt.DotnetGD

## About
Dotnet GD is an open source managed wrapper around [libgd](https://github.com/libgd/libgd) designed to work with ASP.NET 5 using either .NET Framework 4.51 (DNX451) or .NET Core 5.0 (DNXCORE50). The library is still a work in progress and a lot of functions supported by libgd are still not exposed. It is intended as a replacement for System.Drawing for projects targetting ASP.NET 5.

## The currently implemented features are:

* Creating true color (32-bits per pixel ARGB) image
* Creating 256-color pallete based images.
* Reading and writing of GIF, JPEG, PNG, TIFF, BMP and WBMP formats.
* Writing animated gif files.
* Drawing Lines, Ellipses and Rectangles using solid colors, tiles or brushes (see libgd for more info)
* Drawing Strings including some support for handle RTL languages and Arabic language shaping (this is not handled by libgd itself, but by CodeArt.Bidi library included in this repository.)
* Image cropping, resizing, resampling, etc.

## TODO List include:
* Implementing the rest of function supported by libgd (Drawing, conversion, resizing, alpha blend, AA, etc)
* Better handling for fonts. Currently font names are passed using is a file name relative to the application base folder. The extension .ttf is added if the name has no extension. And a check is done to verify that font file exists. This is done because passing an invalid font name causes libgd to crash the process.
* Extension methods to provide some overloads for methods (for convenience when using the library).
* More unit tests and bug fixing.
* Mac support.
* Documentation!


The library is tested to work in DNX451 and DNXCORE50 on Windows 32-bit and 64-bit and on DNXCORE50 Ubuntu 64-bit. I still have not built libgd for Mac and I don't have a Mac machine to test it. The library include native libgd built for Windows 32 and 64-bit versions and Ubuntu 64-bit. For MAC and other Linux distributions, libgd need to be built for that OS and included in runtimes/--OSMoniker--/native/. When I get my hands on a MAC machine, I will build libgd for MAC, but it's not practical to build libgd for all Linux distributions.
