using System.Windows.Input;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Item web view view model.
    /// </summary>
    public class ItemWebViewViewModel : AuthenticatingWebViewViewModel
    {
        readonly TeaserViewModel teaserViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemWebViewViewModel"/> class.
        /// </summary>
        /// <param name="teaser">Teaser.</param>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="pageFailedToLoad">Page failed to load.</param>
        public ItemWebViewViewModel(TeaserViewModel teaser, ICommand linkClicked, ICommand pageFailedToLoad) : base(linkClicked, pageFailedToLoad, teaser?.Url?.OriginalString)
        {
            Title = teaser.Name;
            teaserViewModel = teaser;
            ShareRemoteResourceCommand = GetCommandFactory<ShareCommandFactory>().CreateCommand(teaser.ModelItem as IShareableRemoteResource);
        }

        /// <summary>
        /// Gets the state of the bookmark.
        /// </summary>
        /// <value>The state of the bookmark.</value>
        public BookmarkStateViewModel BookmarkState => teaserViewModel.BookmarkState;

        /// <summary>
        /// Gets the command to share a shareable remote resource.
        /// </summary>
        /// <value>The command to share a shareable remote resource.</value>
        public ICommand ShareRemoteResourceCommand { get; }

        /// <summary>
        /// Gets the icon for sharing.
        /// </summary>
        /// <value>The icon for sharing.</value>
        public string ShareIcon => "shareResourceIcon";
    }
}
