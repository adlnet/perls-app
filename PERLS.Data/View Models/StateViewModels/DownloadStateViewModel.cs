using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using Float.Core.ViewModels;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The state of a download.
    /// </summary>
    public class DownloadStateViewModel : ViewModel<IItem>
    {
        static readonly Dictionary<string, string> ReplaceMap = new Dictionary<string, string> { { "#fff", ((Color)Application.Current.Resources["DownloadedColor"]).ToHex() } };
        readonly IAsyncCommand<IItem> downloadContentCommand;
        bool isDownloading;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadStateViewModel"/> class.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public DownloadStateViewModel(IItem item, IAsyncCommand<IItem> downloadContentCommand) : base(item)
        {
            this.downloadContentCommand = downloadContentCommand;
            DownloadCommand = new AsyncCommand(HandleDownload);
        }

        /// <summary>
        /// Gets a command to download this corpus item.
        /// </summary>
        /// <value>The download command.</value>
        public ICommand DownloadCommand { get; }

        /// <summary>
        /// Gets a value indicating whether this item is downloading.
        /// </summary>
        /// <value><c>true</c> if currently downloading, <c>false</c> otherwise.</value>
        public bool IsDownloading
        {
            get => isDownloading;
            private set => SetField(ref isDownloading, value);
        }

        /// <summary>
        /// Gets a value indicating whether the current corpus item is downloaded.
        /// </summary>
        /// <value><c>true</c> if downloaded, <c>false</c> otherwise.</value>
        public bool HasBeenDownloaded => DependencyService.Get<IDownloaderService>().GetItemDownloadStatus(Model);

        /// <summary>
        /// Gets a value indicating whether the current corpus item is downloadable.
        /// </summary>
        /// <value><c>true</c> if downloadable, <c>false</c> otherwise.</value>
        public bool IsDownloadableContent => Model is IPackagedContent content && content.PackageFile != null && Constants.OfflineAccess;

        /// <summary>
        /// Gets a value indicating whether this item can be downloaded (is downloadable, but not downloaded yet).
        /// </summary>
        /// <value><c>true</c> if downloadable, <c>false</c> otherwise.</value>
        public bool CanBeDownloaded => IsDownloadableContent && !HasBeenDownloaded && !IsDownloading;

        /// <summary>
        /// Gets an image to use as the download icon.
        /// The image updates to reflect the current download state.
        /// </summary>
        /// <value>The download icon.</value>
        public ImageSource DownloadIcon
        {
            get
            {
                if (CanBeDownloaded)
                {
                    return SvgImageSource.FromResource("PERLS.Data.Resources.download.svg");
                }
                else if (HasBeenDownloaded)
                {
                    return SvgImageSource.FromResource("PERLS.Data.Resources.download_complete.svg", replaceStringMap: ReplaceMap);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets an image to use as the progress indicator.
        /// </summary>
        /// <value>The progress indicator image.</value>
        public ImageSource ProgressIcon => ImageSource.FromResource("PERLS.Data.Resources.progress.gif");

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsDownloadableContent):
                case nameof(HasBeenDownloaded):
                case nameof(IsDownloading):
                    OnPropertyChanged(nameof(CanBeDownloaded));
                    OnPropertyChanged(nameof(DownloadIcon));
                    break;
            }
        }

        async Task HandleDownload()
        {
            if (downloadContentCommand == null)
            {
                throw new InvalidOperationException("Cannot download content without a download comand.");
            }

            IsDownloading = true;

            await downloadContentCommand.ExecuteAsync(Model).ConfigureAwait(false);

            IsDownloading = false;
            OnPropertyChanged(nameof(HasBeenDownloaded));
        }
    }
}
