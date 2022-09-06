using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using Float.Core.Commands;
using Float.Core.ViewModels;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary.Definition;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model for a single downloaded item on the download management page.
    /// </summary>
    public class DownloadedItemViewModel : ViewModel<IDownloadable>
    {
        readonly IDownloadable item;
        readonly Command<IDownloadable> deleteContentCommand;
        readonly ICommand downloadCompleteCommand;
        readonly DownloadStatus downloadStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadedItemViewModel"/> class.
        /// </summary>
        /// <param name="item">The downloaded content.</param>
        /// <param name="deleteContentCommand">A command to invoke to delete local content.</param>
        /// <param name="downloadCompleteCommand">A command to invoke when the download completes, if one is in progress.</param>
        /// <param name="status">The status of this item's download, if one exists. Optional.</param>
        public DownloadedItemViewModel(IDownloadable item, Command<IDownloadable> deleteContentCommand, ICommand downloadCompleteCommand, DownloadStatus status = null) : base(item)
        {
            this.item = item ?? throw new ArgumentNullException(nameof(item));
            this.deleteContentCommand = deleteContentCommand ?? throw new ArgumentNullException(nameof(deleteContentCommand));
            this.downloadCompleteCommand = downloadCompleteCommand ?? throw new ArgumentNullException(nameof(downloadCompleteCommand));
            this.downloadStatus = status;

            DeleteCommand = new DebounceCommand(HandleDeleteContent);
            Title = item?.Name ?? Strings.DefaultContentName;

            if (downloadStatus != null)
            {
                downloadStatus.PropertyChanged += HandleDownloadProgress;
                downloadStatus.DownloadsCompleted += HandleDownloadsCompleted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is downloading.
        /// </summary>
        /// <value><c>true</c> if downloading, <c>false</c> otherwise.</value>
        public bool IsDownloading => (downloadStatus?.State == DownloadStatus.DownloadState.Downloading || downloadStatus?.State == DownloadStatus.DownloadState.Waiting) && (downloadStatus?.PercentComplete ?? 1) < 0.99;

        /// <summary>
        /// Gets text for displaying download progress.
        /// </summary>
        /// <value>The download progress text.</value>
        public string DownloadProgressText => IsDownloading ? string.Format(CultureInfo.InvariantCulture, Strings.DownloadWithProgress, DownloadPercent) : Strings.DownloadNoProgress;

        /// <summary>
        /// Gets the download percent as a whole number from 0 to 100.
        /// </summary>
        /// <value>The percentage downloaded.</value>
        public int DownloadPercent => (int)Math.Round((downloadStatus?.PercentComplete * 100) ?? 0);

        /// <summary>
        /// Gets the command to delete this content.
        /// </summary>
        /// <value>The command to delete this content.</value>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Gets the thumbnail for this content.
        /// </summary>
        /// <value>The thumbnail.</value>
        public ImageSource Thumbnail => item is IActivity activity && activity.Thumbnail is Float.TinCan.ActivityLibrary.Definition.IFile thumbnail
            ? ImageSource.FromUri(thumbnail.Url)
            : ImageSource.FromFile("placeholder");

        /// <summary>
        /// Gets the title of this content.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; }

        /// <summary>
        /// Gets the unique identifier for this content.
        /// </summary>
        /// <value>The unique content identifier.</value>
        public Guid Id => item.Id;

        /// <summary>
        /// Gets the downloaded size of this content, in bytes.
        /// </summary>
        /// <value>The downloaded size.</value>
        public long DownloadSize
        {
            get
            {
                if (Model?.DownloadableFile?.Url is Uri url)
                {
                    return DependencyService.Get<IDownloaderService>().GetFileSize(url);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a label for presenting the download size.
        /// </summary>
        /// <value>The label for presenting download size.</value>
        public string DownloadSizeLabel => SizeLabel(Strings.DownloadSizeLabel, DownloadSize);

        /// <summary>
        /// Gets an icon to indicate more actions are available.
        /// </summary>
        /// <value>The more action icon.</value>
        public ImageSource MoreIcon => SvgImageSource.FromResource(IsDownloading ? "PERLS.Data.Resources.close.svg" : "PERLS.Data.Resources.trash.svg");

        /// <summary>
        /// Gets an icon to indicate the download state of this content.
        /// </summary>
        /// <value>The download icon.</value>
        public ImageSource DownloadIcon => SvgImageSource.FromResource("PERLS.Data.Resources.download.svg", replaceStringMap: new Dictionary<string, string> { { "#fff", ((Color)Application.Current.Resources["DownloadManagementIconsColor"]).ToHex() } });

        /// <summary>
        /// Gets an icon to indicate that a task is in progress.
        /// </summary>
        /// <value>The progress icon.</value>
        public ImageSource ProgressIcon => ImageSource.FromResource("PERLS.Data.Resources.progress.gif");

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return item.Id.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is DownloadedItemViewModel model && model.GetHashCode() == GetHashCode();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsDownloading):
                    OnPropertyChanged(nameof(MoreIcon));
                    break;
                case nameof(DownloadSize):
                    OnPropertyChanged(nameof(DownloadSizeLabel));
                    break;
                case nameof(DownloadPercent):
                    OnPropertyChanged(nameof(DownloadProgressText));
                    break;
            }
        }

        static string SizeLabel(string format, long bytes)
        {
            if (bytes < 1)
            {
                return string.Format(CultureInfo.InvariantCulture, format, $"{0}");
            }

            var mega = BytesToMega(bytes);

            if (mega < 1)
            {
                return string.Format(CultureInfo.InvariantCulture, format, Strings.LessThanOne);
            }

            return string.Format(CultureInfo.InvariantCulture, format, $"{mega}");
        }

        static int BytesToMega(long bytes)
        {
            return (int)Math.Round(bytes / 1000000f);
        }

        void HandleDeleteContent()
        {
            if (IsDownloading)
            {
                downloadStatus.CancelDownload();
            }

            deleteContentCommand.Execute(item);
        }

        void HandleDownloadProgress(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "PercentComplete":
                    OnPropertyChanged(nameof(DownloadPercent));
                    OnPropertyChanged(nameof(DownloadProgressText));
                    return;
                default:
                    return;
            }
        }

        void HandleDownloadsCompleted(object sender, EventArgs args)
        {
            OnPropertyChanged(nameof(IsDownloading));
            OnPropertyChanged(nameof(MoreIcon));
            OnPropertyChanged(nameof(DownloadSize));
            OnPropertyChanged(nameof(DownloadSizeLabel));
            OnPropertyChanged(nameof(Thumbnail));

            downloadStatus.PropertyChanged -= HandleDownloadProgress;
            downloadStatus.DownloadsCompleted -= HandleDownloadsCompleted;

            downloadCompleteCommand?.Execute(sender);
        }
    }
}
