namespace PERLS
{
    /// <summary>
    /// The App Shell.
    /// </summary>
    public partial class CorpusShell : Xamarin.Forms.Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorpusShell"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public CorpusShell(Data.ViewModels.CorpusShellViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            foreach (var item in ToolbarItems)
            {
                item.BindingContext = viewModel;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this shell is the currently visible Shell.
        /// </summary>
        /// <value><c>true</c> if this shell is the currently visible Shell, <c>false</c> otherwise.</value>
        public bool IsCurrent => Current == this;
    }
}
