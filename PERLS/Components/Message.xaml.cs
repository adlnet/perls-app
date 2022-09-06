using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Extensions;
using Xamarin.Forms;
using XFormsTouch;

namespace PERLS.Components
{
    /// <summary>
    /// A banner containing a title and body to give the user a brief message.
    /// </summary>
    public partial class Message : ContentView
    {
        /// <summary>
        /// Identifies the <see cref="Title"/> property.
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Message));

        /// <summary>
        /// Identifies the <see cref="Body"/> property.
        /// </summary>
        public static readonly BindableProperty BodyProperty = BindableProperty.Create(nameof(Body), typeof(string), typeof(Message));

        /// <summary>
        /// Identifies the <see cref="TappedCommand"/> property.
        /// </summary>
        public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(Message));

        Color originalBackgroundColor;
        Point? touchStart;

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the title of the message.
        /// </summary>
        /// <value>The message title.</value>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the body of the message.
        /// </summary>
        /// <value>The message body.</value>
        public string Body
        {
            get => (string)GetValue(BodyProperty);
            set => SetValue(BodyProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to invoke when the message is tapped.
        /// </summary>
        /// <value>The command invoked when the message is tapped.</value>
        public ICommand TappedCommand
        {
            get => (ICommand)GetValue(TappedCommandProperty);
            set => SetValue(TappedCommandProperty, value);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(IsVisible))
            {
                if (!IsVisible)
                {
                    AnimateOut()
                        .ContinueWith(_ =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                base.OnPropertyChanged(nameof(IsVisible));

                                if (!IsVisible)
                                {
                                    HeightRequest = 0.1;
                                }
                            });
                        });
                    return;
                }

                HeightRequest = -1d;
                AnimateIn();
            }

            base.OnPropertyChanged(propertyName);
        }

        void HandleTouch(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    SetSelected(true);
                    touchStart = args.Location;
                    break;
                case TouchActionType.Released:
                    SetSelected(false);
                    touchStart = null;
                    if (TappedCommand is ICommand command && command.CanExecute(sender))
                    {
                        command.Execute(sender);
                    }

                    break;
                case TouchActionType.Moved when touchStart is Point start && args.Location.Distance(start) < 10:
                    // Do nothing
                    break;
                default:
                    SetSelected(false);
                    touchStart = null;
                    break;
            }
        }

        void SetSelected(bool selected)
        {
            if (selected && originalBackgroundColor == default)
            {
                originalBackgroundColor = Content.BackgroundColor;
                Content.BackgroundColor = Content.BackgroundColor.DarkerColor();
            }
            else if (!selected && originalBackgroundColor != default)
            {
                Content.BackgroundColor = originalBackgroundColor;
                originalBackgroundColor = default;
            }
        }

        Task AnimateIn()
        {
            ViewExtensions.CancelAnimations(Content);
            if (Content.TranslationY == 0)
            {
                Content.TranslationY = -Height;
            }

            return Content.TranslateTo(0, 0, 500, Easing.SinOut);
        }

        Task AnimateOut()
        {
            if (Height <= 0)
            {
                return Task.CompletedTask;
            }

            ViewExtensions.CancelAnimations(Content);
            return Content.TranslateTo(0, -Height, 250, Easing.SinIn);
        }
    }
}
