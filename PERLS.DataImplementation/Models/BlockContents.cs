using System;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The block contents.
    /// </summary>
    [Serializable]
    public class BlockContents : IBlockContents, IEquatable<IBlockContents>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockContents"/> class.
        /// </summary>
        /// <param name="icon">The icon to show in this block's content.</param>
        /// <param name="text">The text to show in this block's content.</param>
        /// <param name="url">The URL to use when acting on this block.</param>
        public BlockContents(Uri icon, string text, Uri url)
        {
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <inheritdoc />
        public Uri Icon { get; internal set; }

        /// <inheritdoc />
        public string Text { get; internal set; }

        /// <inheritdoc />
        public Uri Url { get; internal set; }

        /// <summary>
        /// Determines if two block contents are not equivalent using value equality.
        /// </summary>
        /// <param name="left">The first block contents to compare.</param>
        /// <param name="right">The second block contents to compare.</param>
        /// <returns><c>true</c> if the block contents are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(BlockContents left, BlockContents right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines if two block contents are equivalent using value equality.
        /// </summary>
        /// <param name="left">The first block contents to compare.</param>
        /// <param name="right">The second block contents to compare.</param>
        /// <returns><c>true</c> if the block contents are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(BlockContents left, BlockContents right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is IBlockContents other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(IBlockContents other)
        {
            return Icon == other?.Icon
                && Text == other?.Text
                && Url == other?.Url;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Icon, Text, Url).GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"<BlockContents: Icon={Icon}, Text={Text}, Url={Url}>";
        }
    }
}
