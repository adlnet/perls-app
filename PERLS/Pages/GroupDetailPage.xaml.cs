using System;
using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// A page to view details about a single group.
    /// </summary>
    public partial class GroupDetailPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDetailPage"/> class.
        /// </summary>
        /// <param name="groupViewModel">The group view model.</param>
        public GroupDetailPage(GroupPageViewModel groupViewModel)
        {
            BindingContext = groupViewModel ?? throw new ArgumentNullException(nameof(groupViewModel));
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is GroupPageViewModel groupViewModel)
            {
                // load data
                groupViewModel.Refresh();
            }
        }
    }
}
