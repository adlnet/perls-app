using System;
using System.Collections.Specialized;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// A (hopefully temporary) recreation and adaptation of Xamarin's SectionRootHeader, with changes to one line.
    /// https://github.com/xamarin/Xamarin.Forms/blob/caab66bcf9614aca0c0805d560a34e176d196e17/Xamarin.Forms.Platform.iOS/Renderers/ShellSectionRootHeader.cs#L69.
    /// </summary>
    public class XamarinShellSectionRootHeader : UICollectionViewController, IAppearanceObserver, IShellSectionRootHeader
    {
        static readonly NSString CellId = new NSString("HeaderCell");
        readonly IShellContext shellContext;
        Color defaultBackgroundColor = new Color(0.964);
        Color defaultForegroundColor = Color.Black;
        Color defaultUnselectedColor = Color.Black.MultiplyAlpha(0.7);
        UIView bar;
        UIView bottomShadow;
        Color selectedColor;
        Color unselectedColor;
        bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamarinShellSectionRootHeader"/> class.
        /// </summary>
        /// <param name="shellContext">The context.</param>
        /// <param name="flowLayout">The flow layout.</param>
        public XamarinShellSectionRootHeader(IShellContext shellContext, UICollectionViewFlowLayout flowLayout) : base(flowLayout)
        {
            this.shellContext = shellContext;
        }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        /// <value>
        /// The selected index.
        /// </value>
        public double SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the shell section.
        /// </summary>
        /// <value>
        /// The shell section.
        /// </value>
        public ShellSection ShellSection { get; set; }

        /// <summary>
        /// Gets the view controller.
        /// </summary>
        /// <value>
        /// The view controller.
        /// </value>
        public UIViewController ViewController => this;

        IShellSectionController ShellSectionController => ShellSection;

        /// <inheritdoc/>
        public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return false;
        }

        /// <inheritdoc/>
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (collectionView == null)
            {
                throw new ArgumentNullException(nameof(collectionView));
            }

            if (indexPath == null)
            {
                throw new ArgumentNullException(nameof(indexPath));
            }

            var reusedCell = (UICollectionViewCell)collectionView.DequeueReusableCell(CellId, indexPath);
            var headerCell = reusedCell as ShellSectionHeaderCell;

            if (headerCell == null)
            {
                return reusedCell;
            }

            var selectedItems = collectionView.GetIndexPathsForSelectedItems();

            var shellContent = ShellSectionController.GetItems()[indexPath.Row];
            headerCell.Label.Text = shellContent.Title;
            headerCell.Label.SetNeedsDisplay();

            headerCell.SelectedColor = selectedColor.ToUIColor();
            headerCell.UnSelectedColor = unselectedColor.ToUIColor();

            if (selectedItems.Length > 0 && selectedItems[0].Row == indexPath.Row)
            {
                headerCell.Selected = true;
            }
            else
            {
                headerCell.Selected = false;
            }

            headerCell.SetAccessibilityProperties(shellContent);
            return headerCell;
        }

        /// <inheritdoc/>
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return ShellSectionController.GetItems().Count;
        }

        /// <inheritdoc/>
        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (CollectionView.CellForItem(indexPath) is ShellSectionHeaderCell cell)
            {
                cell.Label.TextColor = unselectedColor.ToUIColor();
            }
        }

        /// <inheritdoc/>
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath == null)
            {
                return;
            }

            var row = indexPath.Row;

            var item = ShellSectionController.GetItems()[row];

            if (item != ShellSection.CurrentItem)
            {
                ShellSection.SetValueFromRenderer(ShellSection.CurrentItemProperty, item);
            }

            if (CollectionView.CellForItem(indexPath) is ShellSectionHeaderCell cell)
            {
                cell.Label.TextColor = selectedColor.ToUIColor();
            }
        }

        /// <inheritdoc/>
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        /// <inheritdoc/>
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath == null)
            {
                return false;
            }

            var row = indexPath.Row;
            var item = ShellSectionController.GetItems()[row];
            IShellController shellController = shellContext.Shell;

            if (item == ShellSection.CurrentItem)
            {
                return true;
            }

            return shellController.ProposeNavigation(ShellNavigationSource.ShellContentChanged, (ShellItem)ShellSection.Parent, ShellSection, item, ShellSection.Stack, true);
        }

        /// <inheritdoc/>
        public override void ViewDidLayoutSubviews()
        {
            if (isDisposed)
            {
                return;
            }

            base.ViewDidLayoutSubviews();

            LayoutBar();

            bottomShadow.Frame = new CGRect(0, CollectionView.Frame.Bottom, CollectionView.Frame.Width, 0.5);
        }

        /// <inheritdoc/>
        public override void ViewDidLoad()
        {
            if (isDisposed)
            {
                return;
            }

            base.ViewDidLoad();

            CollectionView.ScrollsToTop = false;
            CollectionView.Bounces = false;
            CollectionView.AlwaysBounceHorizontal = false;
            CollectionView.ShowsHorizontalScrollIndicator = false;
            CollectionView.ClipsToBounds = false;

            bar = new UIView(new CGRect(0, 0, 20, 20));
            bar.BackgroundColor = UIColor.White;
            bar.Layer.ZPosition = 9001;
            CollectionView.AddSubview(bar);

            bottomShadow = new UIView(new CGRect(0, 0, 10, 1));
            bottomShadow.BackgroundColor = Color.Black.MultiplyAlpha(0.3).ToUIColor();
            bottomShadow.Layer.ZPosition = 9002;
            CollectionView.AddSubview(bottomShadow);

            var flowLayout = Layout as UICollectionViewFlowLayout;
            flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            flowLayout.MinimumInteritemSpacing = 0;
            flowLayout.MinimumLineSpacing = 0;
            flowLayout.EstimatedItemSize = new CGSize(70, 35);

            CollectionView.RegisterClassForCell(GetCellType(), CellId);

            ((IShellController)shellContext.Shell).AddAppearanceObserver(this, ShellSection);
            ShellSectionController.ItemsCollectionChanged += OnShellSectionItemsChanged;

            UpdateSelectedIndex();
            ShellSection.PropertyChanged += OnShellSectionPropertyChanged;
        }

        /// <summary>
        /// Called when appearanced changed.
        /// </summary>
        /// <param name="appearance">The appearance.</param>
        public void OnAppearanceChanged(ShellAppearance appearance)
        {
            if (appearance == null)
            {
                ResetAppearance();
            }
            else
            {
                SetAppearance(appearance);
            }
        }

        /// <summary>
        /// Resets the appearance.
        /// </summary>
        protected virtual void ResetAppearance()
        {
            SetValues(defaultBackgroundColor, defaultForegroundColor, defaultUnselectedColor);
        }

        /// <summary>
        /// Sets the appearance.
        /// </summary>
        /// <param name="appearance">The appearance.</param>
        protected virtual void SetAppearance(ShellAppearance appearance)
        {
            if (appearance == null)
            {
                return;
            }

            SetValues(
                appearance.BackgroundColor.IsDefault ? defaultBackgroundColor : appearance.BackgroundColor,
                appearance.ForegroundColor.IsDefault ? defaultForegroundColor : appearance.ForegroundColor,
                appearance.UnselectedColor.IsDefault ? defaultUnselectedColor : appearance.UnselectedColor);
        }

        /// <summary>
        /// Gets the cell type.
        /// </summary>
        /// <returns>The cell type.</returns>
        protected virtual Type GetCellType()
        {
            return typeof(ShellSectionHeaderCell);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                ((IShellController)shellContext.Shell).RemoveAppearanceObserver(this);
                ShellSectionController.ItemsCollectionChanged -= OnShellSectionItemsChanged;
                ShellSection.PropertyChanged -= OnShellSectionPropertyChanged;

                ShellSection = null;
                bar.RemoveFromSuperview();
                this.RemoveFromParentViewController();
                bar.Dispose();
                bar = null;

                if (bottomShadow != null)
                {
                    bottomShadow.Dispose();
                    bottomShadow = null;
                }
            }

            isDisposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Lays out the bar.
        /// </summary>
        protected void LayoutBar()
        {
            if (SelectedIndex < 0)
            {
                return;
            }

            if (ShellSectionController.GetItems().IndexOf(ShellSection.CurrentItem) != SelectedIndex)
            {
                return;
            }

            var layout = CollectionView.GetLayoutAttributesForItem(NSIndexPath.FromItemSection((int)SelectedIndex, 0));

            if (layout == null)
            {
                return;
            }

            var frame = layout.Frame;

            if (bar.Frame.Height != 2)
            {
                bar.Frame = new CGRect(frame.X, frame.Bottom - 2, frame.Width, 2);
            }
            else
            {
                UIView.Animate(.25, () => bar.Frame = new CGRect(frame.X, frame.Bottom - 2, frame.Width, 2));
            }
        }

        /// <summary>
        /// The shell section property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
            {
                UpdateSelectedIndex();
            }
        }

        /// <summary>
        /// Updated the selected index.
        /// </summary>
        /// <param name="animated">Is animated.</param>
        protected virtual void UpdateSelectedIndex(bool animated = false)
        {
            if (ShellSection.CurrentItem == null)
            {
                return;
            }

            SelectedIndex = ShellSectionController.GetItems().IndexOf(ShellSection.CurrentItem);

            if (SelectedIndex < 0)
            {
                return;
            }

            LayoutBar();

            CollectionView.SelectItem(NSIndexPath.FromItemSection((int)SelectedIndex, 0), false, UICollectionViewScrollPosition.CenteredHorizontally);
        }

        void OnShellSectionItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReloadData();
        }

        void ReloadData()
        {
            if (isDisposed)
            {
                return;
            }

            CollectionView.ReloadData();
            CollectionView.CollectionViewLayout.InvalidateLayout();
        }

        void SetValues(Color backgroundColor, Color foregroundColor, Color unselectedColor)
        {
            CollectionView.BackgroundColor = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, .863).ToUIColor();

            bool reloadData = selectedColor != foregroundColor || this.unselectedColor != unselectedColor;

            selectedColor = foregroundColor;
            this.unselectedColor = unselectedColor;

            if (reloadData)
            {
                ReloadData();
            }
        }

        /// <summary>
        /// The Shell Section Header Cell.
        /// </summary>
