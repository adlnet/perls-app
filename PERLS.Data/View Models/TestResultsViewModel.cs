using System.Windows.Input;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Test Results View Model.
    /// </summary>
    public class TestResultsViewModel : CardViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultsViewModel"/> class.
        /// The TestResultsViewModel Class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="startOverCommand">The command to start the test over.</param>
        public TestResultsViewModel(ITest model, ICommand startOverCommand) : base(model)
        {
            TestResultState = new TestResultStateViewModel(model);
            StartOverCommand = startOverCommand;
        }

        /// <summary>
        /// Gets the current test result state.
        /// </summary>
        /// <value>The test result state.</value>
        public TestResultStateViewModel TestResultState { get; }

        /// <summary>
        /// Gets a command to start the test over.
        /// </summary>
        /// <value>A command.</value>
        public ICommand StartOverCommand { get; }

        /// <summary>
        /// Gets the title string.
        /// </summary>
        /// <value>
        /// The title string.
        /// </value>
        public string TitleString => Strings.TypeTest;

        /// <summary>
        /// Gets the subtitle string.
        /// </summary>
        /// <value>
        /// The subtitle string.
        /// </value>
        public string SubtitleString => Model.Name;

        /// <summary>
        /// Gets the status string.
        /// </summary>
        /// <value>
        /// The status string.
        /// </value>
        public string StatusString => Strings.StatLabelCompleted;

        /// <inheritdoc/>
        protected override void OnCardAppeared()
        {
            // Do nothing.
        }

        /// <inheritdoc />
        protected override void OnCardDisappeared()
        {
            // Do nothing.
        }
    }
}
