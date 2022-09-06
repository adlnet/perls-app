using System;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// The Prompt Card as a popup page.
    /// </summary>
    public partial class PromptPopupCard : PopupPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptPopupCard"/> class.
        /// </summary>
        /// <param name="promptCollectionViewModel">The prompt collection view model.</param>
        public PromptPopupCard(PromptCollectionViewModel promptCollectionViewModel)
        {
            if (promptCollectionViewModel == null)
            {
                throw new ArgumentNullException(nameof(promptCollectionViewModel));
            }

            BindingContext = promptCollectionViewModel;
            InitializeComponent();
        }
    }
}
