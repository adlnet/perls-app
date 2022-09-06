using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Collections;
using Float.Core.Commands;
using Float.Core.Events;
using Float.FileDownloader;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model for the download management page.
    /// </summary>
    public class DownloadManagementViewModel : BasePageViewModel
    {
        readonly ICommand clearCommand;
        readonly IAsyncOutCommand<bool> deleteAllCommand;
        readonly Command<IDownloadable> deleteContentCommand;
        readonly SelectViewModelCommand<IDownloadable> selectCommand;
        DownloadedItemViewModel selected;
        long activitySize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadManagementViewModel"/> class.
        /// </summary>
        /// <param name="clearCommand">A command to clear the cache.</param>
        /// <param name="deleteAllCommand">A command to delete all downloads.</param>
        /// <param name="deleteContentCommand">A command to invoke to delete locally cached content.</param>
        /// <param name="selectCommand">A command to select one download.</param>
        public DownloadManagementViewModel(ICommand clearCommand, IAsyncOutCommand<bool> deleteAllCommand, Command<IDownloadable> deleteContentCommand, SelectViewModelCommand<IDownloadable> selectCommand)
        {
            Title = Strings.DownloadManagement.ToUpper(CultureInfo.CurrentCulture);

            this.clearCommand = clearCommand ?? throw new ArgumentNullException(nameof(clearCommand));
            this.deleteAllCommand = deleteAllCommand ?? throw new ArgumentNullException(nameof(deleteAllCommand));
            this.deleteContentCommand = deleteContentCommand ?? throw new ArgumentNullException(nameof(deleteContentCommand));
            this.selectCommand = selectCommand ?? throw new ArgumentNullException(nameof(selectCommand));

            ClearCacheCommand = new DebounceCommand(HandleClearCache);
            DeleteAllDownloadsCommand = new AsyncCommand(HandleDeleteAll);

            var statuses = DependencyService.Get<IDownloaderService>().ActiveTrackedDownloads;
            var viewModels = from item in DownloadedItems
                                orderby item.Name
                                select CreateViewModel(item, statuses);
            DownloadedActivities.AddRange(viewModels);
            TotalActivitySize = DownloadedActivities.Sum(act => act.DownloadSize);
        }

        /// <summary>
        /// Gets or sets the selected activity.
        /// </summary>
        /// <value>The selected activity.</value>
        public DownloadedItemViewModel SelectedActivity
        {
            get
            {
                return selected;
            }

            set
            {
                if (value == selected)
                {
                    return;
                }

                selected = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are cache files to clear.
        /// </summary>
        /// <value><c>true</c> if there is a cache to clear, <c>false</c> otherwise.</value>
        public bool HasCache => TotalCacheSize > 1;

        /// <summary>
        /// Gets a value indicating whether there are activity files to delete.
        /// </summary>
        /// <value><c>true</c> if there are files to delete, <c>false</c> otherwise.</value>
        public bool HasDownloads => TotalActivitySize > 1;

        /// <summary>
        /// Gets a label for the clear cache button.
        /// </summary>
        /// <value>The clear cache label.</value>
        public string ClearCacheLabel => SizeLabel(Strings.ClearCacheLabel, TotalCacheSize);

        /// <summary>
        /// Gets a label for the delete downloads button.
        /// </summary>
        /// <value>The delete downloads label.</value>
        public string DeleteDownloadsLabel => SizeLabel(Strings.DeleteDownloadsLabel, TotalActivitySize);

        /// <summary>
        /// Gets a label for the top of the list of downloads.
        /// </summary>
        /// <value>A download list label.</value>
        public string YourDownloadsLabel => Strings.YourDownloads;

        /// <summary>
        /// Gets a list of downloaded activities.
        /// </summary>
        /// <value>All downloaded activities.</value>
        public ObservableElementCollection<DownloadedItemViewModel> DownloadedActivities { get; } = new ObservableElementCollection<DownloadedItemViewModel>();

        /// <summary>
        /// Gets a command to clear the cache.
        /// </summary>
        /// <value>The clear cache command.</value>
        public ICommand ClearCacheCommand { get; }

        /// <summary>
        /// Gets a commmand to delete downloaded items.
        /// </summary>
        /// <value>The delete downloads command.</value>
        public ICommand DeleteAllDownloadsCommand { get; }

        IEnumerable<IDownloadable> DownloadedItems => DependencyService.Get<IDownloaderService>().DownloadedItems.OfType<IDownloadable>().Distinct();

        IEnumerable<string> AllActivityFolders => Directory.EnumerateDirectories(FileSystem.CacheDirectory);

        IEnumerable<string> AllCacheFiles => DependencyService.Get<ICacheRegistryService>().RegisteredCaches();

        long TotalCacheSize => AllCacheFiles?.Sum(file => File.Exists(file) ? new FileInfo(file).Length : 0) ?? 0;

        long TotalActivitySize
        {
            get => activitySize;
            set => SetField(ref activitySize, value);
        }

        int DownloadMegabytes => BytesToMega(TotalActivitySize);

        int CacheMegabytes => BytesToMega(TotalCacheSize);

        /// <inheritdoc/>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(SelectedActivity):
                    if (selected != null)
                    {
                        selectCommand.Execute(selected);
                        SelectedActivity = null;
                    }

                    break;
                case nameof(DownloadedItems):
                    OnPropertyChanged(nameof(DownloadedActivities));
                    break;
                case nameof(AllActivityFolders):
                    OnPropertyChanged(nameof(DownloadedActivities));
                    break;
                case nameof(AllCacheFiles):
                    OnPropertyChanged(nameof(CacheMegabytes));
                    OnPropertyChanged(nameof(TotalCacheSize));
                    break;
                case nameof(TotalActivitySize):
                    OnPropertyChanged(nameof(DownloadMegabytes));
                    OnPropertyChanged(nameof(HasDownloads));
                    break;
                case nameof(DownloadedActivities):
                    OnPropertyChanged(nameof(TotalActivitySize));
                    break;
                case nameof(DownloadMegabytes):
                    OnPropertyChanged(nameof(DeleteDownloadsLabel));
                    break;
                case nameof(CacheMegabytes):
                    OnPropertyChanged(nameof(ClearCacheLabel));
                    break;
                case nameof(TotalCacheSize):
                    OnPropertyChanged(nameof(HasCache));
                    break;
            }
        }

        /// <inheritdoc />
        protected override void OnObservingBegan()
        {
            base.OnObservingBegan();
            DependencyService.Get<IDownloaderService>().OnDownloadStarted += HandleDownloadStarted;
        }

        /// <inheritdoc />
        protected override void OnObservingEnded()
        {
            base.OnObservingEnded();
            DependencyService.Get<IDownloaderService>().OnDownloadStarted -= HandleDownloadStarted;
        }

        static TValue GetValueOrDefault<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, TKey key)
        {
            foreach (var pair in dictionary)
            {
                if (EqualityComparer<TKey>.Default.Equals(pair.Key, key))
                {
                    return pair.Value;
                }
            }

            return default;
        }

        static int BytesToMega(long bytes)
        {
            return (int)Math.Round(bytes / 1000000f);
        }

        string SizeLabel(string format, long bytes)
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

        void HandleClearCache()
        {
            clearCommand.Execute(this);

            OnPropertyChanged(nameof(CacheMegabytes));
            OnPropertyChanged(nameof(ClearCacheLabel));
        }

        async Task HandleDeleteAll()
        {
            var result = await deleteAllCommand.ExecuteAsync(this).ConfigureAwait(false);

            if (result)
            {
                DownloadedActivities.Clear();
            }

            UpdateViewModel();
        }

        void HandleDeleteItem(IDownloadable acontent)
        {
            deleteContentCommand.Execute(acontent);

            var modelForContent = DownloadedActivities.FirstOrDefault(vm => vm.Id == acontent.Id);

            if (modelForContent is DownloadedItemViewModel model)
            {
                DownloadedActivities.Remove(model);
            }

            UpdateViewModel();
        }

        void HandleDownloadComplete()
        {
            UpdateViewModel();
        }

        void UpdateViewModel()
        {
            TotalActivitySize = DownloadedActivities.Sum(act => act.DownloadSize);
            OnPropertyChanged(nameof(DownloadedActivities));
        }

        void HandleDownloadStarted(object sender, TypedEventArgs<DownloadStatus> args)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Data is not DownloadStatus status)
            {
                return;
            }

            DependencyService.Get<AnalyticsService>().TrackEvent("Download Started", new Dictionary<string, string>
            {
                { "name", status.Name },
            });

            if (sender is IDownloadable item)
            {
                var model = new DownloadedItemViewModel(item, new Command<IDownloadable>(HandleDeleteItem), new Command(HandleDownloadComplete), status);
                DownloadedActivities.Add(model);
                UpdateViewModel();
            }
        }

        DownloadedItemViewModel CreateViewModel(IDownloadable item, IEnumerable<KeyValuePair<Guid, DownloadStatus>> statuses)
        {
            return new DownloadedItemViewModel(
                item,
                new Command<IDownloadable>(HandleDeleteItem),
                new Command(HandleDownloadComplete),
                GetValueOrDefault(statuses, item.Id));
        }
    }
}
