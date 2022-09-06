using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Recommendation state view model.
    /// </summary>
    public class TestResultStateViewModel : ItemStateViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultStateViewModel"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        public TestResultStateViewModel(IItem item) : base(item)
        {
        }

        /// <summary>
        /// Gets the current feedback for the test result.
        /// </summary>
        /// <value>The test feedback.</value>
        public string Feedback => State.Feedback;

        /// <summary>
        /// Gets a value indicating whether this test has been completed.
        /// </summary>
        /// <value>A value indicating whether the test is complete.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Completed))]
        public bool IsComplete => State.Completed == CorpusItemLearnerState.Status.Enabled;

        /// <summary>
        /// Gets a value indicating whether the results are currently loading.
        /// </summary>
        /// <value><c>true</c> if state is currently being loaded.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Completed))]
        public bool IsLoading => State.Completed == CorpusItemLearnerState.Status.Retrieving;
    }
}
