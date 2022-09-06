using UIKit;

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// The localizable collection view flow layout.
    /// </summary>
    class LocalizableCollectionViewFlowLayout : UICollectionViewFlowLayout
    {
        public LocalizableCollectionViewFlowLayout() : base()
        {
            MinimumInteritemSpacing = 10;
            MinimumLineSpacing = 10;
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
        }

        public override bool FlipsHorizontallyInOppositeLayoutDirection => true;
    }
}
