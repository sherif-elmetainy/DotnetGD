using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Enumeration for arc styles
    /// </summary>
    [Flags]
    public enum ArcStyles
    {
        /// <summary>
        /// Draws an arc
        /// </summary>
        Arc = 0,
        /// <summary>
        /// Draws an arc (Same as pie)
        /// </summary>
        Pie = 0,
        /// <summary>
        /// Draws a chord (mutually exclusive with Arc or Pie). The chord is the line connection the 2 points in the ellies.
        /// </summary>
        Chord = 1,
        /// <summary>
        /// Don't fill
        /// </summary>
        NoFill = 2,
        /// <summary>
        /// When used with no fill, indicates that the start and end points should be connected to the center.
        /// </summary>
        Edged = 4
    }
}
