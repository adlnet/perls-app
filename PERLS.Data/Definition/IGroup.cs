using System.ComponentModel;
using Float.Core.Definitions;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The different types of groups.
    /// </summary>
    public enum IGroupVisibility
    {
        /// <summary>
        /// Groups that are visible to all users.
        /// </summary>
        Public,

        /// <summary>
        /// Groups that are only visible to members.
        /// </summary>
        Private,
    }

    /// <summary>
    /// Any group from the server.
    /// </summary>
    public interface IGroup : INamed, IRemoteResource, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the group ID for this group.
        /// </summary>
        /// <value>The group id.</value>
        int Gid { get; }

        /// <summary>
        /// Gets the description for this group.
        /// </summary>
        /// <value>The group description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the image associated with the group.
        /// </summary>
        /// <value>The image associated with the group.</value>
        IFile Image { get; }

        /// <summary>
        /// Gets the visibility of the group.
        /// </summary>
        /// <value>The group visibility.</value>
        IGroupVisibility Visibility { get; }
    }
}
