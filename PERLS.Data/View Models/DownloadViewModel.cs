using System.Windows.Input;
using Float.Core.ViewModels;
using Float.FileDownloader;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model for downloading a content package.
    /// </summary>
    public class DownloadViewModel : ViewModel<DownloadStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadViewModel"/> class.
        /// </summary>
        /// <param name="downloadStatus">The status of the download.</param>
        /// <param name="downloadCanceled">A command to invoke when the user requests to cancel the download.</param>
        public DownloadViewModel(DownloadStatus downloadStatus, ICommand downloadCanceled) : base(downloadStatus)
        {
            CancelCommand = downloadCanceled;
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        /// <value>The package name.</value>
        public string PackageName => Model.Name;

        /// <summary>
        /// Gets the label for the cancel button.
        /// </summary>
        /// <value>The cancel label.</value>
        public string CancelLabel => Strings.CancelDownload;

        /// <summary>
        /// Gets the downloading.
        /// </summary>
        /// <value>The downloading.</value>
        [NotifyWhenPropertyChanges(nameof(DownloadStatus.Name))]
        public string Downloading => Strings.DefaultLoadingMessage;

        /// <summary>
        /// Gets a value indicating whether the download can be cancelled.
        /// </summary>
        /// <value><c>true</c> if can cancel; otherwise, <c>false</c>.</value>
        [NotifyWhenPropertyChanges(nameof(DownloadStatus.State))]
        public bool CanCancel => Model.State == DownloadStatus.DownloadState.Downloading;

        /// <summary>
        /// Gets the icon image source for the activity indicator.
        /// </summary>
        /// <value>The progress icon.</value>
        public ImageSource ProgressIcon => ImageSource.FromResource("PERLS.Data.Resources.progress.gif");

        /// <summary>
        /// Gets the command for canceling the download.
        /// </summary>
        /// <value>The download cancel command.</value>
        public ICommand CancelCommand { get; }
    }
}
