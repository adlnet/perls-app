using System;
using System.Linq;
using Float.Core.Analytics;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The following page.
    /// </summary>
    public partial class FollowingPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FollowingPage"/> class.
        /// </summary>
        public FollowingPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this is showing from the enhanced dashboard.
        /// </summary>
        /// <value>
        /// A value indicating whether or not this is showing from the enhanced dashboard.
        /// </value>
        public bool ShowingFromEnhanced { get; set; } = false;

        /// <inheritdoc />
        public override bool ShowLogo => !ShowingFromEnhanced;

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => !ShowingFromEnhanced;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Collection.SelectedItem = null;
            if (BindingContext is FollowingViewModel followingViewModel)
            {
                followingViewModel.ClearSelection();
                try
                {
                    followingViewModel.GetContentCommand.Execute(followingViewModel);
                }
                catch (Exception e)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                    Error = e;
                }
            }
        }
    }
}
