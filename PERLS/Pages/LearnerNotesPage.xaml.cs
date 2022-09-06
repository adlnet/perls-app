using System;
using System.Collections.Generic;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The Learner Notes Page.
    /// </summary>
    public partial class LearnerNotesPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerNotesPage"/> class.
        /// </summary>
        public LearnerNotesPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerNotesPage"/> class.
        /// </summary>
        /// <param name="learnerNotesViewModel">The view model.</param>
        public LearnerNotesPage(LearnerNotesViewModel learnerNotesViewModel)
        {
            BindingContext = learnerNotesViewModel;
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Collection.SelectedItem = null;

            if (BindingContext is LearnerNotesViewModel learnerNotesViewModel)
            {
                learnerNotesViewModel.Refresh();
            }
        }
    }
}
