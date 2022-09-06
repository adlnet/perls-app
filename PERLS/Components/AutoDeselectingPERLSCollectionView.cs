using System.Runtime.CompilerServices;

namespace PERLS.Components
{
    /// <summary>
    /// Exactly like <see cref="PERLSCollectionView"/>, but it deselects the selected item immediately.
    /// </summary>
    public class AutoDeselectingPERLSCollectionView : PERLSCollectionView
    {
        /// <inheritdoc />
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedItem) && SelectedItem != null)
            {
                SelectedItem = null;
            }
        }
    }
}
