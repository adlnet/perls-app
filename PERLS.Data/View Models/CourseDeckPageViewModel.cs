using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Extensions;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The View Model for when a course launches to the deck page.
    /// </summary>
    public class CourseDeckPageViewModel : CardDeckPageViewModel
    {
        readonly ICourse course;
        Task updateStateTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseDeckPageViewModel"/> class.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="selectItemCommand">The select item command.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public CourseDeckPageViewModel(ICourse course, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand)
            : base(course?.Name, course?.LearningObjects, selectItemCommand, downloadContentCommand, Strings.EmptyCourseMessageLabel, Strings.EmptyCourseTitleLabel, "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data")
        {
            this.course = course ?? throw new ArgumentNullException(nameof(course));
            BookmarkState = new BookmarkStateViewModel(course);
            if (!string.IsNullOrWhiteSpace(course.Description))
            {
                Header = new CourseViewModel(course, downloadContentCommand);
            }
        }

        /// <summary>
        /// Gets the current bookmark state.
        /// </summary>
        /// <value>The bookmark state.</value>
        public BookmarkStateViewModel BookmarkState { get; }

        /// <summary>
        /// Gets the course view model.
        /// </summary>
        /// <value>
        /// The course view model.
        /// </value>
        public CourseViewModel Header { get; }

        /// <summary>
        /// Gets the completion label.
        /// </summary>
        /// <value>
        /// The completion label.
        /// </value>
        public string CompletedLabel
        {
            get
            {
                var completedCount = Deck.Elements.Count(cvm => cvm.CompletionState.Completed);
                var totalCount = Deck.Elements.Count();

                if (completedCount == 0 && totalCount == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return $"{completedCount} / {totalCount} {Strings.StatLabelCompleted}";
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Typically, this would call out to the source of data for this model
        /// and get the latest data. However, our source of data is a static list
        /// of learning objects--that cannot be refreshed. So instead of
        /// attempting to refresh the list of contents, we only refresh the
        /// _state_ of those contents.
        /// </remarks>
        public override void Refresh()
        {
            UpdateCourseState();
        }

        /// <inheritdoc />
        protected override void OnObservingBegan()
        {
            base.OnObservingBegan();

            course.LearningObjects
                .Select(item => DependencyService.Get<ILearnerStateProvider>().GetState(item))
                .ForEach(state => state.PropertyChanged += HandleLearningObjectStateChanged);

            course.LearningObjects
                .OfType<ITest>()
                .Select(item => DependencyService.Get<ILearnerStateProvider>().GetState(item))
                .ForEach(state => state.PropertyChanged += HandleTestObjectStateChanged);
        }

        /// <inheritdoc />
        protected override void OnObservingEnded()
        {
            base.OnObservingEnded();

            course.LearningObjects
                .Select(item => DependencyService.Get<ILearnerStateProvider>().GetState(item))
                .ForEach(state => state.PropertyChanged -= HandleLearningObjectStateChanged);

            course.LearningObjects
                .OfType<ITest>()
                .Select(item => DependencyService.Get<ILearnerStateProvider>().GetState(item))
                .ForEach(state => state.PropertyChanged -= HandleTestObjectStateChanged);
        }

        void HandleLearningObjectStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CorpusItemLearnerState.Completed))
            {
                OnPropertyChanged(nameof(CompletedLabel));
            }
        }

        void HandleTestObjectStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is CorpusItemLearnerState state && state.Completed == CorpusItemLearnerState.Status.Enabled)
            {
                UpdateCourseState();
            }
        }

        void UpdateCourseState()
        {
            if (updateStateTask != null)
            {
                return;
            }

            var items = new List<IItem> { course };
            items.AddRange(course.LearningObjects);

            if (!Deck.Elements.Any((arg) => !arg.CompletionState.Completed))
            {
                DependencyService.Get<ILearnerStateProvider>().GetState(course).MarkAsComplete();
            }

            updateStateTask = DependencyService.Get<ILearnerStateProvider>().GetStateList(items)
                .ContinueWith(
                task =>
                {
                    updateStateTask = null;

                    if (task.Exception != null)
                    {
                        DependencyService.Get<AnalyticsService>().TrackException(task.Exception);
                    }
                }, TaskScheduler.Current);
        }
    }
}
