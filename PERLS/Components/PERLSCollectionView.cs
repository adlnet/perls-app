using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Float.Core.Analytics;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// Shim to fix bugs/oddities in CollectionView. By default, this should be used throughout the app.
    /// </summary>
    public class PERLSCollectionView : CollectionView
    {
        INotifyCollectionChanged observedSource;
        ItemsViewScrolledEventArgs lastScrolledEvent;

        /// <inheritdoc />
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // this is a workaround for #SL-2557
            try
            {
                base.OnPropertyChanged(propertyName);
            }
            catch (Exception e)
            {
#pragma warning disable CS0162 // Unreachable code detected
                if (Constants.Configuration == BuildConfiguration.Release)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to propagate OnPropertyChanged to base. {e}");
                }
#pragma warning restore CS0162 // Unreachable code detected
            }

            if (propertyName == nameof(ItemsSource))
            {
                ObserveItemsSource(ItemsSource);
            }
        }

        /// <inheritdoc />
        protected override void OnScrolled(ItemsViewScrolledEventArgs e)
        {
            base.OnScrolled(e);
            ((App)Application.Current).InteractivityHelper.ResetIdleTimer();
            lastScrolledEvent = e;
        }

        void ObserveItemsSource(IEnumerable source)
        {
            if (observedSource != null)
            {
                observedSource.CollectionChanged -= HandleCollectionChanged;
            }

            if (source is INotifyCollectionChanged liveCollection)
            {
                liveCollection.CollectionChanged += HandleCollectionChanged;
                observedSource = liveCollection;
            }
        }

        /// <summary>
        /// Responds to changes to the item source when it implements <see cref="INotifyCollectionChanged"/>.
        /// </summary>
        /// <param name="sender">The collection that changed.</param>
        /// <param name="e">Information about the change.</param>
        void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // If the view has never been scrolled, then this is perhaps the first time
                // this view has data to show. In which case, for consistency sake,
                // notify any listeners that the first item is scrolled into view.
                if (lastScrolledEvent == null)
                {
                    var scrollEvent = new ItemsViewScrolledEventArgs
                    {
                        FirstVisibleItemIndex = 0,
                        CenterItemIndex = 0,
                        LastVisibleItemIndex = 1,
                    };
                    SendScrolled(scrollEvent);
                }
            });
        }
    }
}
