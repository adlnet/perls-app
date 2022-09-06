using System;
using System.Windows.Input;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model for locally-served web content.
    /// </summary>
    public class LocalItemWebViewViewModel : AuthenticatingWebViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalItemWebViewViewModel"/> class.
        /// </summary>
        /// <param name="teaser">Used to derive the web view title.</param>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="pageFailedToLoad">Page failed to load.</param>
        public LocalItemWebViewViewModel(TeaserViewModel teaser, ICommand linkClicked, ICommand pageFailedToLoad) : base(linkClicked, pageFailedToLoad, teaser?.Url?.AbsoluteUri)
        {
            if (teaser?.Name == null)
            {
                throw new ArgumentException("Teaser must include a valid name.", nameof(teaser));
            }

            Title = teaser.Name;
            Teaser = teaser;
            if (teaser.ModelItem is IShareableRemoteResource shareableItem && shareableItem.CanBeShared)
            {
                ShareRemoteResourceCommand = GetCommandFactory<ShareCommandFactory>().CreateCommand(shareableItem);
            }
        }

        /// <summary>
        /// Gets the state of the bookmark.
        /// </summary>
        /// <value>The state of the bookmark.</value>
        public BookmarkStateViewModel BookmarkState => Teaser.BookmarkState;

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model of the view.</value>
        public TeaserViewModel Teaser { get; }

        /// <summary>
        /// Gets the icon for sharing.
        /// </summary>
        /// <value>The icon for sharing.</value>
        public string ShareIcon => "shareResourceIcon";

        /// <summary>
        /// Gets or sets the URL for this web view.
        /// This proxies to the base view model's InitialUrl property.
        /// </summary>
        /// <value>The current URL.</value>
        public Uri Url
        {
            get => InitialUrl;
            set => InitialUrl = value;
        }

        /// <summary>
        /// Gets the command to share a shareable remote resource.
        /// </summary>
        /// <value>The command to share a shareable remote resource.</value>
        public ICommand ShareRemoteResourceCommand { get; }

        /// <inheritdoc />
        /// <remarks>We override authentication methods provided by the base view model as local web content never needs authentication.</remarks>
        protected override bool RequiresAuthentication => false;

        /// <inheritdoc />
        /// <remarks>While local content is "internal" to the project, it is "external" to the PERLS server.</remarks>
        protected override bool IsDestinationExternal => true;
    }
}
