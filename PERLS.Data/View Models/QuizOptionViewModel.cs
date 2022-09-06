using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// An answer option in the quiz.
    /// </summary>
    public class QuizOptionViewModel : Float.Core.ViewModels.SelectableViewModel<IQuizOption>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizOptionViewModel"/> class.
        /// </summary>
        /// <param name="model">The quiz option.</param>
        /// <param name="quiz">The quiz containing option.</param>
        public QuizOptionViewModel(QuizViewModel quiz, IQuizOption model) : base(model)
        {
            Quiz = quiz;
        }

        /// <summary>
        /// Gets the parent view model for this option (the quiz).
        /// </summary>
        /// <value>The quiz view model.</value>
        public QuizViewModel Quiz { get; }

        /// <summary>
        /// Gets the text of the answer option.
        /// </summary>
        /// <value>The answer text.</value>
        public string Text => Model.Text;

        /// <summary>
        /// Gets the feedback for the answer option.
        /// </summary>
        /// <value>The answer feedback.</value>
        public string Feedback => Model.Feedback;

        /// <summary>
        /// Gets a value indicating whether the answer option is correct.
        /// </summary>
        /// <value><c>true</c> if the option is correct, <c>false</c> otherwise.</value>
        public bool IsCorrect => Model.IsCorrect;

        internal new IQuizOption Model => base.Model;
    }
}
