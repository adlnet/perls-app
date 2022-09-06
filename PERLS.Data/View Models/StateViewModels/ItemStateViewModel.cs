using System;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Item state view model.
    /// </summary>
    public class ItemStateViewModel : BaseViewModel
    {
        readonly WeakReference<CorpusItemLearnerState> learnerState = new WeakReference<CorpusItemLearnerState>(default);

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemStateViewModel"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        public ItemStateViewModel(IItem item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));

            if (DependencyService.Get<ILearnerStateProvider>() is ILearnerStateProvider provider)
            {
                var state = provider.GetState(item);
                learnerState.SetTarget(state);

                // This _may_ need to be a weak event listener.
                if (learnerState.TryGetTarget(out CorpusItemLearnerState target))
                {
                    target.PropertyChanged += OnModelPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        protected IItem Item { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        protected CorpusItemLearnerState State
        {
            get
            {
                learnerState.TryGetTarget(out CorpusItemLearnerState target);
                return target;
            }
        }
    }
}
