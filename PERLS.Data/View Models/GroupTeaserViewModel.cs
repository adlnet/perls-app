using System;
using Float.Core.Definitions;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Teaser view model for groups.
    /// </summary>
    public class GroupTeaserViewModel : Float.Core.ViewModels.ViewModel<IGroup>, INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupTeaserViewModel"/> class.
        /// </summary>
        /// <param name="model">The group to represent.</param>
        /// <param name="isJoinable">Whether the join image should be shown.</param>
        public GroupTeaserViewModel(IGroup model, bool isJoinable) : base(model)
        {
            IsJoinable = isJoinable;
        }

        /// <summary>
        /// Gets the name of the item being represented.
        /// </summary>
        /// <value>The item name.</value>
        public string Name => Model.Name;

        /// <summary>
        /// Gets a description for the item.
        /// </summary>
        /// <remarks>
        /// This is displayed with the item when it appears in a list to help give more context.
        /// </remarks>
        /// <value>The item subtitle.</value>
        public string Description => Model.Description;

        /// <summary>
        /// Gets a value indicating whether the join image should be shown.
        /// </summary>
        /// <value>A bool value.</value>
        public bool IsJoinable { get; }

        /// <summary>
        /// Gets the image Url.
        /// </summary>
        /// <value>The image source.</value>
        public ImageSource Image
        {
            get
            {
                if (Model?.Image?.Url is Uri uri)
                {
                    return ImageSource.FromUri(uri);
                }

                return ImageSource.FromFile("placeholder");
            }
        }

        /// <summary>
        /// Gets the join group image.
        /// </summary>
        /// <value>The join group image.</value>
        public ImageSource JoinImage => ImageSource.FromFile("icon_add_group");

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        /// <remarks>TODO: This should be internal.</remarks>
        public IGroup ModelItem => Model;
    }
}
