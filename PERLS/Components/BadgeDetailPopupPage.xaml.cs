using System;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace PERLS.Components
{
    /// <summary>
    /// The Bdage Detail Popup Page.
    /// </summary>
    public partial class BadgeDetailPopupPage : PopupPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeDetailPopupPage"/> class.
        /// </summary>
        /// <param name="badgeViewModel">The badge view model.</param>
        public BadgeDetailPopupPage(BadgeViewModel badgeViewModel)
        {
            BindingContext = badgeViewModel ?? throw new ArgumentNullException(nameof(badgeViewModel));
            InitializeComponent();
        }
    }
}
