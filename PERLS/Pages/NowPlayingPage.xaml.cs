using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The now playing page.
    /// </summary>
    public partial class NowPlayingPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingPage"/> class.
        /// </summary>
        /// <param name="playerViewModel">The view model of episodes to play.</param>
        public NowPlayingPage(MediaPlayerViewModel playerViewModel)
        {
            BindingContext = playerViewModel;
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MediaPlayerViewModel model)
            {
                model.Subscribe();
                model.StartPlayback();
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is MediaPlayerViewModel model)
            {
                model.StopPlayback();
                model.Unsubscribe();
            }
        }
    }
}
