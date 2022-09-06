using System;
using System.Collections.Generic;
using System.Linq;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// Tags view.
    /// </summary>
    public partial class TagsView : ContentView
    {
        /// <summary>
        /// The max lines property.
        /// </summary>
        public static readonly BindableProperty MaxLinesProperty = BindableProperty.Create(nameof(MaxLines), typeof(int), typeof(TagsView), propertyChanged: MaxLinesPropertyChanged);

        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty TagSourceProperty = BindableProperty.Create(nameof(TagSource), typeof(IEnumerable<TagViewModel>), typeof(TagsView), propertyChanged: TagSourcePropertyChanged);

        /// <summary>
        /// The tags style property.
        /// </summary>
        public static readonly BindableProperty AlternateColorProperty = BindableProperty.Create(nameof(AlternateColor), typeof(bool), typeof(TagsView));

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsView"/> class.
        /// </summary>
        public TagsView()
        {
            InitializeComponent();

            if (Device.FlowDirection == FlowDirection.RightToLeft)
            {
                Tags.Direction = FlexDirection.RowReverse;
            }
        }

        /// <summary>
        /// Gets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor => AlternateColor ? Color.White : Application.Current.Color("SecondaryColor");

        /// <summary>
        /// Gets or sets the max lines.
        /// </summary>
        /// <value>The max lines.</value>
        public int MaxLines
        {
            get => (int)GetValue(MaxLinesProperty);
            set => SetValue(MaxLinesProperty, value);
        }

        /// <summary>
        /// Gets or sets the tag source.
        /// </summary>
        /// <value>The tag source.</value>
        public IEnumerable<TagViewModel> TagSource
        {
            get => GetValue(TagSourceProperty) as IEnumerable<TagViewModel>;
            set => SetValue(TagSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tags should be secondary color or white.
        /// </summary>
        /// <value>Should be secondary color or white.</value>
        public bool AlternateColor
        {
            get => (bool)GetValue(AlternateColorProperty);
            set => SetValue(AlternateColorProperty, value);
        }

        private static void TagSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is not IEnumerable<TagViewModel> tags)
            {
                return;
            }

            var control = (TagsView)bindable;
            BindableLayout.SetItemsSource(control.Tags, tags);
        }

        private static void MaxLinesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var maxLines = (int)newValue;

            var control = (TagsView)bindable;
            control.HeightRequest = 25 * maxLines;
        }

        void HandleTagPressed(object obj, EventArgs args)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAsync();
            }

            var button = obj as Button;
            var tagViewModel = button.BindingContext as TagViewModel;
            tagViewModel.TagSelected.Execute(tagViewModel);
        }
    }
}
