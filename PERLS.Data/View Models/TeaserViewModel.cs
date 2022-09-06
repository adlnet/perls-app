using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Svg.Forms;
using PERLS.Data.Commands;
using PERLS.Data.Converters;
using PERLS.Data.Definition;
using PERLS.Data.Infrastructure;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Represents any item for previewing to the user.
    /// </summary>
    public class TeaserViewModel : CardViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeaserViewModel"/> class.
        /// </summary>
        /// <param name="item">The content item.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public TeaserViewModel(IItem item, IAsyncCommand<IItem> downloadContentCommand = null) : base(item, downloadContentCommand)
        {
            SetValues();
        }

        /// <summary>
        /// Gets a description associated with the item.
        /// </summary>
        /// <value>The associated description.</value>
        public string Description => Model.Description;

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <value>The link.</value>
        public Uri Url => Model.Url;

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        /// <remarks>TODO: This should be internal.</remarks>
        public IItem ModelItem => Model;

        /// <summary>
        /// Gets a value indicating whether a gradient should be applied. Dependent on card type.
        /// </summary>
        /// <value>A bool value.</value>
        public bool ShouldShowGradient { get; internal set; }

        /// <summary>
        /// Gets the color of the lower half of the gradient. The top half will always be a semi-transparent black.
        /// </summary>
        /// <value>The color of the lower half of the gradient.</value>
        public Color GradientColor { get; internal set; }

        /// <summary>
        /// Gets the centered image of a tile.
        /// </summary>
        /// <value>The centered image.</value>
        public SvgImageSource CenterIcon { get; internal set; }

        /// <summary>
        /// Gets the centered text of a tile.
        /// </summary>
        /// <value>The centered text.</value>
        public string CenterText { get; internal set; }

        /// <summary>
        /// Based on model type, sets the various fields for the tile's layout.
        /// </summary>
        void SetValues()
        {
            switch (ModelItem)
            {
                case ICourse course:
                    ShouldShowGradient = true;
                    GradientColor = (Color)Application.Current.Resources["CourseCardColor"];
                    CenterIcon = SvgImageSource.FromResource("PERLS.Data.Resources.course_icon.svg");
                    var provider = DependencyService.Get<ILearnerStateProvider>();
                    var courseObjects = course.LearningObjects;
                    CenterText = $"{(courseObjects != null ? courseObjects.Where((arg) => provider.GetState(arg).Completed == CorpusItemLearnerState.Status.Enabled).Count() : 0)} / {(courseObjects != null ? courseObjects.Count() : 0)} {Strings.Lessons} {Strings.CompleteLabel}";
                    break;
                case IPodcast podcast:
                    ShouldShowGradient = true;
                    GradientColor = (Color)Application.Current.Resources["PodcastCardColor"];
                    CenterIcon = SvgImageSource.FromResource("PERLS.Data.Resources.podcast_icon.svg");
                    var podcasts = podcast?.EpisodesCount;
                    CenterText = $"{(podcasts != null ? podcasts : 0)} {(podcasts == 1 ? Strings.Episode : Strings.Episodes)}";
                    break;
                case IFlashcard flashcard:
                    ShouldShowGradient = false;
                    GradientColor = Color.Transparent;
                    CenterIcon = SvgImageSource.FromResource("PERLS.Data.Resources.flashcard_icon.svg");
                    CenterText = null;
                    break;
                case IQuiz quiz:
                    ShouldShowGradient = false;
                    GradientColor = Color.Transparent;
                    CenterIcon = SvgImageSource.FromResource("PERLS.Data.Resources.quiz_icon.svg");
                    CenterText = null;
                    break;
                case ITip tip:
                    ShouldShowGradient = false;
                    GradientColor = Color.Transparent;
                    CenterIcon = SvgImageSource.FromResource("PERLS.Data.Resources.tip_icon.svg");
                    CenterText = null;
                    break;
                default:
                    ShouldShowGradient = true;
                    GradientColor = Color.Transparent;
                    CenterText = null;
                    CenterIcon = ModelItem.Type == NodeTypeConstants.EventItem ? SvgImageSource.FromResource("PERLS.Data.Resources.event_icon.svg") : null;
                    break;
            }
        }
    }
}
