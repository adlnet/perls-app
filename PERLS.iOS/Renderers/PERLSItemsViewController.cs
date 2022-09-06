using System;
using System.Linq;
using CoreGraphics;
using Float.Core.Analytics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Custom controller for retrieving collection view cells.
    /// </summary>
    /// <typeparam name="TItemsView">The type of cell.</typeparam>
    internal class PERLSItemsViewController<TItemsView> : GroupableItemsViewController<TItemsView> where TItemsView : GroupableItemsView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PERLSItemsViewController{TItemsView}"/> class.
        /// </summary>
        /// <param name="groupableItemsView">The items view.</param>
        /// <param name="layout">The desired layout.</param>
        public PERLSItemsViewController(TItemsView groupableItemsView, ItemsViewLayout layout) : base(groupableItemsView, layout)
        {
            CollectionView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
        }

        /// <inheritdoc />
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell = null;

            // this is a workaround for #SL-2557
            try
            {
                cell = base.GetCell(collectionView, indexPath);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
#pragma warning disable CS0162 // Unreachable code detected
                if (Constants.Configuration == BuildConfiguration.Release)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to propagate GetCell to base. {e}");
                }
#pragma warning restore CS0162 // Unreachable code detected
            }

            if (cell?.ContentView?.Subviews?.FirstOrDefault() is UIView contents)
            {
                FixBounds(contents);
            }

            if (cell is UICollectionViewCell theCell)
            {
                // Workaround for an issue in Xamarin Forms. At the TemplateCell.cs:131, an
                // if statement checks the color of the SelectedBackgroundView. However, we used
                // set SelectedBackgroundView to null, and since there is no null check, recycled
                // cells would result in a null reference within Xamarin code.
                // See ticket SL-2697 for info about this bug.
                theCell.SelectedBackgroundView = new UIView
                {
                    BackgroundColor = UIColor.Clear,
                };
            }

            return cell;
        }

        /// <inheritdoc />
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (CollectionView.ViewWithTag(ItemsViewTags.HeaderTag) is UIView header)
            {
                FixBounds(header);
            }

            if (CollectionView.ViewWithTag(ItemsViewTags.FooterTag) is UIView footer)
            {
                FixBounds(footer);
            }

            if (CollectionView.ViewWithTag(EmptyTag) is UIView empty)
            {
                FixBounds(empty);
            }
        }

        /// <summary>
        /// Fixes the bounds of the view so its children are not inset.
        /// </summary>
        /// <param name="view">The view to fix the bounds on.</param>
        /// <remarks>
        /// There is an extremely subtle bug in CollectionView when a templated cell or header/footer/empty view
        /// uses the DefaultRenderer (i.e. ContentView, Grid, StackLayout, etc.) -- the bounds X,Y will be
        /// set to -1,-1 resulting in the contents of the view to be inset by 1x1. This ends up looking like a single
        /// border pixel along the top and left.
        /// </remarks>
        void FixBounds(UIView view)
        {
            if (view.Bounds.X != -1 || view.Bounds.Y != -1)
            {
                return;
            }

            var bounds = view.Bounds;
            bounds.Location = CGPoint.Empty;
            view.Bounds = bounds;
        }
    }
}
