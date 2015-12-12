using System.IO;

namespace CodeArt.DotnetGD.Formatters
{
    public interface IAnimatedImageFormatter
    {
        IAnimationContext BeginAnimation(Image image, Stream outStream, bool globalColorMap, int loops);
    }
}
