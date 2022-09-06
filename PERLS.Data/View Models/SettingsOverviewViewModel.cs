using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Float.Core.Commands;
using PERLS.Data.Commands;
using PERLS.Data.Converters;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// View model for the settings page.
    /// </summary>
    public class SettingsOverviewViewModel : BasePageViewModel
    {
        readonly Definition.Services.IAppContextService appInfo;
        readonly ILearner learner;
        NavigationOption<SettingsDestination> selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsOverviewViewModel"/> class.
        /// </summary>
        /// <param name="navigateCommand">Command for handling selected navigation.</param>
        /// <param name="closeCommand">Command for closing the settings page.</param>
        public SettingsOverviewViewModel(NavigateCommand<SettingsDestination> navigateCommand, ICommand closeCommand)
        {
            if (navigateCommand == null)
            {
                throw new ArgumentNullException(nameof(navigateCommand));
            }

            NavigateCommand = new DebounceCommand(navigateCommand, 700);
            CloseCommand = closeCommand ?? throw new ArgumentNullException(nameof(closeCommand));
            appInfo = DependencyService.Get<Definition.Services.IAppContextService>();
            learner = appInfo.CurrentLearner;
        }

        /// <summary>
        /// Gets destination options for settings.
        /// </summary>
        public enum SettingsDestination
        {
            /// <summary>
            /// Go to the user's account information.
            /// </summary>
            Account,

            /// <summary>
            /// Go to page so user can update their groups
            /// </summary>
            Groups,

            /// <summary>
            /// Go to page so user can update their interests
            /// </summary>
            Interests,

            /// <summary>
            /// Go to a screen for the user to provide feedback.
            /// </summary>
            Feedback,

            /// <summary>
            /// Go to a screen for the user to receive support.
            /// </summary>
            Support,

            /// <summary>
            /// Go to the app's terms of use.
            /// </summary>
            Terms,

            /// <summary>
            /// Go to the app acknowledgements.
            /// </summary>
            Acknowledgements,

            /// <summary>
            /// Go to the download management.
            /// </summary>
            DownloadManagement,

            /// <summary>
            /// Go to the LRS cache viewer (only in debug builds).
            /// </summary>
            LRSCache,

            /// <summary>
            /// Trigger an application crash (only in debug builds).
            /// </summary>
            Crash,

            /// <summary>
            /// Log the user out.
            /// </summary>
            Logout,

            /// <summary>
            /// Goes to device settings.
            /// </summary>
            DeviceSettings,
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <value>The page title.</value>
        public new string Title => Strings.SettingsLabel;

        /// <summary>
        /// Gets the command to handle navigation.
        /// </summary>
        /// <value>The navigation command.</value>
        public ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets the command to handle closing the page.
        /// </summary>
        /// <value>The command to close the page.</value>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Gets or sets the selected navigation item.
        /// </summary>
        /// <value>The selected item.</value>
        public NavigationOption<SettingsDestination> SelectedItem
        {
            get => selectedItem;
            set => SetAndExecute(ref selectedItem, NavigateCommand, value);
        }

        /// <summary>
        /// Gets the current user's avatar.
        /// </summary>
        /// <value>The avatar.</value>
        [NotifyWhenPropertyChanges(nameof(ILearner.Avatar))]
        public ImageSource UserAvatar => learner.Avatar != null ? ImageSource.FromUri(learner.Avatar.Url) : ImageSource.FromFile("avatar_settings");

        /// <summary>
        /// Gets the current user's name.
        /// </summary>
        /// <value>The user name.</value>
        public string UserName => learner.Name;

        /// <summary>
        /// Gets the current user's email address.
        /// </summary>
        /// <value>The user email.</value>
        public string UserEmail => learner.Email;

        /// <summary>
        /// Gets information about the app.
        /// </summary>
        /// <value>The app information.</value>
        public string AppInfo => $"{appInfo.Name} {appInfo.Version} ({appInfo.BuildNumber})\n{appInfo.Server.Host}\n{Constants.Configuration} ({appInfo.PackageIdentifier})";

        /// <summary>
        /// Gets navigation options for the settings screen.
        /// </summary>
        /// <value>The navigation options.</value>
        public IList<NavigationOption<SettingsDestination>> Options => new List<NavigationOption<SettingsDestination>>
        {
            Constants.AccountSetting ? new NavigationOption<SettingsDestination>(SettingsDestination.Account, Strings.ViewAccountLabel) : default,
            Constants.GroupsSetting ? new NavigationOption<SettingsDestination>(SettingsDestination.Groups, StringsSpecific.GroupsTitle) : default,
            Constants.InterestsAccess ? new NavigationOption<SettingsDestination>(SettingsDestination.Interests, Strings.InterestsTitle) : default,
            Constants.FeedbackAccess ? new NavigationOption<SettingsDestination>(SettingsDestination.Feedback, Strings.SendFeedbackLabel) : default,
            Constants.PrivacyPolicySetting ? new NavigationOption<SettingsDestination>(SettingsDestination.Terms, StringsSpecific.ViewTermsLabel) : default,
            new NavigationOption<SettingsDestination>(SettingsDestination.Acknowledgements, Strings.ViewAcknowledgementsLabel),
            Constants.Configuration == BuildConfiguration.Debug ? new NavigationOption<SettingsDestination>(SettingsDestination.LRSCache, "Statement Cache") : default,
            Constants.Configuration == BuildConfiguration.Debug ? new NavigationOption<SettingsDestination>(SettingsDestination.Crash, "Trigger Crash") : default,
            Constants.OfflineAccess ? new NavigationOption<SettingsDestination>(SettingsDestination.DownloadManagement, Strings.DownloadManagement) : default,
            Constants.SupportSetting ? new NavigationOption<SettingsDestination>(SettingsDestination.Support, Strings.ViewSupportLabel) : default,
            DependencyService.Get<ISettingsService>().CanOpenSettings() ? new NavigationOption<SettingsDestination>(SettingsDestination.DeviceSettings, Strings.DeviceSettingsLabel, false) : default,
            new NavigationOption<SettingsDestination>(SettingsDestination.Logout, Strings.LogOutLabel, false, true),
        }.Where(a => a != default).ToList();

        /// <summary>
        /// A method to subscribe view model to events.
        /// </summary>
        public void Subscribe()
        {
            if (learner is INotifyPropertyChanged learnerNotificator)
            {
                learnerNotificator.PropertyChanged += OnModelPropertyChanged;
            }
        }

        /// <summary>
        /// A method to unsubscribe view model from events.
        /// </summary>
        public void Unsubscribe()
        {
            if (learner is INotifyPropertyChanged learnerNotificator)
            {
                learnerNotificator.PropertyChanged -= OnModelPropertyChanged;
            }
        }
    }
}
