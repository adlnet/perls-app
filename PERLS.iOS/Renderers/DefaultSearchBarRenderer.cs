using Foundation;
using PERLS.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBar), typeof(DefaultSearchBarRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Overrides the default button.
    /// </summary>
    public class DefaultSearchBarRenderer : SearchBarRenderer
    {
        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                return;
            }

            UISearchBar.Appearance.BarTintColor = ((Color)App.Current.Resources["SearchBarBackgroundColor"]).ToUIColor();
            UITextView.AppearanceWhenContainedIn(Control.GetType()).BackgroundColor = ((Color)App.Current.Resources["SearchBarBackgroundColor"]).ToUIColor();

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                Control.SearchTextField.BackgroundColor = ((Color)App.Current.Resources["SearchBarBackgroundColor"]).ToUIColor();
                Control.SearchTextField.TextColor = ((Color)App.Current.Resources["PrimaryTextColor"]).ToUIColor();
                var leftImage = Control.SearchTextField.LeftView as UIImageView;
                leftImage.TintColor = ((Color)App.Current.Resources["SecondaryColor"]).ToUIColor();
            }
            else
            {
                using (var searchKey = new NSString("_searchField"))
                {
                    var textField = (UITextField)Control.ValueForKey(searchKey);
                    textField.TextColor = ((Color)App.Current.Resources["PrimaryTextColor"]).ToUIColor();
                    textField.BackgroundColor = ((Color)App.Current.Resources["SearchBarBackgroundColor"]).ToUIColor();
                    var imageView = textField.LeftView as UIImageView;
                    imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    imageView.TintColor = ((Color)App.Current.Resources["SecondaryColor"]).ToUIColor();
                }
            }

            Control.BackgroundColor = ((Color)App.Current.Resources["BackgroundColor"]).ToUIColor();
        }
    }
}
