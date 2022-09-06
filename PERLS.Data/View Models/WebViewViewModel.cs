using System;
using System.Windows.Input;
using PERLS.Data.Definition.Services;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The interests and groups webview view model.
    /// </summary>
    public class WebViewViewModel : AuthenticatingWebViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewViewModel"/> class.
        /// </summary>
        /// <param name="linkClicked">The link clicked command.</param>
        /// <param name="pageFailedToLoad">The page failed to load command.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="networkConnectionService">The nextwork connection service, optionally.</param>
        /// <param name="title">The title of the Page.</param>
        public WebViewViewModel(ICommand linkClicked, ICommand pageFailedToLoad, string destinationPath, INetworkConnectionService networkConnectionService = null, string title = null) : base(linkClicked, pageFailedToLoad, destinationPath, false, networkConnectionService)
        {
            Title = title;
        }
    }
}
