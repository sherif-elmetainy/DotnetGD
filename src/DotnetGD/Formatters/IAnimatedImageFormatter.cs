using System.IO;

namespace DotnetGD.Formatters
{
    public interface IAnimatedImageFormatter
    {
        IAnimationContext BeginAnimation(Image image, Stream outStream, bool globalColorMap, int loops);
    }
}
