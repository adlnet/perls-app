using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// View model for the nav bar.
    /// </summary>
    public class LogoNavBarViewModel : BasePageViewModel
    {
        bool shouldShowLogo;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoNavBarViewModel"/> class.
        /// </summary>
        /// <param name="navigateSettingsCommand">A command to navigate to the settings.</param>
        /// <param name="shouldShowLogo">Whether or not to show the PERLS logo.</param>
        /// <param name="title">An optional title to display.</param>
        public LogoNavBarViewModel(ICommand navigateSettingsCommand, bool shouldShowLogo, string title = null)
        {
            Learner = DependencyService.Get<IAppContextService>().CurrentLearner;
            NavigateSettingsCommand = navigateSettingsCommand ?? throw new ArgumentNullException(nameof(navigateSettingsCommand));

            if (Learner is INotifyPropertyChanged learner)
            {
                learner.PropertyChanged += OnModelPropertyChanged;
            }

            this.shouldShowLogo = shouldShowLogo;
            Title = title;
        }

        /// <summary>
        /// Gets or sets the location of the logo.
        /// </summary>
        /// <value>The Logo location.</value>
        public static string LogoLocation { get; set; }

        /// <summary>
        /// Gets the Uri for the Logo.
        /// </summary>
        /// <value>The Logo uri.</value>
        public ImageSource LogoUri
        {
            get
            {
                if (string.IsNullOrEmpty(LogoLocation))
                {
                    return new FileImageSource()
                    {
                        File = "icon_no_shadow_inverted",
                    };
                }

                return IsSvg ? SvgImageSource.FromUri(new Uri(LogoLocation)) : ImageSource.FromUri(new Uri(LogoLocation));
            }
        }

        /// <summary>
        /// Gets the current learner.
        /// </summary>
        /// <value>The current learner.</value>
        public ILearner Learner { get; }

        /// <summary>
        /// Gets a value indicating whether to show the app logo.
        /// </summary>
        /// <value><c>true</c> if the logo should be shown, false otherwise.</value>
        public bool ShouldShowLogo
        {
            get
            {
                return shouldShowLogo && !IsSvg;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the logo is an SVG.
        /// </summary>
        /// <value><c>true</c> if the logo is an SVG, false otherwise.</value>
        public bool ShouldShowSvgLogo
        {
            get
            {
                return shouldShowLogo && IsSvg;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show the current title.
        /// </summary>
        /// <value><c>true</c> if the title should be shown, false otherwise.</value>
        public bool ShouldShowTitle => !shouldShowLogo;

        /// <summary>
        /// Gets the settings icon source.
        /// </summary>
        /// <value>The settings icon source.</value>
        [NotifyWhenPropertyChanges(nameof(ILearner.Avatar))]
        public ImageSource SettingsIcon
        {
            get
            {
                if (Learner?.Avatar?.Url is Uri uri)
                {
                    return ImageSource.FromUri(uri);
                }

                return ImageSource.FromFile(DeviceInfo.Platform == DevicePlatform.iOS ? "menu_avatar" : "avatar_settings");
            }
        }

        /// <summary>
        /// Gets a command to invoke when the user selects the settings icon.
        /// </summary>
        /// <value>A navigation command.</value>
        public ICommand NavigateSettingsCommand { get; }

        bool IsSvg
        {
            get
            {
                var ext = Path.GetExtension(LogoLocation);
                if (ext == ".svg")
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Unsubscribe from learner events. This must be called before this object will be garbage collected.
        /// </summary>
        public void Unsubscribe()
        {
            if (Learner is INotifyPropertyChanged learner)
            {
                learner.PropertyChanged -= OnModelPropertyChanged;
            }
        }
    }
}
