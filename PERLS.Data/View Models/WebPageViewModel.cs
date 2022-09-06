using System;
using System.Windows.Input;
using Float.Core.ViewModels;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The web page view model.
    /// </summary>
    public class WebPageViewModel : BasePageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebPageViewModel"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public WebPageViewModel(string location = null)
        {
            Location = location;

            LoadingEvent = new Command((obj) =>
            {
                IsLoading = obj is WebNavigatingEventArgs;
            });
        }

        /// <summary>
        /// Gets the loading event for the WebView.
        /// </summary>
        /// <value>The loading event command.</value>
        public ICommand LoadingEvent { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public new string Title => Strings.InterestsTitle;

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        /// <value>
        /// The Location.
        /// </value>
        public string Location { get; set; }
    }
}
