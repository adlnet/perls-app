using System;
using System.Drawing;
using System.Windows.Input;
using Float.Core.Exceptions;
using Float.Core.ViewModels;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.Sections
{
    /// <summary>
    /// A simple view model for binding a name, index, and command to a button.
    /// </summary>
    public class BlockButtonViewModel : BaseViewModel, IEquatable<BlockButtonViewModel>
    {
        bool isSelected = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockButtonViewModel"/> class.
        /// </summary>
        /// <param name="name">The name to show.</param>
        /// <param name="index">The block index to which this button relates.</param>
        public BlockButtonViewModel(string name, int index)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidStringArgumentException(nameof(name));
            }

            this.Name = name;
            this.Index = index;
            isSelected = index == 0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is active.
        /// </summary>
        /// <value><c>true</c> if this button is selected, <c>false</c> otherwise.</value>
        public bool IsSelected
        {
            get => isSelected;
            set => SetField(ref isSelected, value);
        }

        /// <summary>
        /// Gets the index to which this button relates.
        /// </summary>
        /// <value>The button index.</value>
        public int Index { get; }

        /// <summary>
        /// Gets the name to display in this button.
        /// </summary>
        /// <value>The button name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the font attributes.
        /// </summary>
        /// <value>
        /// The font attributes.
        /// </value>
        public FontAttributes FontAttributes => Device.RuntimePlatform == Device.Android ? FontAttributes.Bold : isSelected ? FontAttributes.Bold : FontAttributes.None;

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily => isSelected ? "BoldFont" : "NormalFont";

        /// <summary>
        /// Gets the selection background color.
        /// </summary>
        /// <value>
        /// The selection background color.
        /// </value>
        public Xamarin.Forms.Color SelectionBackgroundColor => isSelected ? (Xamarin.Forms.Color)Application.Current.Resources["SecondaryColor"] : Xamarin.Forms.Color.Transparent;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is BlockButtonViewModel other)
            {
                return Index == other.Index;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Equals(BlockButtonViewModel obj) => obj is BlockButtonViewModel other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -2134847229 + Index.GetHashCode();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsSelected))
            {
                OnPropertyChanged(nameof(SelectionBackgroundColor));
                OnPropertyChanged(nameof(FontAttributes));
                OnPropertyChanged(nameof(FontFamily));
            }
        }
    }
}
