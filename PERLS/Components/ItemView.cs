using System;
using System.Collections.Generic;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A base implementation of a view displaying a title, image, and tags for a content item.
    /// </summary>
    public abstract class ItemView : ContentView
    {
        /// <summary>
        /// Identifies the <see cref="Title"/> property.
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(ItemView));

        /// <summary>
        /// Identifies the <see cref="Image"/> property.
        /// </summary>
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(ItemView));

        /// <summary>
        /// Identifies the <see cref="Tags"/> property.
        /// </summary>
        public static readonly BindableProperty TagsProperty = BindableProperty.Create(nameof(Tags), typeof(IEnumerable<TagViewModel>), typeof(ItemView));

        /// <summary>
        /// Identifies the <see cref="Caption"/> property.
        /// </summary>
        public static readonly BindableProperty CaptionProperty = BindableProperty.Create(nameof(Caption), typeof(string), typeof(ItemView));

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> property.
        /// </summary>
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(ItemView));

        /// <summary>
        /// Identifies the <see cref="AltColor"/> property.
        /// </summary>
        public static readonly BindableProperty AltColorProperty = BindableProperty.Create(nameof(AltColor), typeof(Color), typeof(ItemView));

        /// <summary>
        /// Gets or sets the contents Alternative color.
        /// </summary>
        /// <value>The alternative color.</value>
        public Color AltColor
        {
            get => (Color)GetValue(AltColorProperty);
            set => SetValue(AltColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the content item's title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get => GetValue(TitleProperty) as string;
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the content item's image.
        /// </summary>
        /// <value>The image.</value>
        public ImageSource Image
        {
            get => GetValue(ImageProperty) as ImageSource;
            set => SetValue(ImageProperty, value);
        }

        /// <summary>
        /// Gets or sets the content item's caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get => GetValue(CaptionProperty) as string;
            set => SetValue(CaptionProperty, value);
        }

        /// <summary>
        /// Gets or sets the content item's tags.
        /// </summary>
        /// <value>The tags.</value>
        public IEnumerable<TagViewModel> Tags
        {
            get => GetValue(TagsProperty) as IEnumerable<TagViewModel>;
            set => SetValue(TagsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        /// <value><c>true</c> if the item is selected, <c>false</c> otherwise.</value>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}
