using System;
using System.Windows.Input;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace PERLS.Pages
{
    /// <summary>
    /// The popup stack of cards card.
    /// </summary>
    public partial class PopupStackOfCardsCard : PopupPage
    {
        readonly ICommand onCloseCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupStackOfCardsCard"/> class.
        /// </summary>
        /// <param name="promptCollectionViewModel">The view model.</param>
        /// <param name="onCloseCommand">An optional command to invoke when the popup is closed.</param>
        public PopupStackOfCardsCard(PromptCollectionViewModel promptCollectionViewModel, ICommand onCloseCommand)
        {
            BindingContext = promptCollectionViewModel ?? throw new ArgumentNullException(nameof(promptCollectionViewModel));
            this.onCloseCommand = onCloseCommand;
            InitializeComponent();
        }

        void ClosePopupClicked(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
            onCloseCommand?.Execute(sender);
        }
    }
}
