using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Float.Core.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A quiz item.
    /// </summary>
    public class QuizViewModel : CardViewModel, IAdvanceableStackViewModel
    {
        readonly ITest test;
        bool isAnswered;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizViewModel"/> class.
        /// </summary>
        /// <param name="model">The quiz item.</param>
        /// <param name="questionAnsweredCommand">A command to optionally invoke when a question is answered.</param>
        /// <param name="nextCardCommand">Gets the goto next card command.</param>
        /// <param name="test">The test, optionally.</param>
        public QuizViewModel(IQuiz model, ICommand questionAnsweredCommand = null, ICommand nextCardCommand = null, ITest test = null) : base(model)
        {
            var selectAnswerCommand = new Command(
                (arg) =>
                {
                    if (arg is QuizOptionViewModel option)
                    {
                        option.IsSelected = true;
                        IsAnswered = true;
                        SelectedItem = option;
                        OnPropertyChanged(nameof(SelectedItem));
                        OnPropertyChanged(nameof(Feedback));
                        OnPropertyChanged(nameof(ResetAnswer));

                        ReportingService.ReportQuestionAnswered(Model, SelectedItem.Model, Duration, test);
                    }
                },
                (arg) => !IsAnswered);

            if (questionAnsweredCommand != null)
            {
                SelectAnswer = new MultiCommand(selectAnswerCommand, questionAnsweredCommand);
            }
            else
            {
                SelectAnswer = selectAnswerCommand;
            }

            ResetAnswer = new Command(
                (arg) =>
                {
                    if (SelectedItem != null)
                    {
                        SelectedItem.IsSelected = false;
                    }

                    SelectedItem = null;
                    isAnswered = false;
                    OnPropertyChanged(nameof(IsAnswered));
                    OnPropertyChanged(nameof(SelectedItem));
                    OnPropertyChanged(nameof(Feedback));

                    if (test == null)
                    {
                        ReportingService.ReportQuestionAsked(Model);
                    }
                },
                (arg) => SelectedItem == null);

            AdvanceStackCommand = nextCardCommand;

            this.test = test;
        }

        /// <summary>
        /// Gets the command to invoke when an answer option is selected.
        /// </summary>
        /// <value>The answer selection command.</value>
        public ICommand SelectAnswer { get; }

        /// <summary>
        /// Gets reset answer command.
        /// </summary>
        /// <value>The reset answer.</value>
        public ICommand ResetAnswer { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the question has been answered.
        /// </summary>
        /// <value><c>true</c> if the question has been answered, <c>false</c> otherwise.</value>
        public bool IsAnswered
        {
            get => isAnswered;
            set => SetField(ref isAnswered, value);
        }

        /// <summary>
        /// Gets the currently selected answer option.
        /// </summary>
        /// <value>The selected item.</value>
        public QuizOptionViewModel SelectedItem { get; private set; }

        /// <summary>
        /// Gets the feedback associated with the currently selected item.
        /// </summary>
        /// <value>The feedback.</value>
        public string Feedback => SelectedItem?.Feedback;

        /// <summary>
        /// Gets a list of answer options for the quiz item.
        /// </summary>
        /// <value>The answer options.</value>
        public IEnumerable<QuizOptionViewModel> Options => Model.Options.Select(option => new QuizOptionViewModel(this, option));

        /// <inheritdoc/>
        public ICommand AdvanceStackCommand { get; set; }

        /// <inheritdoc/>
        public bool IsStacked { get; set; }

        /// <inheritdoc/>
        public int? StackedPositionIndex { get; set; }

        /// <inheritdoc/>
        public int? TotalStackedCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether we should show the status header. _Currently_ we are only showing the status header for stackables.
        /// </summary>
        /// <value>
        /// A value indicating whether we should show the status header. _Currently_ we are only showing the status header for stackables.
        /// </value>
        public bool ShowStatusHeader => IsStacked;

        /// <summary>
        /// Gets a value indicating whether the quiz should show the retry button.
        /// </summary>
        /// <value>
        /// A value indicating whether the quiz should show the retry button.
        /// </value>
        public bool ShouldShowRetryButton => test == null && SelectedItem?.IsCorrect == false;

        /// <summary>
        /// Gets the title string.
        /// </summary>
        /// <value>
        /// The title string.
        /// </value>
        public string TitleString => test != null ? Strings.TypeTest : Strings.TypeQuiz;

        /// <summary>
        /// Gets the subtitle string.
        /// </summary>
        /// <value>
        /// The subtitle string.
        /// </value>
        public string SubtitleString => test?.Name ?? Model.Name;

        /// <summary>
        /// Gets the status string.
        /// </summary>
        /// <value>
        /// The status string.
        /// </value>
        public string StatusString => $"{StackedPositionIndex + 1} / {TotalStackedCount}";

        /// <summary>
        /// Gets the string to be used for the next button.
        /// </summary>
        /// <value>
        /// The string to be used for the next button.
        /// </value>
        public string NextButtonString => StackedPositionIndex + 1 >= TotalStackedCount ? Strings.TestResultsMessage : Strings.NextQuestionLabel;

        /// <summary>
        /// Gets the quiz model.
        /// </summary>
        /// <value>The quiz model.</value>
        protected new IQuiz Model => base.Model as IQuiz;

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(SelectedItem) when test == null:
                    OnPropertyChanged(nameof(ShouldShowRetryButton));
                    break;
            }
        }

        /// <inheritdoc />
        protected override void OnCardAppeared()
        {
            base.OnCardAppeared();
            if (!IsAnswered)
            {
                ReportingService.ReportQuestionAsked(Model, test);
            }
        }

        /// <inheritdoc />
        protected override void OnCardDisappeared()
        {
            if (!IsAnswered)
            {
                ReportingService.ReportQuestionSkipped(Model);
            }
        }
    }
}
