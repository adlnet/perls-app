using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Prompt view model.
    /// </summary>
    public class PromptViewModel : ViewModel<IPrompt>, IAdvanceableStackViewModel
    {
        bool isAnswered;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="nextCardCommand">The goto next card command.</param>
        public PromptViewModel(IPrompt model, ICommand nextCardCommand = null) : base(model)
        {
            var reportingService = DependencyService.Get<IReportingService>();

            SelectPrompt = new Command(
                (arg) =>
                {
                    if (arg is PromptOptionViewModel option)
                    {
                        option.IsSelected = true;
                        IsAnswered = true;
                        SelectedItem = option;
                        OnPropertyChanged(nameof(SelectedItem));
                        reportingService.ReportPromptAnswered(model, option.Option);
                    }
                },
                (arg) => !IsAnswered);

            AdvanceStackCommand = nextCardCommand;
            reportingService.ReportPromptAsked(model);
        }

        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public string Question => Model.Question;

        /// <summary>
        /// Gets the command to invoke when an option is selected.
        /// </summary>
        /// <value>The answer prompt command.</value>
        public ICommand SelectPrompt { get; }

        /// <summary>
        /// Gets a list of answer options for the quiz item.
        /// </summary>
        /// <value>The answer options.</value>
        public IEnumerable<PromptOptionViewModel> Options => Model.Options.Select(option => new PromptOptionViewModel(this, option));

        /// <summary>
        /// Gets or sets a value indicating whether the question has been answered.
        /// </summary>
        /// <value><c>true</c> if the question has been answered, <c>false</c> otherwise.</value>
        public bool IsAnswered
        {
            get => isAnswered;
            set
            {
                SetField(ref isAnswered, value);
                AdvanceStackCommand?.Execute(this);
            }
        }

        /// <summary>
        /// Gets the currently selected answer option.
        /// </summary>
        /// <value>The selected item.</value>
        public PromptOptionViewModel SelectedItem { get; private set; }

        /// <inheritdoc/>
        public ICommand AdvanceStackCommand { get; set; }

        /// <inheritdoc/>
        public bool IsStacked { get; set; }

        /// <inheritdoc/>
        public int? StackedPositionIndex { get; set; }

        /// <inheritdoc/>
        public int? TotalStackedCount { get; set; }
    }
}
