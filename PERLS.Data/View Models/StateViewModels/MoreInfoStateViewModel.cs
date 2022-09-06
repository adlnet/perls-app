using FFImageLoading.Svg.Forms;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// More info state view model.
    /// </summary>
    public class MoreInfoStateViewModel : ItemStateViewModel
    {
        readonly IItem model;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreInfoStateViewModel"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        public MoreInfoStateViewModel(IItem item) : base(item)
        {
            model = item;
        }

        /// <summary>
        /// Gets the recommendation icon.
        /// </summary>
        /// <value>The recommendation icon.</value>
        public SvgImageSource RecommendationIcon => SvgImageSource.FromResource("PERLS.Data.Resources.info.svg");

        /// <summary>
        /// Gets the recommendation reason.
        /// </summary>
        /// <value>The recommendation reason.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.RecommendationReason))]
        public string RecommendationReason => State?.RecommendationReason ?? string.Empty;

        /// <summary>
        /// Gets the close icon.
        /// </summary>
        /// <value>The close icon.</value>
        public SvgImageSource CloseIcon => SvgImageSource.FromResource("PERLS.Data.Resources.close.svg");

        /// <summary>
        /// Gets the recommendation title.
        /// </summary>
        /// <value>The recommendation title.</value>
        public string RecommendationTitle => Strings.RecommendationPopupTitle;

        /// <summary>
        /// Gets a value indicating whether this <see cref="MoreInfoStateViewModel"/> has recommendation.
        /// </summary>
        /// <value><c>true</c> if has recommendation; otherwise, <c>false</c>.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.RecommendationReason))]
        public bool HasRecommendation => !string.IsNullOrEmpty(RecommendationReason);

        /// <summary>
        /// Gets a value indicating whether this <see cref="MoreInfoStateViewModel"/> has descriptions.
        /// </summary>
        /// <value>
        /// A value indicating whether this <see cref="MoreInfoStateViewModel"/> has descriptions.
        /// </value>
        public bool HasDescription => model is not IFlashcard && model is not ITip && !string.IsNullOrEmpty(model.Description);

        /// <summary>
        /// Gets a value indicating whether this <see cref="MoreInfoStateViewModel"/> has recommendation.
        /// </summary>
        /// <value><c>true</c> if has recommendation; otherwise, <c>false</c>.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.RecommendationReason))]
        public bool HasMoreInfo => HasRecommendation || HasDescription;

        /// <summary>
        /// Gets the description title.
        /// </summary>
        /// <value>
        /// The description title.
        /// </value>
        public string DescriptionTitle => Strings.CourseDescriptionLabel;

        /// <summary>
        /// Gets the description text.
        /// </summary>
        /// <value>
        /// The description text.
        /// </value>
        public string DescriptionText => model.Description;
    }
}
