using System;
using System.ComponentModel;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A group the user can join or leave.
    /// </summary>
    [Serializable]
    public class Group : DrupalEntity, IGroup
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public string Description { get; internal set; }

        /// <inheritdoc/>
        public int Gid { get; internal set; }

        /// <summary>
        /// Gets the image of the group.
        /// </summary>
        /// <value>The image of the group.</value>
        public File Image { get; internal set; }

        /// <inheritdoc/>
        public IGroupVisibility Visibility { get; internal set; }

        /// <inheritdoc/>
        IFile IGroup.Image => Image;
    }
}
