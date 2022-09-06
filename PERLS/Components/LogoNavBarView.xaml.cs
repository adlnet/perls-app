using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// Navbar with a logo.
    /// </summary>
    public partial class LogoNavBarView : ContentView
    {
        readonly bool showLogo;
        readonly string title;
        LogoNavBarViewModel context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoNavBarView"/> class.
        /// </summary>
        /// <param name="showLogo">Whether or not to show the app logo in the nav bar.</param>
        /// <param name="title">The title to show in the nav bar. Optional.</param>
        public LogoNavBarView(bool showLogo, string title = null)
        {
            this.showLogo = showLogo;
            this.title = title;
            var navigationProvider = DependencyService.Get<INavigationCommandProvider>();
            context = new LogoNavBarViewModel(navigationProvider.SettingSelected, showLogo, title);
            BindingContext = context;

            InitializeComponent();
        }

        /// <summary>
        /// Call when the parent view is disappearing to unsubscribe from events.
        /// </summary>
        public void OnDisappearing()
        {
            context?.Unsubscribe();
        }
    }
}
