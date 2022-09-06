using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Acr.UserDialogs;
using Float.Core.Analytics;
using Float.Core.Commands;
using Float.Core.Extensions;
using Float.Core.Net;
using Float.Core.Notifications;
using Float.Core.UX;
using Float.FileDownloader;
using PERLS.Components;
using PERLS.Data;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Factories;
using PERLS.Data.Infrastructure;
using PERLS.Data.ViewModels;
using PERLS.Data.ViewModels.Blocks;
using PERLS.DataImplementation.Models;
using PERLS.DataImplementation.Providers;
using PERLS.Extensions;
using PERLS.Pages;
using PERLS.Providers;
using PERLS.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using DownloadStatus = Float.FileDownloader.DownloadStatus;

namespace PERLS.Coordinators
{
    /// <summary>
    /// The primary coordinator for navigating the corpus.
    /// </summary>
    public class PrimaryNavigationCoordinator : CoordinatorParent
    {
        readonly CorpusShell shell;
        readonly CorpusShellViewModel viewModel;
        readonly INetworkConnectionService networkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryNavigationCoordinator"/> class.
        /// </summary>
        public PrimaryNavigationCoordinator()
        {
            var selectItemCommand = new DebounceCommand(HandleItemSelected, 500);
            var downloadItemCommand = new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException));
            DependencyService.Register<NavigationCommandProvider>();
            var navProvider = DependencyService.Get<INavigationCommandProvider>();
            navProvider.ItemSelected = selectItemCommand;
            navProvider.SettingSelected = new DebounceCommand(new Command(this.HandleNavigateSettings), 700);
            navProvider.DownloadRequested = downloadItemCommand;
            navProvider.NextArticleSelected = new AsyncCommand<Uri>(HandleNextArticle); // todo: debounce
            navProvider.GotoCustomGoalsSelected = new Command(HandleGoToCustomGoals);

            networkService = DependencyService.Get<INetworkConnectionService>();

            viewModel = new CorpusShellViewModel(
                selectItemCommand,
                new Command(this.HandleNavigateToSuggestion),
                downloadItemCommand,
                new Command(this.HandleSetGoalReminder),
                new Command(this.HandleAdjustGoal),
                new Command(this.HandleGoToAnnotation),
                new Command(this.HandleGoToGoalDetails));

            shell = new CorpusShell(viewModel);

