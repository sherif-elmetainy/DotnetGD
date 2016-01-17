# CodeArt.DotnetGD

## About
Dotnet GD is an open source managed wrapper around [libgd](https://github.com/libgd/libgd) designed to work with ASP.NET 5 using either .NET Framework 4.51 (DNX451) or .NET Core 5.0 (DNXCORE50). The library is still a work in progress and a lot of functions supported by [libgd](https://github.com/libgd/libgd) are still not exposed. It is intended as a replacement for ```System.Drawing``` for projects targetting ASP.NET 5.

## Using the library:
 * Install ```CodeArt.DotnetGD``` nuget package by adding it to your dependencies in project.json. Current version is 1.0.0-alpha1, or download the source and include the source project in your solution, and add a reference to it to allow debugging.
 * For Windows, the binaries for [libgd](https://github.com/libgd/libgd) are included in the nuget package, and are used automatically. For OSX and Linux you need to download and compile [libgd](https://github.com/libgd/libgd)'s source code. The repository contains a binary build on Ubuntu x64, but since there are many Linux flavours and distribution, I chose not to include the Linux binaries in the package. I plan to include the OSX binaries in the future, for now you can download and build it.
 * Use the ```Image``` class in ```CodeArt.DotnetGD``` namespace to for drawing and image processing functions.
 * Use the ```ImageFormatter``` class in ```CodeArt.DotnetGD.Formatters``` namespace to read and write images. This is a wrapper to simplify usage of difference image format classes such as ```PngImageFormatter```, ```JpegImageFormatter```, etc.
 * The library has xml docs covering a lot of the functionality, but still more work needs to be done to make it more complete.
 * You can also refer to the Unit test project for different examples of usage.
 * The ```CodeArt.Bidi``` package is used for handling text in right to left languages like Arabic. Currently the ```CodeArt.DotnetGD``` package depends on it, but I plan to decouple them so that the ```CodeArt.Bidi``` library would be only downloaded and referenced if needed.
 
## Example usage of ```Image``` class and ```ImageFormatter``` class
```csharp
// draw a black circle in white square and save image as png
using (var image1 = new Image(20, 20))
{
    image1.DrawFilledRectangle(image1.Bounds, Color.White);
    image1.DrawFilledEllipse(new Point(10, 10), new Size(18, 18), Color.Black);
    ImageFormatter.WriteImageToFile(image1, "test.png");
}

// resize a png image and save the result as jpeg
using (var image2 = ImageFormatter.ReadImageFromFile("test.png"))
{
    using (var image3 = image2.Resize(image2.Size*2))
    {
        ImageFormatter.WriteImageToFile(image3, "test.jpg");
    }
}
```

## The currently implemented features are:

* Creating true color (32-bits per pixel ARGB) image
* Creating 256-color pallete based images.
* Reading and writing of GIF, JPEG, PNG, TIFF, BMP and WBMP formats.
* Writing animated gif files.
* Drawing Lines, Ellipses and Rectangles using solid colors, tiles or brushes (see libgd for more info)
* Drawing Strings including some support for handle RTL languages and Arabic language shaping (this is not handled by libgd itself, but by ```CodeArt.Bidi``` library included in this repository.)
* Image cropping, resizing, resampling, etc.
* 2D matrix transformations
* Various image processing functions such as grayscale, negate, pixelate, scatter, etc.


## TODO List include:
* Better handling for fonts. Currently font names are passed using is a file name relative to the application base folder. The extension .ttf is added if the name has no extension. And a check is done to verify that font file exists. This is done because passing an invalid font name causes libgd to crash the process.
* Extension methods to provide some overloads for methods (for convenience when using the library).
* More unit tests and bug fixing.
* More drawing functions not implemented by libgd (DrawBezier, DrawCurve)
* Measure string function
* Mac testing and support.
* Documentation!

## Known Issues:
* While all the unit tests current pass on Windows and Ubuntu, I discovered that when the system is low on memory, there are random crashes. This is most likely an issue with [libgd](https://github.com/libgd/libgd) or one of its dependencies not handling memory allocation failure correctly (with libgd rather than its dependencies being the most likely reason).


The library is tested to work in DNX451 and DNXCORE50 on Windows 32-bit and 64-bit and on DNXCORE50 Ubuntu 64-bit. 

I still have not built [libgd](https://github.com/libgd/libgd) for Mac and I don't have a Mac machine to test it. The library include native libgd built for Windows 32 and 64-bit versions and Ubuntu 64-bit. For MAC and other Linux distributions, libgd need to be built for that OS and included in runtimes/--OSMoniker--/native/. When I get my hands on a MAC machine, I will build libgd for MAC, but it's not practical to build libgd for all Linux distributions.