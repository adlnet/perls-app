using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using PERLS.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SearchBar), typeof(PerlsSearchBarRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Perls search bar renderer.
    /// </summary>
    public class PerlsSearchBarRenderer : SearchBarRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerlsSearchBarRenderer"/> class.
        /// </summary>
        /// <param name="context">The Context.</param>
        public PerlsSearchBarRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Element is SearchBar searchBar)
            {
                searchBar.TextColor = (Color)Application.Current.Resources["PrimaryTextColor"];
            }

            var searchPlate = FindViewByName<ViewGroup>("android:id/search_plate");

            if (Control is SearchView searchView && searchView.Context is Context context)
            {
                searchView.Background = ContextCompat.GetDrawable(context, Resource.Drawable.custom_search_view);

                if (searchPlate != null)
                {
                    searchPlate.Background = ContextCompat.GetDrawable(context, Resource.Drawable.custom_search_view);
                }
            }

            searchPlate?.SetBackgroundResource(Resource.Drawable.custom_search_view);

            var searchFrame = FindViewByName<ViewGroup>("android:id/search_edit_frame");
            searchFrame?.SetBackgroundResource(Resource.Drawable.custom_search_view);

            var searchHintIcon = FindViewByName<ImageView>("android:id/search_mag_icon");
            searchHintIcon?.SetColorFilter(((Color)Application.Current.Resources["SecondaryColor"]).ToAndroid());
        }

        T FindViewByName<T>(string name) where T : Android.Views.View
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid string value");
            }

            return Control?.Resources?.GetIdentifier(name, null, null) is int id
                ? Control?.FindViewById(id) as T
                : null;
        }
    }
}
