using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Course view model.
    /// </summary>
    public class CourseViewModel : CardViewModel, INamedVariableItemViewModel<TeaserViewModel>
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseViewModel"/> class.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public CourseViewModel(ICourse course, IAsyncCommand<IItem> downloadContentCommand) : base(course)
        {
            this.downloadContentCommand = downloadContentCommand;
        }

        /// <summary>
        /// Gets a collection of teaser view models for the course's learning objects.
        /// </summary>
        /// <value>View models representing the learning objects in the course.</value>
        public IEnumerable<TeaserViewModel> Elements
        {
            get
            {
                if (Model is not ICourse course || course.LearningObjects == null)
                {
                    return new List<TeaserViewModel>();
                }

                return course.LearningObjects.Select(item => new TeaserViewModel(item, downloadContentCommand));
            }
        }

        /// <summary>
        /// Gets the lessons label.
        /// </summary>
        /// <value>
        /// The lessons label.
        /// </value>
        [NotifyWhenPropertyChanges(nameof(PercentComplete))]
        public string LessonsLabel
        {
            get
            {
                string lessonsString = $"{Elements.Count()} {Strings.Lessons}";

                if (Elements.Count() == 1)
                {
                    lessonsString = $"{Elements.Count()} {Strings.Lesson}";
                }

                if (Elements.Any((arg) => arg.CompletionState.Completed))
                {
                    var completionString = $"{PercentComplete:N0}% {Strings.CompleteLabel}";
                    return $"{lessonsString} / {completionString}";
                }

                return lessonsString;
            }
        }

        /// <inheritdoc />
        public string EmptyLabel => Strings.DefaultEmptyMessage;

        /// <inheritdoc/>
        public string EmptyMessageTitle => string.Empty;

        /// <inheritdoc/>
        public string EmptyImageName => string.Empty;

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description => Model.Description;

        /// <summary>
        /// Gets the button text.
        /// </summary>
        /// <value>
        /// The button text.
        /// </value>
        public string ButtonText => PercentComplete > 0 ? Strings.ContinueLabel : Strings.StartCourseLabel;

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        /// <remarks>TODO: This should be internal.</remarks>
        public IItem ModelItem => Model;

        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Completed))]
        [NotifyWhenPropertyChanges(nameof(CompletionStateViewModel.Completed))]
        double PercentComplete => Elements.Count((arg) => arg.CompletionState.Completed) / (double)Elements.Count() * 100;
    }
}
