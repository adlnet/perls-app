using System.Linq;
using Float.Core.Extensions;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(PERLS.iOS.Renderers.DefaultShellRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Customizes the appearance of the tabs at the top of the section root.
    /// </summary>
    internal class PERLSShellSectionRootHeader : XamarinShellSectionRootHeader
    {
        readonly UICollectionViewFlowLayout collectionViewFlowLayout;
        Color selectedForegroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PERLSShellSectionRootHeader"/> class.
        /// </summary>
        /// <param name="shellContext">The shell context.</param>
        /// <param name="collectionViewFlowLayout">The collection view flow layout.</param>
        public PERLSShellSectionRootHeader(IShellContext shellContext, UICollectionViewFlowLayout collectionViewFlowLayout) : base(shellContext, collectionViewFlowLayout)
        {
            if (collectionViewFlowLayout == null)
            {
                this.collectionViewFlowLayout = new UICollectionViewFlowLayout();
            }
            else
            {
                this.collectionViewFlowLayout = collectionViewFlowLayout;
            }
        }

        /// <inheritdoc />
        public override UICollectionViewLayout Layout => collectionViewFlowLayout;

        /// <inheritdoc />
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (Layout is UICollectionViewFlowLayout layout)
            {
                layout.SectionInset = new UIEdgeInsets(5, 15, 0, 15);
            }

            CollectionView.Bounces = true;

            // Hides the selection indicator
            // "9001" is the zposition set by Xamarin.Forms and can be used to identify the view
            // https://github.com/xamarin/Xamarin.Forms/blob/f9114b1306f2896cce07d358725f63ce6ab8cac5/Xamarin.Forms.Platform.iOS/Renderers/ShellSectionRootHeader.cs#L153
            var bar = CollectionView.Subviews.FirstOrDefault(v => v.Layer.ZPosition == 9001);
            if (bar != null)
            {
                bar.BackgroundColor = UIColor.Clear;
            }

            // "9002" is the zposition set by Xamarin.Forms and can be used to identify the view
            // https://github.com/xamarin/Xamarin.Forms/blob/f9114b1306f2896cce07d358725f63ce6ab8cac5/Xamarin.Forms.Platform.iOS/Renderers/ShellSectionRootHeader.cs#L158
            var shadow = CollectionView.Subviews.FirstOrDefault(v => v.Layer.ZPosition == 9002);
            if (shadow != null)
            {
                shadow.Hidden = true;
            }
        }

        /// <inheritdoc />
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var selectedItems = collectionView.GetIndexPathsForSelectedItems();
            if (selectedItems.Length == 0)
            {
                UpdateSelectedIndex();
                selectedItems = collectionView.GetIndexPathsForSelectedItems();
            }

            var cell = base.GetCell(collectionView, indexPath);

            if (cell is ShellSectionHeaderCell headerCell)
            {
                headerCell.Label.Text = headerCell.Label.Text?.ToUpperInvariant();
                headerCell.Label.SetNeedsDisplay();
            }

            cell.Layer.CornerRadius = 10;

            if (selectedItems.Length > 0 && selectedItems[0].Row == indexPath.Row)
            {
                SetCellAppearanceAsSelected(cell);
            }
            else
            {
                SetCellAppearanceAsDeselected(cell);
            }

            return cell;
        }

        /// <inheritdoc />
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemSelected(collectionView, indexPath);

            var cell = collectionView.CellForItem(indexPath);
            if (cell != null)
            {
                SetCellAppearanceAsSelected(cell);
            }
        }

        /// <inheritdoc />
        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemDeselected(collectionView, indexPath);

            var cell = collectionView.CellForItem(indexPath);
            if (cell != null)
            {
                SetCellAppearanceAsDeselected(cell);
            }
        }

        /// <inheritdoc />
        protected override void SetAppearance(ShellAppearance appearance)
        {
            base.SetAppearance(appearance);

            CollectionView.BackgroundColor = appearance.BackgroundColor.ToUIColor();

            // Use the Shell background color for the selected foreground color
            // so the label of the selected tab looks like the background.
            selectedForegroundColor = appearance.BackgroundColor;
        }

        /// <inheritdoc />
        protected override void UpdateSelectedIndex(bool animated = false)
        {
            base.UpdateSelectedIndex(animated);

            // we have to manually update cell selection appearance for programmatic selection

            // deselect visible cells
            CollectionView
                .VisibleCells
                .ForEach(SetCellAppearanceAsDeselected);

            // update style on selected cells
            CollectionView
                .GetIndexPathsForSelectedItems()
                .Select(CollectionView.CellForItem)
                .ForEach(SetCellAppearanceAsSelected);
        }

        /// <summary>
        /// Updates a cell's appearance so that it appears selected.
        /// </summary>
        /// <param name="cell">The cell that was selected.</param>
        protected virtual void SetCellAppearanceAsSelected(UICollectionViewCell cell)
        {
            if (cell == null)
            {
                return;
            }

            var selectedColor = App.Current.Color("SecondaryColor");
            cell.BackgroundColor = selectedColor.ToUIColor();

            if (cell is ShellSectionHeaderCell headerCell)
            {
                headerCell.Label.TextColor = selectedForegroundColor.ToUIColor();
            }
        }

        /// <summary>
        /// Updates a cell's appearance so that it appears deselected.
        /// </summary>
        /// <param name="cell">The cell that was deselected.</param>
        protected virtual void SetCellAppearanceAsDeselected(UICollectionViewCell cell)
        {
            if (cell == null)
            {
                return;
            }

            cell.BackgroundColor = CollectionView.BackgroundColor;
        }
    }
}
