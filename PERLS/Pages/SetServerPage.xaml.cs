using System;
using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The set server page which will allow a user to set the debug server.
    /// </summary>
    public partial class SetServerPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetServerPage" /> class.
        /// </summary>
        /// <param name="viewModel">The Set Server View Model.</param>
        public SetServerPage(SetServerViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetServerPage"/> class.
        /// </summary>
        public SetServerPage() : base()
        {
            InitializeComponent();
        }
    }
}
