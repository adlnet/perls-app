using System;
using System.Windows.Input;
using Float.Core.Commands;
using PERLS.Data.Commands;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// Handles the appearance of interactivity hints in the app.
    /// </summary>
    public class InteractivityHelper
    {
        private const string InteractiveHintOpacity = "InteractiveHintOpacity";
        readonly Application application;
        readonly ICommand displayHintsCommand;
        bool hintsDisabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractivityHelper"/> class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="timeout">The duration to wait until considering the user idle.</param>
        public InteractivityHelper(Application application, int timeout = 5000)
        {
            this.application = application ?? throw new ArgumentNullException(nameof(application));
            displayHintsCommand = new ThrottleCommand(new Command(HandleIdleTimer), timeout);

            HideInteractivityHints();
            ResetIdleTimer();

            application.PageAppearing += HandleInteractivity;
            application.PageDisappearing += HandleInteractivity;
        }

        /// <summary>
        /// Gets or sets the current opacity of interactivity hints.
        /// </summary>
        /// <value>The hint opacity.</value>
        public double HintOpacity
        {
            get => (double)application.Resources[InteractiveHintOpacity];
            set => application.Resources[InteractiveHintOpacity] = value;
        }

        /// <summary>
        /// Gets a value indicating whether the hint is currently showing.
        /// </summary>
        /// <value><c>true</c> if the hint is showing, <c>false</c> otherwise.</value>
        public bool IsHintShowing => HintOpacity > 0;

        /// <summary>
        /// Show interactivity hints.
        /// </summary>
        public void ShowInteractivityHints()
        {
            if (IsHintShowing)
            {
                return;
            }

            if (application.MainPage is Page main)
            {
                var animation = new Animation(v => HintOpacity = v, 0, 0.55);
                animation.Commit(main, "ShowHints", 16, 1000);
            }
        }

        /// <summary>
        /// Hides the interactivity hints and disables the idle timer.
        /// </summary>
        public void HideAndDisableInteractivityHints()
        {
            hintsDisabled = true;
            HideInteractivityHints();
        }

        /// <summary>
        /// Hide interactivity hints.
        /// </summary>
        public void HideInteractivityHints()
        {
            HintOpacity = 0;
        }

        /// <summary>
        /// Resets the idle timeout timer.
        /// </summary>
        public void ResetIdleTimer()
        {
            if (hintsDisabled)
            {
                return;
            }

            displayHintsCommand.Execute(null);
        }

        void HandleIdleTimer()
        {
            if (hintsDisabled)
            {
                return;
            }

            ShowInteractivityHints();
        }

        void HandleInteractivity(object sender, Page e)
        {
            HideInteractivityHints();
            ResetIdleTimer();
        }
    }
}
