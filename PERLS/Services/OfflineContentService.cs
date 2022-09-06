using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Extensions;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.DataImplementation.Providers;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// A service that automatically downloads content when necessary.
    /// </summary>
    public class OfflineContentService : IOfflineContentService
    {
        const double ContentDownloadFrequencyInDays = 1.0;
        const int MaximumRecommendationDownloads = 50;
        readonly DiskProviderCache timestampCache = new DiskProviderCache("offline-content-service-time", false);
        readonly DiskProviderCache itemCache = new DiskProviderCache("offline-content-service-item", false);
        readonly DiskProviderCache originCache = new DiskProviderCache("offline-content-service-origin", false);
        readonly DiskProviderCache removalCache = new DiskProviderCache("offline-content-service-remove", false);
        readonly IDownloaderService downloadService = DependencyService.Get<IDownloaderService>();
        readonly ILearnerStateProvider stateProvider = DependencyService.Get<ILearnerStateProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineContentService"/> class.
        /// </summary>
        public OfflineContentService()
        {
        }

        /// <inheritdoc/>
        public async Task UpdateRecommendedItems(IEnumerable<IItem> items)
        {
            // courses can contain downloadable items
            var recursiveRecommendedItems = items
                .OfType<ICourse>()
                .SelectMany(course => course.LearningObjects.Where(IsIncomplete).Take(10))
                .OfType<IDownloadable>()
                .Concat(items.OfType<IDownloadable>())
                .Where(NotManuallyDeleted) // don't download content the user deleted
                .Distinct()
                .Take(MaximumRecommendationDownloads);

            // find cached items that are no longer recommended
            var previouslyCachedItems = itemCache.Values.OfType<IDownloadable>().ToDictionary(item => item.Id);
            var recommendedItems = recursiveRecommendedItems.ToDictionary(item => item.Id);
            var itemsToRemoveFromCache = previouslyCachedItems.Except(recommendedItems).Values();

            // remove old items from the cache and delete their local files
            itemsToRemoveFromCache.ForEach(Destore);

            // wait for all downloads to finish before adding to cache
            await downloadService.DownloadItemsInBackground(recursiveRecommendedItems.Where(ShouldDownloadContent), true).ConfigureAwait(false);

            // store successful downloads in cache
            recursiveRecommendedItems.Where(item => item.DownloadableFile?.IsDownloaded == true).ForEach(Store);
        }

        /// <inheritdoc/>
        public void SetContentOrigin(IItem item, Initiator contentOrigin)
        {
            originCache.Put(item.Id.ToString(), contentOrigin);
        }

        /// <inheritdoc/>
        public Initiator GetContentOrigin(IItem item)
        {
            if (!originCache.ContainsKey(item.Id.ToString()))
            {
                return Initiator.Unknown;
            }

            return originCache.Get<Initiator>(item.Id.ToString());
        }

        /// <inheritdoc/>
        public void SetRemovalOrigin(IItem item, Initiator removalOrigin)
        {
            removalCache.Put(item.Id.ToString(), removalOrigin);
        }

        /// <inheritdoc/>
        public Initiator GetRemovalOrigin(IItem item)
        {
            if (!removalCache.ContainsKey(item.Id.ToString()))
            {
                return Initiator.Unknown;
            }

            return removalCache.Get<Initiator>(item.Id.ToString());
        }

        /// <inheritdoc/>
        public void ClearCaches()
        {
            timestampCache.Clear();
            itemCache.Clear();
            originCache.Clear();
            removalCache.Clear();
        }

        bool NotManuallyDeleted(IDownloadable item) => GetRemovalOrigin(item) != Initiator.User;

        bool WasAutomaticallyDownloaded(IDownloadable item) => GetContentOrigin(item) != Initiator.User;

        void Store(IDownloadable item)
        {
            timestampCache.Put(item.Id.ToString(), DateTimeOffset.Now);
            itemCache.Put(item.Id.ToString(), item);
        }

        void Destore(IDownloadable item)
        {
            timestampCache.Delete(item.Id.ToString());
            itemCache.Delete(item.Id.ToString());

            // don't delete content that the user downloaded
            if (WasAutomaticallyDownloaded(item))
            {
                downloadService.RemoveDownloadedItem(item);
            }
        }

        bool ShouldDownloadContent(IDownloadable item)
        {
            if (item.DownloadableFile == null)
            {
                return false;
            }

            if (!timestampCache.ContainsKey(item.Id.ToString()))
            {
                return true;
            }

            var cacheDate = timestampCache.Get<DateTimeOffset>(item.Id.ToString());

            if (cacheDate == default)
            {
                return true;
            }

            var diff = DateTimeOffset.Now - cacheDate;

            if (diff.TotalDays > ContentDownloadFrequencyInDays)
            {
                return true;
            }

            return false;
        }

        bool IsIncomplete(IItem item)
        {
            return stateProvider.GetState(item).Completed != CorpusItemLearnerState.Status.Enabled;
        }
    }
}
