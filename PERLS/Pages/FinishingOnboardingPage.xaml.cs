using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// A mostly empty page that is displayed at the end of onboarding while the app is prepared for the user.
    /// </summary>
    public partial class FinishingOnboardingPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishingOnboardingPage"/> class.
        /// </summary>
        public FinishingOnboardingPage()
        {
            InitializeComponent();
            Resources["PrimaryTextColor"] = Application.Current.Resources["TertiaryColor"];
        }
    }
}
