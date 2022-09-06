using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Prompt Option View Model.
    /// </summary>
    public class PromptOptionViewModel : Float.Core.ViewModels.SelectableViewModel<IPromptOption>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptOptionViewModel"/> class.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="promptOption">The option.</param>
        public PromptOptionViewModel(PromptViewModel prompt, IPromptOption promptOption) : base(promptOption)
        {
            Option = promptOption;
            Prompt = prompt;
        }

        /// <summary>
        /// Gets the prompt.
        /// </summary>
        /// <value>
        /// The prompt.
        /// </value>
        public PromptViewModel Prompt { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => Model.Value;

        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        /// <value>s
        /// The option.
        /// </value>
        public IPromptOption Option { get; protected set; }
    }
}
