using System;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.StateViewModels
{
    /// <summary>
    /// Term state view model.
    /// </summary>
    public class TermStateViewModel : BaseViewModel
    {
        readonly WeakReference<TaxonomyTermLearnerState> learnerState = new WeakReference<TaxonomyTermLearnerState>(default);

        /// <summary>
        /// Initializes a new instance of the <see cref="TermStateViewModel"/> class.
        /// </summary>
        /// <param name="term">Term.</param>
        public TermStateViewModel(ITaxonomyTerm term)
        {
            Term = term ?? throw new ArgumentNullException(nameof(term));
            Provider = DependencyService.Get<ILearnerStateProvider>();

            learnerState.SetTarget(Provider.GetState(term));

            // This _may_ need to be a weak event listener.
            if (learnerState.TryGetTarget(out TaxonomyTermLearnerState target))
            {
                target.PropertyChanged += OnModelPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        protected ITaxonomyTerm Term { get; }

        /// <summary>
        /// Gets the learner state provider.
        /// </summary>
        /// <value>The learner state provider.</value>
        protected ILearnerStateProvider Provider { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        protected TaxonomyTermLearnerState State
        {
            get
            {
                learnerState.TryGetTarget(out TaxonomyTermLearnerState target);
                return target;
            }
        }
    }
}
