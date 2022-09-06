using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// A page containing podcast information and a list of episodes.
    /// </summary>
    public partial class PodcastDetailPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastDetailPage"/> class.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        public PodcastDetailPage(EpisodeContentListPageViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Collection.SelectedItem = null;
            if (BindingContext is EpisodeContentListPageViewModel listPageViewModel)
            {
                listPageViewModel.Refresh();
            }
        }
    }
}