            var commandFactory = new ShareCommandFactory(DependencyService.Get<INotificationHandler>());
            CommandFactoryRegistry.Register(commandFactory);
        }

        /// <inheritdoc />
        public override void Start(INavigationContext context)
        {
            var navigationContext = new ShellNavigationContext(shell);
            base.Start(navigationContext);
            Application.Current.MainPage = shell;

            MessagingCenter.Subscribe<Application>(this, App.ApplicationResumedNotificationName, HandleRefresh);
        }

        /// <summary>
        /// Kills the coordinator.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public void KillCoordinator(EventArgs args)
        {
            if (!IsFinished)
            {
                Finish(args);
            }
        }

        /// <summary>
        /// Navigates to a specific piece of content in the app.
        /// </summary>
        /// <remarks>
        /// Currently, this only supports taxonomy terms, but it should
        /// eventually be expanded to support deeplinking.
        /// </remarks>
        /// <param name="uri">The URI of the content.</param>
        /// <returns><c>true</c> if the uri can be handled.</returns>
        public async Task<bool> Navigate(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            // The use of "/taxonomy/term" here is an implementation detail and thus
            // _should_ eventually be handled by the `DataImplementation` project.
            // The pattern is "/taxonomy/term/{tid}".
            if (uri.LocalPath.StartsWith("/taxonomy/term", StringComparison.InvariantCultureIgnoreCase))
            {
                if (uri.Segments.Length < 4)
                {
                    return false;
                }

                var tid = uri.Segments[3];
                if (int.TryParse(tid, out int termId))
                {
                    // note that in this case we can't provide a term name
                    await HandleNavigateToTerm(termId, string.Empty);
                    return true;
                }
            }

            // The use of "/system/files" here is an implementation detail and thus
            // _should_ eventually be handled by the `DataImplementation` project.
            // The pattern is "/system/files/{year}-{month}/{filename}
            else if (uri.LocalPath.StartsWith("/system/files", StringComparison.InvariantCultureIgnoreCase))
            {
                await HandleOpenFileAsync(new EmbeddedFile(uri));
                return true;
            }
            else if (uri.Host == DependencyService.Get<IAppContextService>().Server.Host || uri.Host == "localhost" || uri.Host == "127.0.0.1")
            {
                if (uri.LocalPath.EndsWith(Constants.DiscussionPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToDiscussion(uri);
                    return true;
                }
                else if (uri.LocalPath.EndsWith(Constants.PromptsPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToPrompts();
                    return true;
                }
                else if (uri.LocalPath.EndsWith(Constants.HistoryPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToHistory();
                    return true;
                }
                else if (uri.LocalPath.EndsWith(Constants.BookmarksPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToBookmarks();
                    return true;
                }
                else if (uri.LocalPath.EndsWith(Constants.GroupsPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToGroups();
                    return true;
                }
                else if (uri.LocalPath.EndsWith(Constants.FollowingPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToFollowing();
                    return true;
                }
                else if (uri.LocalPath.StartsWith(Constants.VidyoRoomPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleNavigateToVidyoRoom(uri);
                    return true;
                }
                else if (uri.LocalPath.Equals(Constants.EditGoalPath))
                {
                    HandleNavigateToEditGoal(uri);
                    return true;
                }

                // Working based on a Uri we need to find the model.
                var corpusProvider = DependencyService.Get<ICorpusProvider>();
                IRemoteResource resourceForUri;

                var loadingIndicatorView = await ShowLoadingIndicator();
                try
                {
                    resourceForUri = await corpusProvider.GetResourceForUri(uri).ConfigureAwait(false);
                }
                catch (HttpRequestException httpException) when (httpException.Code == (int)HttpStatusCode.NotAcceptable)
                {
                    // If we're unable to load the resource we can try to show the page in an authenticated browser.
                    HandleNavigateToAuthenticatedPage(uri);

                    return true;
                }
                catch (Exception e)
                {
                    DependencyService.Get<AlertNotificationHandler>().NotifyException(e, Strings.ItemNotFoundMessage);
                    return true;
                }
                finally
                {
                    HideLoadingIndicator(loadingIndicatorView);
                }

                HandleNavigateToResource(resourceForUri);

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override void Finish(EventArgs args)
        {
            NavigationContext?.DismissPage(false);
            NavigationContext?.Reset(false);

            if (NavigationContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.Finish(args);

            MessagingCenter.Unsubscribe<Application>(this, App.ApplicationResumedNotificationName);
        }

        /// <inheritdoc />
        protected override void HandleChildFinish(object sender, EventArgs args)
        {
            base.HandleChildFinish(sender, args);

            if (args is SettingsCoordinator.LogoutEventArgs)
            {
                Finish(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Invoked when the application has requested a refresh of it's content (i.e. it has returned to the foreground).
        /// </summary>
        /// <param name="obj">The application.</param>
        void HandleRefresh(Application obj)
        {
            viewModel.Refresh();
        }

        /// <summary>
        /// Handle when the user selects an item.
        /// </summary>
        /// <param name="arg">The item selected.</param>
        void HandleItemSelected(object arg)
        {
            switch (arg)
            {
                case TeaserViewModel teaser:
                    HandleNavigateToItem(teaser.ModelItem);
                    break;
                case TagViewModel term:
                    HandleNavigateToTerm(term.ModelItem);
                    break;
                case CourseViewModel course:
                    HandleNavigateToItem(course.ModelItem);
                    break;
                case PodcastViewModel podcast:
                    HandleNavigateToItem(podcast.ModelItem);
                    break;
                case EpisodeViewModel episode:
                    HandleNavigateToItem(episode.ModelItem);
                    break;
                case BadgeViewModel badge:
                    HandleNavigateToBadge(badge);
                    break;
                case CertificateViewModel certificate:
                    HandleNavigateToCertificate(certificate);
                    break;
                case CertificateSharingViewModel certificateSharing:
                    HandleShareCertificate(certificateSharing);
                    break;
                case TeaserGroupViewModel group when group.Url.LocalPath.Contains("/taxonomy/term"):
                    if (int.TryParse(group.Url.Segments.Last(), out int termId))
                    {
                        Task.Run(() => HandleNavigateToTerm(termId, group.Name));
                    }
                    else
                    {
                        DependencyService.Get<INotificationHandler>().NotifyError(Strings.ViewTermErrorMessage);
                    }

                    break;
                case GroupTeaserViewModel group:
                    HandleNavigateToGroup(group.ModelItem);
                    break;
                case IItemGroup itemList:
                    HandleNavigateToItemGroup(itemList);
                    break;
                case ChipViewModel chip:
                    switch (chip.ModelItem)
                    {
                        case IGroup group:
                            HandleNavigateToGroup(group);
                            break;
                        case ITaxonomyTerm term:
                            HandleNavigateToTerm(term);
                            break;
                    }

                    break;
                case ITaxonomyTerm term:
                    HandleNavigateToTerm(term);
                    break;
                case BlockViewModel blockViewModel:
                    if (blockViewModel.ActionUri is Uri blockAction)
                    {
                        Application.Current.OpenUri(blockAction);
                    }
                    else if (blockViewModel.MoreUri is Uri blockMore)
                    {
                        Application.Current.OpenUri(blockMore);
                    }
                    else
                    {
                        DependencyService.Get<INotificationHandler>().NotifyError(Strings.UnknownErrorMessage);
                    }

                    break;
                default:
                    // Do nothing--the item selected is not a known type.
                    return;
            }

            ((App)Application.Current).InteractivityHelper.HideAndDisableInteractivityHints();
        }

        /// <summary>
        /// Handle the user's request to navigate to settings.
        /// </summary>
        void HandleNavigateSettings()
        {
            if (HasChild<SettingsCoordinator>())
            {
                // This will only occur if a double press makes it through the debounce command. Unlikely, and if
                // it does happen, we should ignore it.
                return;
            }

            StartChildCoordinator(
                new SettingsCoordinator(
                    new DebounceCommand(new Command<IDownloadable>(HandleNavigateToItem)),
                    new Command<IDownloadable>(HandleDeleteLocalContent),
                    new Command<Exception>(HandleDeleteException)));
        }

        /// <summary>
        /// Handles the user's request to navigate to making a suggestion.
        /// </summary>
        void HandleNavigateToSuggestion()
        {
            var webView = new ItemWebViewPage(new SuggestionsViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad)));
            webView.ToolbarItems.Add(new ToolbarItem(null, "close_button", DismissModal));
            webView.Title = Strings.MakeASuggestionTitle;
            var navPage = new NavigationPage(webView);
            NavigationContext.PresentPage(navPage);
        }

        void HandleAdjustGoal(object arg)
        {
            if (arg is LearnerGoalViewModel learnerGoalViewModel)
            {
                var goalView = new GoalAdjustmentPopupPage(learnerGoalViewModel);
                _ = PopupNavigation.Instance.PushAsync(goalView);
            }
        }

        async void HandleGoToGoalDetails(object arg)
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var context = DependencyService.Get<IAppContextService>();
            var insightsPage = new ItemWebViewPage(new LearnerWebViewViewModel(context.CurrentLearner, new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), new Uri(context.Server, $"{context.CurrentLearner.Url.AbsolutePath}/insights").AbsoluteUri, title: Strings.GoalDetailsTitle.ToUpper(CultureInfo.CurrentCulture)));
            NavigationContext.PushPage(insightsPage);
        }

        async void HandleGoToCustomGoals(object arg)
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var context = DependencyService.Get<IAppContextService>();
            var customGoalsPage = new ItemWebViewPage(new LearnerWebViewViewModel(context.CurrentLearner, new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), new Uri(context.Server, $"{context.CurrentLearner.Url.AbsolutePath}/tasks").AbsoluteUri, title: Strings.CustomGoalsButtonTitle.ToUpper(CultureInfo.CurrentCulture)));
            NavigationContext.PushPage(customGoalsPage);
        }

        async void HandleNavigateToAuthenticatedPage(Uri uri)
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var authenticatedPage = new ItemWebViewPage(new WebViewViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), uri.AbsoluteUri));
            NavigationContext.PushPage(authenticatedPage);
        }

        void HandleSetGoalReminder(object arg)
        {
            if (arg is LearnerStatsViewModel learnerStatsViewModel)
            {
                var goalView = new GoalReminderPopupPage(new GoalReminderViewModel(), learnerStatsViewModel);
                _ = PopupNavigation.Instance.PushAsync(goalView);
            }
        }

        void HandleGoToAnnotation(object arg)
        {
            if (arg is AnnotationViewModel annotationViewModel)
            {
                Application.Current.OpenUri(annotationViewModel.NodeUri);
            }
        }

        void HandleNavigateToResource(IRemoteResource resource)
        {
            switch (resource)
            {
                case IItem item:
                    HandleNavigateToItem(item);
                    break;
                case ITaxonomyTerm term:
                    HandleNavigateToTerm(term);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Navigates to a content item.
        /// </summary>
        /// <param name="item">The content item.</param>
        void HandleNavigateToItem(IItem item)
        {
            var learnerStateProvider = DependencyService.Get<ILearnerStateProvider>();

            switch (item)
            {
                case IPackagedContent packagedContent when packagedContent.PackageFile != null && Constants.OfflineAccess:
                    StartChildCoordinator(CreateArticleLauncher(packagedContent));
                    learnerStateProvider?.AddToHistory(item);
                    learnerStateProvider?.GetState(item)?.SetCompletionStatusUnknown();
                    break;
                case ICourse course:
                    DependencyService.Get<IReportingService>().ReportCourseViewed(course);
                    var deck = new CourseDeckPageViewModel(course, new DebounceCommand(HandleItemSelected, 500), new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException)));
                    var cardPage = new CourseDetailDeckPage(deck);
                    NavigationContext.PushPage(cardPage);
                    learnerStateProvider?.AddToHistory(item);
                    break;
                case IPodcast podcast:
                    if (Connectivity.NetworkAccess == NetworkAccess.None)
                    {
                        DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                        break;
                    }

                    var podViewModel = ItemToViewModelFactory.CreateViewModelFromItem(podcast, new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException)));
                    if (podViewModel is PodcastViewModel podcastViewModel)
                    {
                        var episodesViewModel = new EpisodesViewModel(podcastViewModel, new Command(HandleItemSelected), new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException)));
                        var podcastDetailPage = new PodcastDetailPage(episodesViewModel);
                        NavigationContext.PushPage(podcastDetailPage);
                    }

                    break;
                case IEpisode episode:
                    if (Connectivity.NetworkAccess == NetworkAccess.None)
                    {
                        DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                        break;
                    }

                    IList<IEpisode> mediaList;
                    if (episode.Podcast != null)
                    {
                        var episodes = new List<IEpisode>(episode.Podcast.Episodes);
                        var episodeIndex = episodes.IndexOf(episode);
                        var queue = episodes
                            .Skip(episodeIndex)
                            .Where(epi =>
                            {
                                return learnerStateProvider.GetState(epi).Completed != CorpusItemLearnerState.Status.Enabled || epi.MediaUri == episode.MediaUri;
                            })
                            .ToList();
                        mediaList = queue;
                    }
                    else
                    {
                        mediaList = new List<IEpisode>() { episode };
                    }

                    MediaPlayerViewModel playerViewModel = new MediaPlayerViewModel(mediaList);
                    var nowPlayingPage = new NowPlayingPage(playerViewModel);
                    NavigationContext.PushPage(nowPlayingPage);
                    break;
                case IDocument document:
                    HandleOpenDocumentAsync(document).OnSuccess((task) =>
                    {
                        learnerStateProvider?.AddToHistory(item);
                        learnerStateProvider?.MarkAsComplete(item);
                    });
                    break;
                default:
                    var itemViewModel = ItemToViewModelFactory.CreateViewModelFromItem(item, new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException)));

                    if (itemViewModel is TeaserViewModel teaserViewModel)
                    {
                        var webViewViewModel = new ItemWebViewViewModel(teaserViewModel, new DebounceCommand(HandleWebLinkClicked), new DebounceCommand(WebPageFailedToLoad));

                        // Link resources on Android need to be opened in a browser (in case they refer to PDF resources).
                        if (Device.RuntimePlatform == Device.Android && teaserViewModel.Type == NodeTypeConstants.LearnLink)
                        {
                            _ = Task.Run(webViewViewModel.GetAuthenticatingUrl).OnSuccess(task => ((App)Application.Current).OpenExternalUri(task.Result));
                        }
                        else if (teaserViewModel.Url.Scheme == "slar")
                        {
                            // this is special handling for native experiences
                            var reportingService = DependencyService.Get<IReportingService>();
                            var query = HttpUtility.ParseQueryString(teaserViewModel.Url.Query);

                            if (DependencyService.Get<IUnityService>() is IUnityService unityService
                                && query["gameobject"] is string gameObject
                                && query["method"] is string method
                                && query["arg"] is string arg)
                            {
                                unityService.OnCompleted(() => reportingService.ReportResourceCompleted(item));
                                unityService.OnQuit(() => { });
                                unityService.ShowUnity();
                                unityService.SendMessage(gameObject, method, arg);
                                reportingService.ReportResourceViewed(item);
                            }
                            else
                            {
                                DependencyService.Get<AlertNotificationHandler>().NotifyError(Strings.UnsupportedContentTitle, Strings.UnsupportedContentMessage);
                            }
                        }
                        else
                        {
                            var page = new ItemWebViewPage(webViewViewModel);
                            NavigationContext.ShowDetailPage(page);
                        }

                        learnerStateProvider?.AddToHistory(item);
                        learnerStateProvider?.GetState(item)?.SetCompletionStatusUnknown();
                    }
                    else
                    {
                        var searchPopUp = new SearchPopUpPage(itemViewModel);
                        _ = PopupNavigation.Instance.PushAsync(searchPopUp);
                    }

                    break;
            }
        }

        /// <summary>
        /// Navigates to a taxonomy (i.e. a tag or a topic).
        /// </summary>
        /// <param name="term">The term.</param>
        void HandleNavigateToTerm(ITaxonomyTerm term)
        {
            var termViewModel = new TermContentListViewModel(
                term,
                new Command(HandleItemSelected),
                new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException)),
                string.Format(CultureInfo.InvariantCulture, Strings.EmptyTagMessage, term.Name),
                "icon_awesome_hashtag");
            var accountListPage = new SearchableListPage(termViewModel)
            {
                ShowsLargeTitleAndAvatar = false,
            };

            DependencyService.Get<IReportingService>().ReportTermViewed(term);
            NavigationContext.PushPage(accountListPage);
        }

        async Task HandleNavigateToTerm(int termId, string termName)
        {
            ITaxonomyTerm term;

            var loadingIndicatorView = await ShowLoadingIndicator();

            try
            {
                term = await DependencyService.Get<ICorpusProvider>().GetTaxonomyTerm(termId);
            }
            catch (Exception e) when (e is DrupalCorpusProviderException || e.IsOfflineException())
            {
                // we end up here if the term doesn't have any content and/or we are offline
                // it's possible that in the future we may need to disambiguate tags vs topics, etc
                term = new Tag(termId, termName);
            }
            catch (Exception e)
            {
                DependencyService.Get<INotificationHandler>().NotifyException(e);
                return;
            }
            finally
            {
                HideLoadingIndicator(loadingIndicatorView);
            }

            if (string.IsNullOrWhiteSpace(term.Name))
            {
                DependencyService.Get<INotificationHandler>().NotifyError(Strings.EmptyUnspecifiedTagMessage);
                return;
            }

            HandleNavigateToTerm(term);
        }

        async Task<LoadingIndicatorView> ShowLoadingIndicator()
        {
            var loadingIndicatorView = new LoadingIndicatorView();

            // Catch null reference exception that happens when you start ios in landscape and discard it.
            // Not much else I saw you can do because it happens through the way its suppose to be called in onappearing.
            // Null reference seems to have to do with invoke on main thread called inside this.
            try
            {
                await PopupNavigation.Instance.PushAsync(loadingIndicatorView);
            }
            catch (NullReferenceException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            return loadingIndicatorView;
        }

        async void HideLoadingIndicator(LoadingIndicatorView loadingIndicatorView)
        {
            try
            {
                await PopupNavigation.Instance.RemovePageAsync(loadingIndicatorView);
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }
        }

        void HandleNavigateToBadge(BadgeViewModel badgeViewModel)
        {
            PopupNavigation.Instance.PushAsync(new BadgeDetailPopupPage(badgeViewModel));

            DependencyService.Get<IReportingService>().ReportBadgeViewed(badgeViewModel.Model);
        }

        void HandleNavigateToCertificate(CertificateViewModel certificateViewModel)
        {
            var detailPage = new CertificateDetailPage(certificateViewModel);
            NavigationContext.PushPage(detailPage);

            DependencyService.Get<IReportingService>().ReportCertificateViewed(certificateViewModel.Model);
        }

        void HandleNavigateToGroup(IGroup group)
        {
            var detailPage = new GroupDetailPage(
                new GroupPageViewModel(
                    group,
                    new DebounceCommand(HandleItemSelected),
                    new Command(() => NavigationContext.Reset()),
                    new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException))));
            NavigationContext.PushPage(detailPage);
        }

        void HandleNavigateToItemGroup(IItemGroup group)
        {
            var corpusProvider = DependencyService.Get<ICorpusProvider>();
            var viewModel = new SearchableContentListPageViewModel(
                group.Name,
                () => corpusProvider.GetNodeList(group.Url),
                new DebounceCommand(HandleItemSelected),
                new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException)),
                Strings.EmptyGroupsMessage);

            var searchable = new SearchableListPage(viewModel)
            {
                ShowsLargeTitleAndAvatar = false,
            };

            NavigationContext.PushPage(searchable);
        }

        void HandleShareCertificate(CertificateSharingViewModel certificateSharingViewModel)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var request = new ShareFileRequest
                {
                    File = new ShareFile(certificateSharingViewModel.CertificateFile.LocalPath),
                    PresentationSourceBounds = new System.Drawing.Rectangle(10, 10, 100, 100),
                    Title = certificateSharingViewModel.Model.CertificateName,
                };

                if (Device.RuntimePlatform == Device.iOS)
                {
                    // the Essentials plugin always attaches the share sheet to the topmost view controller
                    // in our case, that is a loading indicator that is dismissed immediately
                    await DependencyService.Get<IShareService>().RequestAsync(request).ConfigureAwait(false);
                }
                else
                {
                    await Share.RequestAsync(request).ConfigureAwait(false);
                }
            });

            DependencyService.Get<IReportingService>().ReportCertificateShared(certificateSharingViewModel.Model);
        }

        void HandleWebLinkClicked(object arg)
        {
            if (arg is Uri uri)
            {
                // Special case!! if they click on Suggestion that means they've submitted a suggestion.
                if (uri.LocalPath == Constants.SuggestionsPath)
                {
                    DismissModal();
                    return;
                }

                Application.Current.OpenUri(uri);
            }
        }

        void HandleNavigateToDiscussion(Uri uri)
        {
            var viewModel = new PopupWebViewViewModel(
                new DebounceCommand(HandleWebLinkClicked),
                new DebounceCommand(WebPageFailedToLoad),
                uri.OriginalString,
                new DebounceCommand(HandleClosePopupSelected))
            {
                Title = Strings.CommentsTitleLabel,
            };

            var webView = new WebViewPopupPage(viewModel);
            PopupNavigation.Instance.PushAsync(webView);
        }

        void HandleNavigateToPrompts()
        {
            UserDialogs.Instance.ShowLoading();
            var learnerProvider = DependencyService.Get<ILearnerProvider>();
            learnerProvider.GetPrompts().ContinueWith((task) =>
            {
                UserDialogs.Instance.HideLoading();

                if (task.Exception != null)
                {
                    DependencyService.Get<INotificationHandler>().NotifyException(task.Exception);
                    return;
                }

                var prompts = new PromptCollectionViewModel(task.Result);
                var popupPage = new PopupStackOfCardsCard(prompts, new DebounceCommand(HandleRefreshMainPage));
                PopupNavigation.Instance.PushAsync(popupPage);
            });
        }

        void HandleRefreshMainPage()
        {
            if (Shell.Current?.CurrentPage is not EnhancedDashboardPage enhancedDashboardPage)
            {
                return;
            }

            enhancedDashboardPage.Refresh();
        }

        async void HandleNavigateToHistory()
        {
            await Shell.Current.GoToAsync("///history");
        }

        async void HandleNavigateToBookmarks()
        {
            await Shell.Current.GoToAsync("///bookmarks");
        }

        async void HandleNavigateToGroups()
        {
            await Shell.Current.GoToAsync("///groups");
        }

        void HandleNavigateToFollowing()
        {
            var downloadItemCommand = new AsyncCommand<IItem>(HandleDownloadLocalContent, new Command<Exception>(HandleDownloadException));
            var viewModel = new FollowingViewModel(new DebounceCommand(HandleItemSelected, 500), downloadItemCommand);
            var page = new FollowingPage()
            {
                BindingContext = viewModel,
                Title = Strings.TabFollowingLabel,
                ShowingFromEnhanced = true,
            };
            NavigationContext.PushPage(page);
        }

        void HandleNavigateToVidyoRoom(Uri vidyoUri)
        {
            var context = DependencyService.Get<IAppContextService>();
            var groupsPage = new ItemWebViewPage(new WebViewViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), vidyoUri.AbsoluteUri));
            NavigationContext.PushPage(groupsPage);
        }

        async Task HandleNextArticle(Uri uri)
        {
            var previousPage = Shell.Current.CurrentPage;
            await Application.Current.OpenUriAsync(uri).ConfigureAwait(false);

            // remove previous article from nav stack
            await Device.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.RemovePage(previousPage);
            }).ConfigureAwait(false);
        }

        async void HandleNavigateToEditGoal(Uri editGoalUri)
        {
            var goalToEditString = editGoalUri.Fragment.Replace("#", string.Empty);

            if (!Enum.TryParse(goalToEditString, true, out GoalType goalToEdit))
            {
                DependencyService.Get<AlertNotificationHandler>().NotifyError(Strings.ServerErrorMessage);
                return;
            }

            var context = DependencyService.Get<IAppContextService>();
            var learnerGoals = context.CurrentLearner.LearnerGoals;
            var learnerStats = await DependencyService.Get<ILearnerProvider>().GetCurrentLearnerStats();

            int count = 0;
            int goal = 0;

            switch (goalToEdit)
            {
                case GoalType.ViewArticlesPerWeek:
                    count = learnerStats?.SeenWeekCount ?? 0;
                    goal = learnerGoals.ArticlesViewedPerWeekGoal;
                    break;
                case GoalType.CompleteArticlesPerWeek:
                    count = learnerStats?.CompletedWeekCount ?? 0;
                    goal = learnerGoals.ArticlesCompletedPerWeekGoal;
                    break;
                case GoalType.CompleteCoursesPerMonth:
                    count = learnerStats?.CompletedCourseMonthCount ?? 0;
                    goal = learnerGoals.CoursesCompletedPerMonthGoal;
                    break;
                case GoalType.AverageTestScore:
                    count = learnerStats?.TestWeekAverage ?? 0;
                    goal = learnerGoals.AverageTestScoreGoal;
                    break;
            }

            var learnerGoalViewModel = new LearnerGoalViewModel(new Goal(goalToEdit, count, goal), new Command(HandleAdjustGoal));
            HandleAdjustGoal(learnerGoalViewModel);
        }

        void WebPageFailedToLoad(object obj)
        {
            // Delay because if you pop to fast after a push it causes a crash
            Task.Delay(500).ContinueWith(
            (arg) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (PopupNavigation.Instance.PopupStack.Any())
                    {
                        PopupNavigation.Instance.PopAsync();
                    }
                    else
                    {
                        NavigationContext.PopPage(true);
                    }
                });
            }, TaskScheduler.Current);
        }

        async Task HandleDownloadLocalContent(IItem item)
        {
            if (item is IPackagedContent content)
            {
                var downloadService = DependencyService.Get<IDownloaderService>();

                // if some other context downloaded the file first, just update properties and bail
                if (content.PackageFile.IsDownloaded)
                {
                    return;
                }

                try
                {
                    await downloadService.DownloadItemToPath(content);
                }
                catch (DownloadException e) when (e.InnerException is TimeoutException && e.InnerException.InnerException is TaskCanceledException)
                {
                    // this occurs when the download is canceled
                }

                DependencyService.Get<IOfflineContentService>().SetContentOrigin(content, Initiator.User);
            }
        }

        void HandleDeleteLocalContent(IDownloadable item)
        {
            try
            {
                DependencyService.Get<IDownloaderService>().RemoveDownloadedItem(item);
                DependencyService.Get<IOfflineContentService>().SetRemovalOrigin(item, Initiator.User);
            }
            catch (Exception e)
            {
                DependencyService.Get<AlertNotificationHandler>().NotifyException(e);
            }
        }

        void HandleDeleteException(Exception e)
        {
            DependencyService.Get<AlertNotificationHandler>().NotifyException(e);
        }

        void HandleDownloadException(Exception e)
        {
            DependencyService.Get<INotificationHandler>().NotifyException(e, Strings.DownloadError);
        }

        void DismissModal()
        {
            NavigationContext.DismissPage(true);
        }

        void HandleClosePopupSelected()
        {
            PopupNavigation.Instance.PopAsync();
        }

        async Task HandleOpenFileAsync(IFile remoteFile)
        {
            var downloaderService = DependencyService.Get<IDownloaderService>();

            try
            {
                // This is to ensure that on Android devices, documents are opened separately from the main thread.
                // Otherwise, canceling the download request will result in an Android.OS.NetworkOnMainThreadException.
                await Task.Run(() =>
                {
                    return OpenFileAsync(remoteFile, (status) => downloaderService.DownloadFileToPath(remoteFile, status));
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DependencyService.Get<INotificationHandler>().NotifyException(e, Strings.DefaultErrorTitle);
            }
        }

        async Task HandleOpenDocumentAsync(IDocument document)
        {
            // Because of the way the app is opening the document (i.e. as a file) we should also report the resource as viewed beforehand.
            DependencyService.Get<IReportingService>().ReportResourceViewed(document);
            var downloaderService = DependencyService.Get<IDownloaderService>();

            try
            {
                // This is to ensure that on Android devices, documents are opened separately from the main thread.
                // Otherwise, canceling the download request will result in an Android.OS.NetworkOnMainThreadException.
                await Task.Run(() =>
                {
                    return OpenFileAsync(document.File, (status) => downloaderService.DownloadItemToPath(document, status));
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DependencyService.Get<INotificationHandler>().NotifyException(e, Strings.DefaultErrorTitle);
            }
        }

        async Task OpenFileAsync(IFile remoteFile, Func<DownloadStatus, Task> downloadTask)
        {
            if (remoteFile.IsDownloaded)
            {
                LaunchDocumentLauncher(remoteFile);
                return;
            }

            var downloadStatus = new DownloadStatus(remoteFile.Name);
            var config = new ProgressDialogConfig
            {
                Title = ProgressDialogConfig.DefaultTitle,
                IsDeterministic = true,
            };
            _ = config.SetCancel(ProgressDialogConfig.DefaultCancelText, () =>
            {
                if (downloadStatus.State != DownloadStatus.DownloadState.Cancelled)
                {
                    downloadStatus.CancelDownload();
                }
            });

            var loadingDialog = UserDialogs.Instance.Progress(config);
            loadingDialog.Show();
            downloadStatus.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                var percent = ((DownloadStatus)sender).PercentComplete * 100;
                Device.BeginInvokeOnMainThread(() => { loadingDialog.PercentComplete = (int)percent; });
            };

            try
            {
                var downloaderService = DependencyService.Get<IDownloaderService>();
                await downloadTask(downloadStatus).ConfigureAwait(false);
                DependencyService.Get<IReportingService>()?.ReportFileDownloaded(remoteFile);
                LaunchDocumentLauncher(remoteFile);
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);

                if (e.GetBaseException() is OperationCanceledException)
                {
                    return;
                }

                throw;
            }
            finally
            {
                loadingDialog.Hide();
                loadingDialog.Dispose();
            }
        }

        void LaunchDocumentLauncher(IFile remoteFile)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                var opener = DependencyService.Get<IDocumentOpener>();
                await opener.OpenAsync(remoteFile).ConfigureAwait(false);
                DependencyService.Get<IReportingService>()?.ReportFileViewed(remoteFile);
            }).OnFailure(task =>
            {
                DependencyService.Get<INotificationHandler>().NotifyException(task.Exception);
            });
        }

        ICoordinator CreateArticleLauncher(IPackagedContent packagedContent)
        {
            if (packagedContent == null)
            {
                throw new ArgumentNullException(nameof(packagedContent));
            }

            var activityLauncher = new PackageLaunchCoordinator(packagedContent, new DebounceCommand(HandleWebLinkClicked), new DebounceCommand(WebPageFailedToLoad));

            return activityLauncher;
        }
    }
}
