using System;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The Block Contents interface.
    /// </summary>
    public interface IBlockContents
    {
        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        Uri Icon { get; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        string Text { get; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <value>
        /// The url.
        /// </value>
        Uri Url { get; }
    }
}
