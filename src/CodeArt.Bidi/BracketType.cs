// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.Bidi
{
    /// <summary>
    /// Bracket type
    /// </summary>
    public enum BracketType
    {
        /// <summary>
        /// Not a bracket 
        /// </summary>
        None = 0,
        /// <summary>
        /// Opening bracket such as (, [, {, etc
        /// </summary>
        Opening = 1,
        /// <summary>
        /// Closing bracket such as ), ], }, etc
        /// </summary>
        Closing = 2,
    }
}