using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.ViewModels;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Largely the same as <see cref="RefreshableBaseCollectionViewModel{TModel, TViewModel}"/> but it does not clear view models on sync.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object associated with the viewmodel in this collection.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewmodel in this collection.</typeparam>
    /// <remarks>
    /// This opts to more closely inspect for changes to view models in the collection, at the potential cost of strange bugs if a view model expected to be disposed but wasn't.
    /// Generally, this should be more performant and prevent "white screen" type flashes when new content loads, but has not yet been tested throughout the application.
    /// Where possible, this should be used in favor of its superclass.
    /// </remarks>
    public class RefreshableClearlessBaseCollectionViewModel<TModel, TViewModel> : RefreshableBaseCollectionViewModel<TModel, TViewModel>, IRefreshableViewModel, IList
        where TModel : INotifyPropertyChanged
        where TViewModel : ViewModel<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshableClearlessBaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskAction">Model collection task action.</param>
        public RefreshableClearlessBaseCollectionViewModel(Func<Task<IEnumerable<TModel>>> modelCollectionTaskAction) : base(modelCollectionTaskAction)
        {
        }

        /// <inheritdoc/>
        protected override void UpdateModels(ObservableCollection<TModel> liveModels, IEnumerable<TModel> newModels)
        {
            if (liveModels == null)
            {
                throw new ArgumentNullException(nameof(liveModels));
            }

            if (newModels == null)
            {
                throw new ArgumentNullException(nameof(newModels));
            }

            IsUpdating = true;

            var modelsToRemove = liveModels.Except(newModels).ToList();
            var modelsToAdd = newModels.Except(liveModels).ToList();

            foreach (var oldModel in modelsToRemove)
            {
                liveModels.Remove(oldModel);
            }

            foreach (var newModel in modelsToAdd)
            {
                liveModels.Add(newModel);
            }

            // since we added new models to the live collection without checking their position,
            // it's possible that models are now out of order. we can only fix this if given a list
            if (newModels is IList<TModel> list)
            {
                // iterate over all indices in the new model list
                for (var newIndex = 0; newIndex < list.Count; newIndex++)
                {
                    // get the model at this index in the new model list
                    var model = list[newIndex];

                    // get the index of this model in the current list
                    var currentIndex = liveModels.IndexOf(model);

                    // in theory, this should be unlikely
                    if (currentIndex < 0)
                    {
                        continue;
                    }

                    // if the indices match, we don't need to move
                    if (currentIndex == newIndex)
                    {
                        continue;
                    }

                    // move the model that was at currentIndex to its new position
                    liveModels.Move(currentIndex, newIndex);
                }
            }

            IsUpdating = false;

            if (modelsToRemove.Any() || modelsToAdd.Any())
            {
                OnElementsChanged();
            }
        }
    }
}
