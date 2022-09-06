using System;
using System.Threading.Tasks;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The onboarding page.
    /// </summary>
    public partial class OnboardingPage : BasePage
    {
        bool currentlyRepositioning = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public OnboardingPage(LandingGroupViewModel viewModel)
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            Resources["PageWidth"] = 300;
            viewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        /// <inheritdoc/>
        public override bool AllowLandscape => false;

        /// <inheritdoc />
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Resources["PageWidth"] = width;
            var ratio = 0.5;
            logoImage.WidthRequest = width * ratio;
            logoImage.HeightRequest = width * ratio * (56 / 220.0);
            carousel.HeightRequest = height / 1.7;
        }

        /// <summary>
        /// Handles the carousel scroll.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="args">The Arguments.</param>
        protected void HandleCarouselScroll(object sender, ItemsViewScrolledEventArgs args)
        {
            if (!currentlyRepositioning && args != null)
            {
                (BindingContext as LandingGroupViewModel).Position = args.CenterItemIndex;
            }
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LandingGroupViewModel.Position))
            {
                currentlyRepositioning = true;
                carousel.ScrollTo((sender as LandingGroupViewModel).Position);
                Task.Delay(400).ContinueWith((task) => currentlyRepositioning = false, TaskScheduler.Default);
            }
        }
    }
}
