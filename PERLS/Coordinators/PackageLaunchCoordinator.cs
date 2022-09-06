using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Commands;
using Float.Core.Notifications;
using Float.Core.UI;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary;
using Float.TinCan.ActivityLibrary.Definition;
using Float.TinCan.LocalLRSServer;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ViewModels;
using PERLS.DataImplementation.Models;
using PERLS.DataImplementation.Providers;
using PERLS.Pages;
using PERLS.Services;
using TinCan;
using TinCan.Documents;
using Xamarin.Forms;

namespace PERLS.Coordinators
{
    /// <summary>
    /// Coordinates packaged content launch.
    /// </summary>
    public class PackageLaunchCoordinator : ActivityLaunchCoordinator
    {
        readonly LocalItemWebViewViewModel model;
        readonly TeaserViewModel teaser;
        readonly IPackagedContent content;
        readonly ILearnerStateProvider learnerStateProvider = DependencyService.Get<ILearnerStateProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageLaunchCoordinator"/> class.
        /// </summary>
        /// <param name="packagedContent">The packaged content to launch.</param>
        /// <param name="webLinkClicked">A command to invoke when a web link is clicked.</param>
        /// <param name="webPageLoadFailed">A command to invoke when loading the web page fails.</param>
        public PackageLaunchCoordinator(IPackagedContent packagedContent, ICommand webLinkClicked, ICommand webPageLoadFailed)
            : base(packagedContent, DependencyService.Get<IRemoteFileProvider>(), new ArticleMetaProvider(), DependencyService.Get<ILRSService>(), new ArticleServerDelegate(), DependencyService.Get<LocalHttpServer>().Uri, DependencyService.Get<IRemoteFileProcessor>())
        {
            teaser = new TeaserViewModel(packagedContent);
            model = new LocalItemWebViewViewModel(teaser, webLinkClicked, webPageLoadFailed);
            this.content = packagedContent;

            var itemWebViewPage = new ItemWebViewPage(model);
            ManagedHtmlActivityRunnerPage = itemWebViewPage;

            DependencyService.Get<IDownloaderService>().AddDownloadedItem(packagedContent);
        }

        /// <inheritdoc />
        protected override void Finish(EventArgs args)
        {
            ManagedHtmlActivityRunnerPage = null;

            // some content doesn't generated a completion statement; instead, the server generates one based on certain criteria
            // in those cases, we don't know if the content is complete or not right away, so we set it as unknown and trigger a fetch
            learnerStateProvider.GetState(content).SetCompletionStatusUnknown();
            learnerStateProvider.GetState(content);

            base.Finish(args);
        }

        /// <inheritdoc />
        protected override void AwardPointsForAssessment(Statement statement)
        {
            // not used
        }

        /// <inheritdoc />
        protected override BaseContentPage CreateActivityCompletePage(bool hasPostAssessment)
        {
            // not used
            return null;
        }

        /// <inheritdoc />
        protected override BaseContentPage CreateDownloadStatusPage(DownloadStatus downloadStatus)
        {
            DependencyService.Get<IDownloaderService>().TrackDownload(content, downloadStatus);
            return new DownloadPage(new DownloadViewModel(downloadStatus, new DebounceCommand(new Command(CancelDownload))));
        }

        /// <inheritdoc />
        protected override void HandleDownloadCompleted(object sender, EventArgs args)
        {
            base.HandleDownloadCompleted(sender, args);
            DependencyService.Get<IOfflineContentService>().SetContentOrigin(content, Initiator.User);
        }

        /// <inheritdoc />
        protected override Agent GetCurrentActor()
        {
            return DependencyService.Get<IAppContextService>().LearnerAgent;
        }

        /// <inheritdoc />
        protected override void OnActivityFinished(object sender, EventArgs args)
        {
            DependencyService.Get<ILearnerStateProvider>().MarkAsComplete(content);
        }

        /// <inheritdoc />
        protected override void OnActivityLaunchException(Exception exception)
        {
            if (exception is DownloadException)
            {
                DependencyService.Get<IDownloaderService>().RemoveDownloadedItem(content);
            }

            DependencyService.Get<INotificationHandler>().NotifyException(exception, Strings.ActivityLaunchError);
        }

        /// <inheritdoc />
        protected override void SetActivityPageBaseLocation(Uri uri)
        {
            // 2 reasons:
            // - LRS starts when the content starts and we need to wait for the port number to finish
            // - when they touch, they need to download the content (starting page may not be index.html)
            // - starting page in tincan.xml
            model.Url = uri;
        }

        void CancelDownload()
        {
            HandleCancelDownloadRequested(this, EventArgs.Empty);

            // Removing items while they are being unzipped results in a System.IO.IOException in the RemoveDownloadedItem method.
            // The try catch prevents a crash, but doesn't delete those items. Delaying 500ms and trying again, when the download
            // has been fully cancelled and is no longer unzipping, allows us to successfully delete the downloaded content.
            try
            {
                DependencyService.Get<IDownloaderService>().RemoveDownloadedItem(content);
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    DependencyService.Get<IDownloaderService>().RemoveDownloadedItem(content);
                });
            }
        }

        class ArticleMetaProvider : IActivityMetaDataProvider
        {
            readonly DiskProviderCache cache;

            public ArticleMetaProvider()
            {
                cache = new DiskProviderCache("article-meta-provider");
            }

            public Task<IActivityMetaData> GetMetaData(IActivity activity)
            {
                if (activity?.UUID?.ToString() is not string key)
                {
                    throw new ArgumentNullException(nameof(activity));
                }

                if (cache.Get<IActivityMetaData>(key) is IActivityMetaData meta)
                {
                    return Task.FromResult(meta);
                }

                if (activity is IPackagedContent content)
                {
                    var meta1 = ActivityMetaDataGenerator.CreateMetaData(new Uri(content.PackageFile.LocalExtractedPath), DependencyService.Get<LocalHttpServer>().Uri);
                    return Task.FromResult(meta1 as IActivityMetaData);
                }

                if (activity.Files.First() is Data.Definition.IFile file)
                {
                    var meta3 = new TinCanMetaData(file.LocalPath, activity.UUID, activity.Name);
                    return Task.FromResult(meta3 as IActivityMetaData);
                }

                // if the activity is not a package and does not have files, we can't provide metadata
                throw new InvalidOperationException();
            }

            public Task RemoveMetaData(IActivity activity)
            {
                cache.Delete(activity.UUID.ToString());
                return Task.CompletedTask;
            }

            public Task<IActivityMetaData> SaveMetaData(IActivity activity, IActivityMetaData metaData)
            {
                cache.Put(activity.UUID.ToString(), metaData);
                return Task.FromResult(metaData);
            }

            public Task SaveFileCachingMetadata(IActivity activity, Float.TinCan.ActivityLibrary.Definition.IFile file, HttpResponseMessage response)
            {
                if (activity is IPackagedContent packagedContent && response.Headers.ETag is EntityTagHeaderValue etag)
                {
                    packagedContent.PackageFile.ETag = etag.Tag;
                }

                return Task.CompletedTask;
            }
        }

        class ArticleServerDelegate : ILRSServerDelegate
        {
            public AgentProfileDocument AgentProfileDocumentForProfileId(string profileId)
            {
                return new AgentProfileDocument();
            }

            public string GetAccessConrolAllowOrigin()
            {
                var serverUri = DependencyService.Get<LocalHttpServer>().Uri;
                return serverUri.AbsoluteUri.TrimEnd('/');
            }
        }
    }
}
