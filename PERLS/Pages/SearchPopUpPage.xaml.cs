using System;
using Float.Core.ViewModels;
using PERLS.Components;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace PERLS.Pages
{
    /// <summary>
    /// Search pop up page.
    /// </summary>
    public partial class SearchPopUpPage : PopupPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPopUpPage"/> class.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        public SearchPopUpPage(ViewModel<IItem> viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            popupContent.Content = ViewForItemViewModelFactory.CreateView(viewModel);
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is CardViewModel card)
            {
                card.CardAppearedCommand.Execute(this);
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is CardViewModel card)
            {
                card.CardDisappearedCommand.Execute(this);
            }
        }

        /// <summary>
        /// Ons the back button pressed.
        /// </summary>
        /// <returns><c>true</c>, if back button pressed was oned, <c>false</c> otherwise.</returns>
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        /// <summary>
        /// A method to handle Close button tap event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        void CloseButtonTappedEventHandler(object sender, EventArgs args)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
