using FFImageLoading.Svg.Forms;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Completion state view model.
    /// </summary>
    public class CompletionStateViewModel : ItemStateViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionStateViewModel"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        public CompletionStateViewModel(IItem item) : base(item)
        {
        }

        /// <summary>
        /// Gets the completion icon.
        /// </summary>
        /// <value>The completion icon.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Completed))]
        public SvgImageSource CompletionIcon
        {
            get
            {
                if (Completed)
                {
                    return SvgImageSource.FromResource("PERLS.Data.Resources.lo_complete.svg");
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CompletionStateViewModel"/> is completed.
        /// </summary>
        /// <value><c>true</c> if completed; otherwise, <c>false</c>.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Completed))]
        public virtual bool Completed => State?.Completed == CorpusItemLearnerState.Status.Enabled;
    }
}
