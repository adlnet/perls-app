using Android.Content;
using AndroidX.RecyclerView.Widget;
using PERLS.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CollectionView), typeof(DefaultCollectionViewRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Default renderer for collection views.
    /// </summary>
    /// <remarks>
    /// Adjusts the spacing on heading and footer views.
    /// </remarks>
    public class DefaultCollectionViewRenderer : CollectionViewRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCollectionViewRenderer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DefaultCollectionViewRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override ItemDecoration CreateSpacingDecoration(IItemsLayout itemsLayout)
        {
            return new FixedSpacingItemDecoration(itemsLayout);
        }

        /// <summary>
        /// Application-specific adjustments to the spacing on collection views.
        /// </summary>
        /// <remarks>
        /// When spacing is specified on a CollectionView layout, there will be TWICE that space between
        /// the header and first element and NO space between the last element and the footer. iOS works
        /// in (almost) the opposite way. To ensure consistent spacing across both platforms, this adjusts
        /// the spacing so that the header and footer are evenly spaced from the contents.
        /// </remarks>
        internal class FixedSpacingItemDecoration : SpacingItemDecoration
        {
            readonly ItemsLayoutOrientation orientation;
            readonly double horizontalSpacing;
            readonly double verticalSpacing;

            public FixedSpacingItemDecoration(IItemsLayout itemsLayout) : base(itemsLayout)
            {
                if (itemsLayout is GridItemsLayout gridItemsLayout)
                {
                    orientation = gridItemsLayout.Orientation;
                    horizontalSpacing = gridItemsLayout.HorizontalItemSpacing;
                    verticalSpacing = gridItemsLayout.VerticalItemSpacing;
                }
                else if (itemsLayout is LinearItemsLayout linearItemsLayout)
                {
                    orientation = linearItemsLayout.Orientation;
                    horizontalSpacing = orientation == ItemsLayoutOrientation.Horizontal ? linearItemsLayout.ItemSpacing : 0;
                    verticalSpacing = orientation == ItemsLayoutOrientation.Vertical ? linearItemsLayout.ItemSpacing : 0;
                }
            }

            /// <inheritdoc />
            public override void GetItemOffsets(Android.Graphics.Rect outRect, Android.Views.View view, RecyclerView parent, State state)
            {
                base.GetItemOffsets(outRect, view, parent, state);

                // Adjusts the spacing around the header and footer so that headers
                // so that between the header and footer, there is the configured spacing.
                switch (parent.GetChildViewHolder(view).ItemViewType)
                {
                    case ItemViewType.Header:
                        outRect.Bottom = 0;
                        outRect.Right = 0;
                        break;
                    case ItemViewType.Footer:
                        outRect.Left = (int)parent.Context.ToPixels(horizontalSpacing);
                        outRect.Top = (int)parent.Context.ToPixels(verticalSpacing);
                        break;
                }
            }
        }
    }
}
