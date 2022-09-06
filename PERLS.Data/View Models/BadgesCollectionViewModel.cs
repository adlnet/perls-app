using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Extensions;
using Float.Core.Net;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The badges collection view model.
    /// </summary>
    public class BadgesCollectionViewModel : BasePageViewModel, IEmptyCollectionViewModel
    {
        readonly Func<Task<IEnumerable<IBadge>>> loadingDataTask;
        IEnumerable<BadgeViewModel> allBadges = new List<BadgeViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BadgesCollectionViewModel"/> class.
        /// </summary>
        /// <param name="loadDataFunction">The function for reloading data.</param>
        /// <param name="selectItemCommand">The select item command.</param>
        public BadgesCollectionViewModel(Func<Task<IEnumerable<IBadge>>> loadDataFunction, ICommand selectItemCommand) : base()
        {
            SelectBadgeCommand = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
            loadingDataTask = loadDataFunction ?? throw new ArgumentNullException(nameof(loadDataFunction));
            Title = Strings.TabMyContentLabel;
            Refresh();
        }

        /// <summary>
        /// Gets the command to invoke when an item is selected.
        /// </summary>
        /// <value>The selected command.</value>
        public ICommand SelectBadgeCommand { get; }

        /// <summary>
        /// Gets the badges.
        /// </summary>
        /// <value>
        /// The badges.
        /// </value>
        public IEnumerable<BadgeViewModel> Badges => allBadges
            .Where(vm => vm.IsVisible)
            .OrderByDescending(vm => vm.IsUnlocked)
            .ThenBy(vm => vm.Label);

        /// <summary>
        /// Gets a value indicating whether there was an error loading the badges.
        /// </summary>
        /// <value>
        /// <c>true</c> if there was an error, <c>false</c> otherwise.
        /// </value>
        public bool IsError => Error != null;

        /// <inheritdoc />
        public string EmptyLabel => IsError ? DependencyService.Get<INotificationHandler>()?.FormatException(Error) : Strings.BadgesEmptyMessage;

        /// <inheritdoc />
        public string EmptyMessageTitle => IsError ? Strings.EmptyViewErrorTitle : string.Empty;

        /// <inheritdoc />
        public string EmptyImageName => IsError ? "error" : "resource://PERLS.Data.Resources.badge_placeholder.svg?Assembly=PERLS.Data";

        /// <inheritdoc/>
        public override void Refresh()
        {
            base.Refresh();

            if (IsLoading)
            {
                return;
            }

            GetBadges().ContinueWith(
                task =>
                {
                    if (task.Exception is AggregateException aggregateException)
                    {
                        ContainsCachedData = task.Exception.InnerException.IsOfflineException();
                        Error = aggregateException;
                        return;
                    }

                    ContainsCachedData = task.Exception != null && task.Exception.IsOfflineException();
                    Error = task.Exception;
                }, TaskScheduler.Current);
        }

        async Task GetBadges()
        {
            if (IsError || Badges == null)
            {
                IsLoading = true;
            }

            try
            {
                var badges = await loadingDataTask().ConfigureAwait(false);
                allBadges = badges.Select((arg) => new BadgeViewModel(arg));
                OnPropertyChanged(nameof(Badges));

                // Ideally, it would be easier to know if the response we got from the provider was cached.
                if (!await DependencyService.Get<INetworkConnectionService>().IsReachable().ConfigureAwait(false))
                {
                    throw new HttpConnectionException(Strings.OfflineMessageBody.AddAppName());
                }

                Error = null;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
