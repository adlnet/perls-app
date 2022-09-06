using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// The Prompt Results Card.
    /// </summary>
    public partial class PromptResultsCard : BaseCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptResultsCard"/> class.
        /// </summary>
        public PromptResultsCard()
        {
            InitializeComponent();

            // Sets the PrimaryTextColor back to default.
            Resources["PrimaryTextColor"] = Application.Current.Resources["PrimaryTextColor"];
        }
    }
}
