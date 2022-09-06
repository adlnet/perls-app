using System.Collections.Generic;
using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A section of content, which contains one or more blocks.
    /// </summary>
    public interface ISection : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the layout that should be used for presenting this section's blocks.
        /// </summary>
        /// <value>The desired section layout.</value>
        SectionLayout Layout { get; }

        /// <summary>
        /// Gets the blocks within this section.
        /// </summary>
        /// <value>An enumerable of blocks.</value>
        IEnumerable<IBlock> Blocks { get; }
    }
}
