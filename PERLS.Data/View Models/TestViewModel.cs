using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A Tests Attempt View Model.
    /// </summary>
    public class TestViewModel : CardViewModel, INamedVariableItemViewModel<CardViewModel>
    {
        int selectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public TestViewModel(ITest model) : base(model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var cards = new List<CardViewModel>();

            var nextCardCommand = new Command(() => SelectedIndex++);
            var questionAnsweredCommand = new Command<QuizOptionViewModel>(HandleQuestionAnswered);
            var startOverCommand = new Command(StartOver);

            foreach (var eachQuestion in model.Questions)
            {
                var quiz = new QuizViewModel(eachQuestion, questionAnsweredCommand, nextCardCommand, model)
                {
                    IsStacked = true,
                    StackedPositionIndex = cards.Count,
                    TotalStackedCount = model.Questions.Count(),
                };
                cards.Add(quiz);
            }

            var results = new TestResultsViewModel(model, startOverCommand);
            cards.Add(results);
            Elements = cards;

            // If the test is already complete, jump to the results.
            if (CompletionState.Completed)
            {
                SelectedIndex = cards.Count - 1;
            }
            else
            {
                SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets or sets the Selected Index.
        /// </summary>
        /// <value>
        /// The Selected Index.
        /// </value>
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                var oldCard = SelectedCard;
                SetField(ref selectedIndex, value);
                OnStackAdvanced(oldCard, SelectedCard);
            }
        }

        /// <summary>
        /// Gets the currently visible card.
        /// </summary>
        /// <value>The currently visible card.</value>
        public CardViewModel SelectedCard => Elements.ElementAt(SelectedIndex);

        /// <summary>
        /// Gets a collection of teaser view models for the test's questions.
        /// </summary>
        /// <value>View models representing the questions in the test.</value>
        public IEnumerable<CardViewModel> Elements { get; }

        /// <inheritdoc />
        public string EmptyLabel => Strings.DefaultEmptyMessage;

        /// <inheritdoc/>
        public string EmptyImageName => "error";

        /// <inheritdoc/>
        public string EmptyMessageTitle => string.Empty;

        /// <inheritdoc/>
        protected override void OnCardAppeared()
        {
            base.OnCardAppeared();

            // When the test appears (and hasn't already been started), record a new attempt.
            if (selectedIndex == 0 && SelectedCard is QuizViewModel question && !question.IsAnswered)
            {
                ReportingService.ReportTestAttempted((ITest)Model);
            }

            OnStackAdvanced(null, SelectedCard);
        }

        /// <inheritdoc />
        protected override void OnCardDisappeared()
        {
            OnStackAdvanced(SelectedCard, null);
            base.OnCardDisappeared();
        }

        /// <summary>
        /// Invoked when the card stack advances to the next card.
        /// </summary>
        /// <param name="oldCard">The card that was previously visible.</param>
        /// <param name="newCard">The newly visible card.</param>
        protected virtual void OnStackAdvanced(CardViewModel oldCard, CardViewModel newCard)
        {
            if (!IsVisible || oldCard == newCard)
            {
                return;
            }

            oldCard?.CardDisappearedCommand?.Execute(this);
            newCard?.CardAppearedCommand?.Execute(this);
        }

        void StartOver()
        {
            // Send attempted statement when you reset stack.
            ReportingService.ReportTestAttempted((ITest)Model);

            // In the mobile app, a test completion state is determined by the status of the most recent attempt.
            // Since we're starting a new attempt, we should proactively update the cached completion state of the test.
            DependencyService.Get<ILearnerStateProvider>().GetState(Model).Completed = CorpusItemLearnerState.Status.Disabled;

            foreach (var question in Elements.OfType<QuizViewModel>())
            {
                question.ResetAnswer?.Execute(null);
            }

            SelectedIndex = 0;
        }

        void HandleQuestionAnswered(QuizOptionViewModel response)
        {
            var lastQuestion = Elements.OfType<QuizViewModel>().LastOrDefault();
            if (response.Quiz == lastQuestion)
            {
                HandleTestCompleted();
            }
        }

        void HandleTestCompleted()
        {
            var test = Model as ITest;

            var questions = Elements.OfType<QuizViewModel>();
            var correctAnswers = questions.Count((arg) => arg.SelectedItem?.IsCorrect ?? false);

            DependencyService.Get<ILearnerStateProvider>().SaveTestResult(test, (uint)correctAnswers, Duration)
                .OnFailure(task =>
                {
                    // Do nothing.
                });
        }
    }
}
