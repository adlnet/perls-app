using System;
using System.Linq;
using System.Windows.Input;
using Float.Core.Collections;
using Float.Core.ViewModels;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A group of teasers to display in a list.
    /// </summary>
    public class TeaserGroupViewModel : BaseCollectionViewModel<IItem, TeaserViewModel>, INamedVariableItemViewModel<TeaserViewModel>
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;
        readonly IItemGroup group;
        bool showsViewMore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeaserGroupViewModel"/> class.
        /// </summary>
        /// <param name="group">A group of items.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public TeaserGroupViewModel(IItemGroup group, IAsyncCommand<IItem> downloadContentCommand) : base(new ObservableElementCollection<IItem>())
        {
            this.group = group ?? throw new ArgumentNullException(nameof(group));
            this.downloadContentCommand = downloadContentCommand ?? throw new ArgumentNullException(nameof(downloadContentCommand));
            ViewMoreCommand = new Command(HandleViewMoreSelected);

            (Models as ObservableElementCollection<IItem>).AddRange(group?.Items);
        }

        /// <summary>
        /// Gets a name for this group of teasers.
        /// </summary>
        /// <value>The group name.</value>
        public string Name => group.Name;

        /// <summary>
        /// Gets an associated url for this group of teasers.
        /// </summary>
        /// <value>The associated group url.</value>
        public Uri Url => group.Url;

        /// <summary>
        /// Gets or sets a value indicating whether the "View more" button should be shown.
        /// </summary>
        /// <value><c>True</c> if the "View more" button should be shown.</value>
        /// <remarks>
        /// The "View more" button will only be shown if _both_ this property is set to <c>true</c>
        /// AND if the collection had more content to show (<see cref="HasMore"/>).
        /// </remarks>
        public bool ShowsViewMore
        {
            get => showsViewMore;
            set => SetField(ref showsViewMore, value);
        }

        /// <summary>
        /// Gets a value indicating whether there is more content in this group than what is contained in <see cref="IItemGroup.Items"/>.
        /// </summary>
        /// <value><c>true</c> if there is more content than what was included in the summary.</value>
        /// <remarks>
        /// Ideally, the server should be telling us whether there is more content.
        /// But for now, we can just assume that if there are 10 item, then there is probably more.
        /// The only scenario this won't work in is if there were exactly 10 items.
        /// </remarks>
        public bool HasMore => showsViewMore && group.Items.Count() >= 10;

        /// <summary>
        /// Gets a value indicating whether the teaser group view model is empty.
        /// </summary>
        /// <value><c>true</c> if empty.</value>
        public bool IsEmpty => !group.Items.Any();

        /// <summary>
        /// Gets a value indicating whether this view model is loading.
        /// </summary>
        /// <value><c>true</c> if loading, <c>false</c> otherwise.</value>
        /// <remarks>
        /// Since the contents are provided as part of the constructor, this is never loading.
        /// </remarks>
        public bool IsLoading => false;

        /// <summary>
        /// Gets the command to invoke when the user taps the button to view more.
        /// </summary>
        /// <value>The navigation command.</value>
        public ICommand ViewMoreCommand { get; }

        /// <inheritdoc />
        public string EmptyLabel { get; set; } = Strings.DefaultEmptyMessage;

        /// <inheritdoc/>
        public string EmptyMessageTitle { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string EmptyImageName { get; set; } = "error";

        /// <inheritdoc />
        protected override TeaserViewModel ConvertModelToViewModel(IItem model)
        {
            return new TeaserViewModel(model, downloadContentCommand);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(ShowsViewMore))
            {
                OnPropertyChanged(nameof(HasMore));
            }
        }

        void HandleViewMoreSelected(object obj)
        {
            // We could have set `ViewMoreCommand` directly to `ItemSelected`,
            // but I decided to "proxy" the command so that I could pass the model
            // instead of the view model.
            // While this is currently inconsistent with what is used elsewhere,
            // I intend to eventually update everything else to base navigation
            // based on models and not view models.
            DependencyService.Get<INavigationCommandProvider>().ItemSelected?.Execute(group);
        }
    }
}
