using System;
using System.Collections.Generic;
using System.Windows.Input;
using Float.Core.Collections;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Landing group view model.
    /// </summary>
    public class LandingGroupViewModel : BaseCollectionViewModel<ILandingData, LandingViewModel>
    {
        int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="LandingGroupViewModel"/> class.
        /// </summary>
        /// <param name="changeServerSelectedCommand">Change server selected command.</param>
        /// <param name="termsOfUseSelectedCommand">Terms of Use selected command.</param>
        /// <param name="modelCollection">Model collection task.</param>
        /// <param name="iconTappedCommand">Icon tapped command.</param>
        /// <param name="currentServerPlain">The current debug uri without any additional text.</param>
        public LandingGroupViewModel(ICommand changeServerSelectedCommand, ICommand termsOfUseSelectedCommand, IEnumerable<ILandingData> modelCollection, ICommand iconTappedCommand, Uri currentServerPlain) : base(modelCollection)
        {
#if DEBUG
            IsSetServerActive = true;
#else
            IsSetServerActive = false;
#endif

            SelectLoginCommand = changeServerSelectedCommand;
            SelectTermsOfUseCommand = termsOfUseSelectedCommand;
            IconTappedCommand = iconTappedCommand;
            CurrentServerPlain = currentServerPlain;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LandingGroupViewModel"/> class.
        /// </summary>
        /// <param name="loginSelectedCommand">Login selected command.</param>
        /// <param name="termsOfUseSelectedCommand">Terms of Use selected command.</param>
        /// <param name="modelCollection">Model collection.</param>
        /// <param name="iconTappedCommand">Icon tapped command.</param>
        /// <param name="filter">The Filter.</param>
        public LandingGroupViewModel(ICommand loginSelectedCommand, ICommand termsOfUseSelectedCommand, IEnumerable<ILandingData> modelCollection, ICommand iconTappedCommand, IFilter<ILandingData> filter = null) : base(modelCollection, filter)
        {
            SelectLoginCommand = loginSelectedCommand;
            SelectTermsOfUseCommand = termsOfUseSelectedCommand;
            IconTappedCommand = iconTappedCommand;
            TapCount = 0;
        }

        /// <summary>
        /// Gets the command to invoke when login is selected.
        /// </summary>
        /// <value>The login selection command.</value>
        public ICommand SelectLoginCommand { get; }

        /// <summary>
        /// Gets the command to invoke when the user taps the Onboarding icon.
        /// </summary>
        /// <value>The icon tapped command.</value>
        public ICommand IconTappedCommand { get; }

        /// <summary>
        /// Gets the command to invoke when termsOfUse/terms of use is selected.
        /// </summary>
        /// <value>The terms of use command.</value>
        public ICommand SelectTermsOfUseCommand { get; }

        /// <summary>
        /// Gets the command to invoke when set server is selected.
        /// </summary>
        /// <value>The set server command.</value>
        public ICommand SelectSetServerCommand { get; }

        /// <summary>
        /// Gets the title text for the login button.
        /// </summary>
        /// <value>The login button title.</value>
        public string LoginButtonTitle => Strings.SignInLabel;

        /// <summary>
        /// Gets the title text for the termsOfUse and Terms of Use button.
        /// </summary>
        /// <value>The title text.</value>
        public string TermsOfUseButtonTitle => StringsSpecific.ViewTermsLabel;

        /// <summary>
        /// Gets or sets the current debug uri without any additional text.
        /// </summary>
        /// <value>The current server.</value>
        public Uri CurrentServerPlain { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the set server button should be active.
        /// </summary>
        /// <value><c>true</c> if the server button should be active, <c>false</c> otherwise.</value>
        public bool IsSetServerActive { get; }

        /// <summary>
        /// Gets or sets the current position of the landing pages.
        /// </summary>
        /// <value>The current position.</value>
        public int Position
        {
            get
            {
                return position;
            }

            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        /// <summary>
        /// Gets or sets the current tap count of the app icon.
        /// </summary>
        /// <value>The current tap count of the app icon.</value>
        public int TapCount { get; set; }

        /// <summary>
        /// Updates the current debug uri.
        /// </summary>
        /// <param name="currentServerPlain">The current debug uri without any additional text.</param>
        public void UpdateUri(Uri currentServerPlain)
        {
            CurrentServerPlain = currentServerPlain;
            OnPropertyChanged(nameof(CurrentServerPlain));
        }
    }
}
