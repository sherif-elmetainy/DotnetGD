using System;

namespace CodeArt.DotnetGD.Formatters
{
    public interface IAnimationContext : IDisposable
    {
        void AddImage(Image image, bool localColorMap, int leftOffset, int topOffset, int delay, int disposable);

        void EndAnimation();
    }
}
