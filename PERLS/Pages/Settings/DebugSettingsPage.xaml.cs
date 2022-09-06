using System;
using System.Collections.Generic;
using System.Text;
using PERLS;
using Xamarin.Forms;

namespace PERLS.Pages.Settings
{
    /// <summary>
    /// The debug settings page.
    /// </summary>
    public partial class DebugSettingsPage : ContentPage
    {
        bool darkMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSettingsPage"/> class.
        /// </summary>
        public DebugSettingsPage()
        {
            InitializeComponent();
        }

        void SwitchColorsClicked(object sender, EventArgs e)
        {
            if (darkMode)
            {
                App.Current.Resources["BackgroundColor"] = Color.White;
                App.Current.Resources["lasdpfl"] = Color.CornflowerBlue;
                App.Current.Resources["PrimaryColor"] = Color.FromHex("#231f20");
                App.Current.Resources["TertiaryColor"] = Color.FromHex("#fe8917");

                App.Current.Resources["QuizColor"] = Color.FromHex("#4587a8");
                App.Current.Resources["AltQuizColor"] = Color.FromHex("#357190");
                App.Current.Resources["TipColor"] = Color.FromHex("#e86659");
                App.Current.Resources["AltTipColor"] = Color.FromHex("#bc2c1e");
                App.Current.Resources["FlashCardColor"] = Color.FromHex("#be76a5");
                App.Current.Resources["FlashCardLightColor"] = Color.FromHex("#9a457c");
                App.Current.Resources["SearchBarBackgroundColor"] = Color.FromHex("#ebebeb");
                App.Current.Resources["TransparentBlackColor"] = Color.FromHex("#66000000");
            }
            else
            {
                App.Current.Resources["BackgroundColor"] = Color.Black;
                App.Current.Resources["PrimaryColor"] = Color.FromHex("#faf2f2");
                App.Current.Resources["TertiaryColor"] = Color.FromHex("#3F8A7C");

                App.Current.Resources["QuizColor"] = Color.FromHex("#ffaa00");
                App.Current.Resources["AltQuizColor"] = Color.FromHex("#78ff02");
                App.Current.Resources["TipColor"] = Color.FromHex("#ff00bb");
                App.Current.Resources["AltTipColor"] = Color.FromHex("#7efffd");
                App.Current.Resources["FlashCardColor"] = Color.FromHex("#9a7bff");
                App.Current.Resources["FlashCardLightColor"] = Color.FromHex("#ff0900");
                App.Current.Resources["SearchBarBackgroundColor"] = Color.FromHex("#fff200");
                App.Current.Resources["TransparentBlackColor"] = Color.FromHex("#0468ff00");
            }

            darkMode = !darkMode;
        }
    }
}
