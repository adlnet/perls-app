using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The Badges page.
    /// </summary>
    public partial class BadgesPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgesPage"/> class.
        /// </summary>
        public BadgesPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Collection.SelectedItem = null;

            if (BindingContext is BadgesCollectionViewModel badgesCollection)
            {
                badgesCollection.Refresh();
            }
        }
    }
}
