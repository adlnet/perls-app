using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A view that is centered vertically with an Image, title, and detail text stacked vertically.
    /// </summary>
    public partial class CenteredImageTitleDetailView : ContentView
    {
        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(CenteredImageTitleDetailView), propertyChanged: ImageChanged);

        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(CenteredImageTitleDetailView), propertyChanged: TitleChanged);

        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty DetailProperty = BindableProperty.Create(nameof(Detail), typeof(string), typeof(CenteredImageTitleDetailView), propertyChanged: DetailChanged);

        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty ShowButtonProperty = BindableProperty.Create(nameof(ShowButton), typeof(bool), typeof(CenteredImageTitleDetailView), propertyChanged: ShowButtonChanged, defaultValue: false);

        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty ButtonTitleProperty = BindableProperty.Create(nameof(ButtonTitle), typeof(string), typeof(CenteredImageTitleDetailView), propertyChanged: ButtonTitleChanged);

        /// <summary>
        /// The tag source property.
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CenteredImageTitleDetailView), propertyChanged: CommandChanged);

        /// <summary>
        /// Initializes a new instance of the <see cref="CenteredImageTitleDetailView"/> class.
        /// </summary>
        public CenteredImageTitleDetailView()
        {
            InitializeComponent();
            ActionButton.IsVisible = ShowButton;
        }

        /// <summary>
        /// Gets or sets the Image Name.
        /// </summary>
        /// <value>The tag source.</value>
        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        /// <value>The tag source.</value>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the detail text.
        /// </summary>
        /// <value>The tag source.</value>
        public string Detail
        {
            get => (string)GetValue(DetailProperty);
            set => SetValue(DetailProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button should be shown.
        /// </summary>
        /// <value>The tag source.</value>
        public bool ShowButton
        {
            get => (bool)GetValue(ShowButtonProperty);
            set => SetValue(ShowButtonProperty, value);
        }

        /// <summary>
        /// Gets or sets the button title.
        /// </summary>
        /// <value>The tag source.</value>
        public string ButtonTitle
        {
            get => (string)GetValue(ButtonTitleProperty);
            set => SetValue(ButtonTitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The tag source.</value>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        static void ImageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CenteredImageTitleDetailView;
            var imageSource = newValue as ImageSource;
            if (view == null)
            {
                return;
            }

            view.EmptyImage.Source = imageSource;

            if (imageSource is FileImageSource fileImageSource && string.IsNullOrEmpty(fileImageSource.File))
            {
                view.EmptyImage.IsVisible = false;
            }
            else
            {
                view.EmptyImage.IsVisible = true;
            }
        }

        static void TitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CenteredImageTitleDetailView;
            var title = newValue as string;
            if (view == null)
            {
                return;
            }

            view.TitleLabel.Text = title;
            view.TitleLabel.IsVisible = !string.IsNullOrEmpty(title);
        }

        static void DetailChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CenteredImageTitleDetailView;
            var detail = newValue as string;
            if (view == null)
            {
                return;
            }

            view.DetailLabel.Text = detail;
        }

        static void ButtonTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CenteredImageTitleDetailView;
            var title = newValue as string;
            if (view == null)
            {
                return;
            }

            view.ActionButton.Text = title;
        }

        static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CenteredImageTitleDetailView;
            var command = newValue as ICommand;
            if (view == null || command == null)
            {
                return;
            }

            view.ActionButton.Command = command;
        }

        static void ShowButtonChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CenteredImageTitleDetailView;
            var showButton = (bool)newValue;
            if (view == null)
            {
                return;
            }

            view.ActionButton.IsVisible = showButton;
        }
    }
}
