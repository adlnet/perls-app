using System;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The search page.
    /// </summary>
    public partial class SearchPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Collection.SelectedItem = null;
            if (BindingContext is SearchViewModel searchViewModel)
            {
                searchViewModel.ClearSelection();
            }
        }

        void OnUserInteraction(object sender, EventArgs e)
        {
            ((App)Application.Current).InteractivityHelper.ResetIdleTimer();
        }
    }
}
