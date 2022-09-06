using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// The Test Results Card.
    /// </summary>
    public partial class TestResultsCard : BaseCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultsCard"/> class.
        /// </summary>
        public TestResultsCard()
        {
            InitializeComponent();

            // Sets the PrimaryTextColor back to default.
            Resources["PrimaryTextColor"] = Application.Current.Resources["PrimaryTextColor"];
        }
    }
}
