using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// An enumeration of sources that the action to download or delete content may stem from.
    /// </summary>
    [Serializable]
    public enum Initiator
    {
        /// <summary>
        /// This action was initiated by the user.
        /// </summary>
        User,

        /// <summary>
        /// This action was initiated by the application.
        /// </summary>
        Application,

        /// <summary>
        /// No record of this action's initiation exists.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// Defines a service that handles changes to recommended content.
    /// </summary>
    public interface IOfflineContentService
    {
        /// <summary>
        /// Notify the offline content service that recommendations are available.
        /// Implementers are responsible for determining which content should be downloaded, if any.
        /// </summary>
        /// <param name="items">The new recommended items.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateRecommendedItems(IEnumerable<IItem> items);

        /// <summary>
        /// Mark a piece of content as originating from a certain action.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="contentOrigin">The origin of this piece of content.</param>
        void SetContentOrigin(IItem item, Initiator contentOrigin);

        /// <summary>
        /// Determine the originating action for a piece of content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The origin of this piece of content.</returns>
        Initiator GetContentOrigin(IItem item);

        /// <summary>
        /// Mark a piece of content as removed from a certain action.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="removalOrigin">The reason the content was removed.</param>
        void SetRemovalOrigin(IItem item, Initiator removalOrigin);

        /// <summary>
        /// Determine the originating action for deleting a piece of content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The reason the content was remove.</returns>
        Initiator GetRemovalOrigin(IItem item);

        /// <summary>
        /// Clears all caches managed by the offlin content service.
        /// </summary>
        void ClearCaches();
    }
}
