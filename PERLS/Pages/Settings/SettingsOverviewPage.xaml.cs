using System;
using PERLS.Data.Commands;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages.Settings
{
    /// <summary>
    /// The settings page.
    /// </summary>
    public partial class SettingsOverviewPage : BasePage
    {
        bool itemSelected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsOverviewPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public SettingsOverviewPage(SettingsOverviewViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        /// <summary>
        /// Gets or sets a command to run when the page is disappearing.
        /// </summary>
        /// <value>A command to run when the page is disappearing.</value>
        public Command DisappearingCommand { get; set; }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            itemSelected = false;
            List.SelectedItem = null;

            if (BindingContext is SettingsOverviewViewModel settingsBc)
            {
                settingsBc.Subscribe();
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            List.SelectedItem = null;

            if (BindingContext is SettingsOverviewViewModel settingsBc)
            {
                // If an item is selected, the page will disappear BUT we don't want to finish it.
                if (DisappearingCommand != null && !itemSelected)
                {
                    DisappearingCommand.Execute(null);
                }

                settingsBc.Unsubscribe();
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            itemSelected = true;

            if (args.SelectedItem is INavigationOption option)
            {
                List.SelectedItem = null;
            }
        }

        void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
#if DEBUG
            var debugSettingsPage = new DebugSettingsPage();
            Navigation.PushAsync(debugSettingsPage);
#endif
        }
    }
}
