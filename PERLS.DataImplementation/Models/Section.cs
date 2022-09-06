using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A section of content to display in the app (usually in the dashboard).
    /// </summary>
    [Serializable]
    public class Section : ISection, IEquatable<ISection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="layout">The layout that should be used for presenting this section's blocks.</param>
        /// <param name="blocks">The blocks within this section.</param>
        public Section(SectionLayout layout, List<Block> blocks)
        {
            this.Layout = layout;
            this.JsonBlocks = blocks ?? throw new ArgumentNullException(nameof(blocks));
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public SectionLayout Layout { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<IBlock> Blocks => JsonBlocks;

        /// <summary>
        /// Gets or sets the backing property for blocks.
        /// </summary>
        [JsonProperty("blocks")]
        internal List<Block> JsonBlocks { get; set; }

        /// <summary>
        /// Determines if two sections are equivalent using value equality.
        /// </summary>
        /// <param name="left">The first section to compare.</param>
        /// <param name="right">The second section to compare.</param>
        /// <returns><c>true</c> if the sections are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(Section left, Section right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines if two sections are equivalent using value equality.
        /// </summary>
        /// <param name="left">The first section to compare.</param>
        /// <param name="right">The second section to compare.</param>
        /// <returns><c>true</c> if the section are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(Section left, Section right)
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
            return obj is ISection section && Equals(section);
        }

        /// <inheritdoc />
        public bool Equals(ISection other)
        {
            // we compare using hash codes here because C# doesn't support value equality for lists
            // generally speaking, you should not replicate this implementation elsewhere
            return GetHashCode() == other?.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = -713272358;
            hashCode = (hashCode * -1521134295) + Layout.GetHashCode();

            // as noted above, this method allows us to generate stable hash codes for sections with the same contents
            // the default EqualityComparer for lists and arrays always seems to use reference equality
            // there may be a better way to do this via a HashSet or some other kind of collection
            foreach (var block in JsonBlocks.OrderBy(blk => blk.Name))
            {
                hashCode = (hashCode * -1521134295) + block.GetHashCode();
            }

            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"<Section: Layout={Layout}, Blocks={string.Join("|", Blocks.Select(blk => blk.Name))}, Hash={GetHashCode()}>";
        }
    }
}
