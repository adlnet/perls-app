using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using PERLS.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CollectionView), typeof(DefaultCollectionViewRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Custom renderer for collection views.
    /// </summary>
    public class DefaultCollectionViewRenderer : CollectionViewRenderer
    {
        /// <inheritdoc />
        protected override GroupableItemsViewController<GroupableItemsView> CreateController(GroupableItemsView itemsView, ItemsViewLayout layout)
        {
            if (itemsView == null)
            {
                return null;
            }

            return new PERLSItemsViewController<GroupableItemsView>(itemsView, layout);
        }

        /// <inheritdoc />
        protected override ItemsViewLayout SelectLayout()
        {
            if (ItemsView.ItemsLayout is LinearItemsLayout linearItemsLayout && linearItemsLayout.SnapPointsType != SnapPointsType.None)
            {
                return new PERLSItemsViewLayout(linearItemsLayout, ItemsView.ItemSizingStrategy);
            }
            else if (ItemsView.ItemsLayout is GridItemsLayout gridItemsLayout)
            {
                return new PERLSGridLayout(gridItemsLayout, ItemsView.ItemSizingStrategy);
            }

            return base.SelectLayout();
        }

        /// <inheritdoc/>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            if (Element == null || Element.ItemsSource == null)
            {
                return;
            }

            base.OnElementPropertyChanged(sender, changedProperty);

            if (changedProperty?.PropertyName is string property
                && (property == nameof(CollectionView.Header)
                || property == nameof(CollectionView.Footer)
                || property == nameof(CollectionView.EmptyView)))
            {
                CheckForMirroring();
            }
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);

            if (e == null || Control == null)
            {
                return;
            }

            CheckForMirroring();
        }

        void CheckForMirroring()
        {
            // Xamarin Bug Alert! EmptyViews and Headers are mirrored rather than right-to-left.
            // Github issue: https://github.com/xamarin/Xamarin.Forms/issues/13873
            if (Control.PreferredFocusEnvironments.FirstOrDefault() is UICollectionView uicollectionView && Element is CollectionView collectionView)
            {
                if (uicollectionView.EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft)
                {
                    if (collectionView.EmptyView is View emptyView)
                    {
                        emptyView.RotationY = -180;
                    }

                    if (collectionView.Header is View headerView)
                    {
                        headerView.RotationY = -180;
                    }

                    if (collectionView.Footer is View footerView)
                    {
                        footerView.RotationY = -180;
                    }
                }
            }
        }

        /// <summary>
        /// Application-specific adjustments for the grid layout.
        /// </summary>
        internal class PERLSGridLayout : GridViewLayout
        {
            readonly GridItemsLayout itemsLayout;

            /// <summary>
            /// Initializes a new instance of the <see cref="PERLSGridLayout"/> class.
            /// </summary>
            /// <param name="itemsLayout">The items layout.</param>
            /// <param name="itemSizingStrategy">The items sizing strategy.</param>
            public PERLSGridLayout(GridItemsLayout itemsLayout, ItemSizingStrategy itemSizingStrategy) : base(itemsLayout, itemSizingStrategy)
            {
                this.itemsLayout = itemsLayout;
            }

            /// <inheritdoc />
            public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
            {
                var inset = base.GetInsetForSection(collectionView, layout, section);

                // When spacing is specified on the layout, the footer will be spaced from the last element by that amount
                // but the header view will not be. Android behaves in the opposite way. To ensure consistent spacing across
                // platforms, this adjusts the left inset when there is a header so that it is spaced from the first element.
                if (CollectionView.ViewWithTag(ItemsViewTags.HeaderTag) is UIView)
                {
                    if (itemsLayout.Orientation == ItemsLayoutOrientation.Horizontal)
                    {
                        inset.Left = (float)itemsLayout.HorizontalItemSpacing;
                    }
                    else
                    {
                        inset.Top = (float)itemsLayout.VerticalItemSpacing;
                    }
                }

                return inset;
            }
        }

        /// <summary>
        /// Application-specific adjustments for the linear layout.
        /// </summary>
        internal class PERLSItemsViewLayout : ListViewLayout
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PERLSItemsViewLayout"/> class.
            /// </summary>
            /// <param name="itemsLayout">The items layout.</param>
            /// <param name="itemSizingStrategy">The items sizing strategy.</param>
            public PERLSItemsViewLayout(LinearItemsLayout itemsLayout, ItemSizingStrategy itemSizingStrategy) : base(itemsLayout, itemSizingStrategy)
            {
            }

            /// <inheritdoc />
            public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
            {
                if (CollectionView.ViewWithTag(ItemsViewTags.HeaderTag) is UIView header)
                {
                    if (proposedContentOffset.X < (header.Frame.Width * -0.5))
                    {
                        return new CGPoint(-header.Frame.Width, 0);
                    }
                }

                return base.TargetContentOffset(proposedContentOffset, scrollingVelocity);
            }
        }
    }
}
