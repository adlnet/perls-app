using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// Base implementation of a Card; primarily defines the text styles used by cards.
    /// </summary>
    public abstract partial class BaseCard : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCard"/> class.
        /// </summary>
        protected BaseCard()
        {
            InitializeComponent();

            // Some day, all cards will have dynamic foreground colors from the server.
            // But for now, basically all cards have white text.
            Resources["PrimaryTextColor"] = Color.White;
        }
    }
}
