using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// View model for a group of <see cref="GroupTeaserGroupViewModel"/>.
    /// </summary>
    public class GroupTeaserGroupViewModel : RefreshableBaseCollectionViewModel<IGroup, GroupTeaserViewModel>, INamedVariableItemViewModel<GroupTeaserViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupTeaserGroupViewModel"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskAction">A function to execute to refresh the list of groups.</param>
        /// <param name="title">The title of the group teaser group.</param>
        /// <param name="subtitle">The subtitle of the group teaser group.</param>
        /// <param name="viewHeight">The view height of the group teaser group.</param>
        /// <param name="isJoinable">Whether to show the join image on the group tile.</param>
        public GroupTeaserGroupViewModel(Func<Task<IEnumerable<IGroup>>> modelCollectionTaskAction, string title = null, string subtitle = null, int viewHeight = 300, bool isJoinable = false) : base(modelCollectionTaskAction)
        {
            ViewHeight = viewHeight;
            Subtitle = subtitle;
            Name = title;
            IsJoinable = isJoinable;
        }

        /// <summary>
        /// Gets a name for this group of teasers.
        /// </summary>
        /// <value>The group name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets a subtitle for this group of teasers.
        /// </summary>
        /// <value>The group subtitle.</value>
        public string Subtitle { get; }

        /// <summary>
        /// Gets the height for the groups tile view.
        /// </summary>
        /// <value>The height for the groups tile view.</value>
        public int ViewHeight { get; }

        /// <summary>
        /// Gets a value indicating whether the join image should be shown.
        /// </summary>
        /// <value>A bool value.</value>
        public bool IsJoinable { get; }

        /// <inheritdoc />
        public string EmptyLabel => string.Format(CultureInfo.InvariantCulture, StringsSpecific.EmptyGroupMessage, Name);

        /// <inheritdoc/>
        public string EmptyMessageTitle => string.Empty;

        /// <inheritdoc/>
        public string EmptyImageName => "error";

        /// <inheritdoc/>
        protected override GroupTeaserViewModel ConvertModelToViewModel(IGroup model)
        {
            return new GroupTeaserViewModel(model, IsJoinable);
        }
    }
}
