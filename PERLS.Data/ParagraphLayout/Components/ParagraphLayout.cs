using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PERLS.Data.Extensions;
using PERLS.Data.ParagraphLayout.Models;
using Xamarin.Forms;

namespace PERLS.Data.ParagraphLayout.Components
{
    /// <summary>
    /// Allows for dynamic rendering of paragraphs.
    /// </summary>
    public class VerticalParagraphLayout : ContentView
    {
        /// <summary>
        /// Identifies the <see cref="MaximumWidth"/> property.
        /// </summary>
        public static readonly BindableProperty MaximumWidthProperty = BindableProperty.Create(nameof(MaximumWidth), typeof(double), typeof(VerticalParagraphLayout), propertyChanged: MaximumWidthPropertyChanged);

        /// <summary>
        /// Identifies the <see cref="Paragraphs"/> property.
        /// </summary>
        public static readonly BindableProperty ParagraphsProperty = BindableProperty.Create(nameof(Paragraphs), typeof(IList<Paragraph>), typeof(VerticalParagraphLayout), propertyChanged: HandleParagraphsChanged);

        object oldBinding;
        bool stackShouldReset;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalParagraphLayout"/> class.
        /// </summary>
        public VerticalParagraphLayout()
        {
            Content = new StackLayout();
        }

        /// <summary>
        /// Gets or sets a value indicating the maximum width of images in the paragraph layout.
        /// </summary>
        /// <value>The maximum width.</value>
        public double MaximumWidth
        {
            get => (double)GetValue(MaximumWidthProperty);
            set => SetValue(MaximumWidthProperty, value);
        }

        /// <summary>
        /// Gets the paragraphs displayed in this view.
        /// </summary>
        /// <value>The displayed paragraphs.</value>
        public IList<Paragraph> Paragraphs
        {
            get => GetValue(ParagraphsProperty) as IList<Paragraph>;
        }

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > MaximumWidth && MaximumWidth != 0)
            {
                WidthRequest = MaximumWidth;
            }

            base.OnSizeAllocated(width, height);
        }

        /// <inheritdoc/>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // On rotation this gets called even though the binding context may not have actually changed.  If it didnt change we dont want rebuild the view.
            if (oldBinding != BindingContext && oldBinding != null)
            {
                stackShouldReset = true;
            }
            else
            {
                stackShouldReset = false;
            }

            oldBinding = BindingContext;
        }

        private static void MaximumWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not VerticalParagraphLayout paragraphLayout
                || oldValue == newValue
                || paragraphLayout.Content is not StackLayout stackLayout)
            {
                return;
            }

            foreach (var child in stackLayout.Children)
            {
                if (child is MaximumWidthImage maximumWidthImage)
                {
                    maximumWidthImage.MaximumWidth = (double)newValue;
                }
            }
        }

        static void HandleParagraphsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((VerticalParagraphLayout)bindable).UpdateLayout();
        }

        void UpdateLayout()
        {
            if (Content is not StackLayout stack)
            {
                return;
            }

            // if the layout is populated we dont always need to update the layout.
            if (stack.Children.Any() && !stackShouldReset)
            {
                return;
            }

            stack.Children.Clear();
            if (Paragraphs == null)
            {
                return;
            }

            var fields = Paragraphs.SelectMany(p => p.Fields);

            foreach (var field in fields)
            {
                View view;
                switch (field.Type)
                {
                    case "Image":
                        var image = new MaximumWidthImage();
                        if (MaximumWidth != 0)
                        {
                            image.MaximumWidth = MaximumWidth;
                        }

                        image.Margin = new Thickness(0, 0, 1, 0);
                        SetAttributes(image, field.Attributes);
                        image.LoadingPlaceholder = ImageSource.FromFile("placeholder");
                        image.ErrorPlaceholder = ImageSource.FromFile("placeholder");
                        view = image;
                        break;
                    case "Label":
                        var label = new Label
                        {
                            Style = FindResource<Style>("TextStyle"),
                        };

                        SetAttributes(label, field.Attributes);

                        // If the type is HTML, generate some basic styling.
                        if (label.TextType == TextType.Html)
                        {
                            string styling = string.Empty;
                            if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                            {
                                // Sometimes CSS refers to the font name without spaces.
                                var altFontFamily = label.FontFamily.Replace(" ", string.Empty);
                                styling = $"<style>body{{color: {label.TextColor.ToHtmlHex()}; font: {label.FontSize}px \"{label.FontFamily}\", {altFontFamily}, sans-serif; }} p {{margin: 0px; padding: 0px;}}</style>";
                            }

                            label.Text = styling + label.Text;
                        }

                        view = label;
                        break;
                    default:
                        continue;
                }

                stack.Children.Add(view);
            }
        }

        void SetAttributes(View view, Dictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                if (view.GetType().GetProperty(attribute.Key) is not PropertyInfo property)
                {
                    continue;
                }

                if (property.PropertyType.IsEnum)
                {
                    property.SetValue(view, Enum.Parse(property.PropertyType, attribute.Value));
                    continue;
                }

                switch (property.PropertyType.Name)
                {
                    case nameof(ImageSource):
                        property.SetValue(view, ImageSource.FromUri(new Uri(attribute.Value)));
                        break;
                    case nameof(Style):
                        property.SetValue(view, FindResource<Style>(attribute.Value + "Style"));
                        break;
                    default:
                        property.SetValue(view, attribute.Value);
                        break;
                }
            }
        }

        T FindResource<T>(string name)
        {
            if (Resources.ContainsKey(name))
            {
                return (T)Resources[name];
            }

            if (Parent is VisualElement parent && parent.Resources.ContainsKey(name))
            {
                return (T)parent.Resources[name];
            }

            var containingView = Parent;
            while (containingView != null && !(containingView is ContentView))
            {
                containingView = containingView.Parent;
            }

            if (containingView is ContentView view && view.Resources.ContainsKey(name))
            {
                return (T)view.Resources[name];
            }

            if (Application.Current.Resources.ContainsKey(name))
            {
                return (T)Application.Current.Resources[name];
            }

            return default;
        }
    }
}
