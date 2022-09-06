using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The prompt collection view model.
    /// </summary>
    public class PromptCollectionViewModel : BaseViewModel
    {
        int selectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptCollectionViewModel"/> class.
        /// </summary>
        /// <param name="promptsTask">The collection of prompts task.</param>
        public PromptCollectionViewModel(Task<IEnumerable<IPrompt>> promptsTask)
        {
            if (promptsTask == null)
            {
                throw new ArgumentNullException(nameof(promptsTask));
            }

            var prompts = promptsTask.ContinueWith(
                (task) =>
            {
                SetupFromPrompts(task.Result);
            }, TaskScheduler.Current);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptCollectionViewModel"/> class.
        /// </summary>
        /// <param name="prompts">The collection of prompts.</param>
        public PromptCollectionViewModel(IEnumerable<IPrompt> prompts)
        {
            if (prompts == null)
            {
                throw new ArgumentNullException(nameof(prompts));
            }

            SetupFromPrompts(prompts);
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
            set => SetField(ref selectedIndex, value);
        }

        /// <summary>
        /// Gets the empty label.
        /// </summary>
        /// <value>
        /// The empty label.
        /// </value>
        public string EmptyLabel => string.Empty;

        /// <summary>
        /// Gets or sets the Elements.
        /// </summary>
        /// <value>
        /// The Elements.
        /// </value>
        public IEnumerable<BaseViewModel> Elements { get; protected set; }

        void SetupFromPrompts(IEnumerable<IPrompt> prompts)
        {
            var nextCardCommand = new Command((arg) =>
            {
                SelectedIndex++;
            });

            var promptViewModels = prompts.Select((arg) => new PromptViewModel(arg, nextCardCommand: nextCardCommand));
            var elements = new List<BaseViewModel>();
            elements.AddRange(promptViewModels);

            if (promptViewModels.Any())
            {
                elements.Add(new PromptResultViewModel());
            }

            Elements = elements;

            SelectedIndex = 0;
        }
    }
}