#pragma warning disable CA1034 // Nested types should not be visible
        public class ShellSectionHeaderCell : UICollectionViewCell
#pragma warning restore CA1034 // Nested types should not be visible
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ShellSectionHeaderCell"/> class.
            /// </summary>
            public ShellSectionHeaderCell()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ShellSectionHeaderCell"/> class.
            /// </summary>
            /// <param name="frame">The frame.</param>
            [Export("initWithFrame:")]
            public ShellSectionHeaderCell(CGRect frame) : base(frame)
            {
                Label = new UILabel();
                Label.TextAlignment = UITextAlignment.Center;
                Label.Font = UIFont.BoldSystemFontOfSize(14);
                ContentView.AddSubview(Label);
            }

            /// <summary>
            /// Gets or sets the selected color.
            /// </summary>
            /// <value>
            /// The selected color.
            /// </value>
            public UIColor SelectedColor { get; set; }

            /// <summary>
            /// Gets or sets the unselected color.
            /// </summary>
            /// <value>
            /// The unselected color.
            /// </value>
            public UIColor UnSelectedColor { get; set; }

            /// <inheritdoc/>
            public override bool Selected
            {
                get => base.Selected;
                set
                {
                    base.Selected = value;
                    Label.TextColor = value ? SelectedColor : UnSelectedColor;
                }
            }

            /// <summary>
            /// Gets the label.
            /// </summary>
            /// <value>
            /// The label.
            /// </value>
            public UILabel Label { get; }

            /// <inheritdoc/>
            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                Label.Frame = Bounds;
            }

            /// <inheritdoc/>
            public override CGSize SizeThatFits(CGSize size)
            {
                return new CGSize(Label.SizeThatFits(size).Width + 30, 35);
            }
        }
    }
}
