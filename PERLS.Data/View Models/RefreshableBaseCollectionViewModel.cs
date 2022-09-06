using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Cache;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Refreshable base collection view model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object associated with the viewmodel in this collection.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewmodel in this collection.</typeparam>
    /// <remarks>
    /// <see cref="Refresh"/> needs to be called to actually populate data into the model.
    /// This is the responsibility of the caller to determine when the right time to load the data is.
    /// </remarks>
    public class RefreshableBaseCollectionViewModel<TModel, TViewModel> : BaseCollectionViewModel<TModel, TViewModel>, IRefreshableViewModel, IList where TModel : INotifyPropertyChanged where TViewModel : ViewModel<TModel>
    {
        readonly Func<Task<IEnumerable<TModel>>> modelCollectionTaskAction;
        INotifyCollectionChanged observedCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshableBaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskAction">Model collection task action.</param>
        public RefreshableBaseCollectionViewModel(Func<Task<IEnumerable<TModel>>> modelCollectionTaskAction) : base(new ObservableCollection<TModel>())
        {
            this.modelCollectionTaskAction = modelCollectionTaskAction;
        }

        /// <inheritdoc />
        public bool IsFixedSize => ElementsList.IsFixedSize;

        /// <inheritdoc />
        public bool IsReadOnly => ElementsList.IsReadOnly;

        /// <inheritdoc />
        public int Count => ElementsList.Count;

        /// <inheritdoc />
        public bool IsSynchronized => ElementsList.IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => ElementsList.SyncRoot;

        /// <inheritdoc />
        public bool IsCacheDerived { get; private set; }

        /// <summary>
        /// Gets or sets the paged response. Is only set if the models retrieved are a paged response.
        /// </summary>
        /// <value>The paged response.</value>
        public IPagedResponse<TModel> PagedResponse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an update is in progress.
        /// </summary>
        /// <value><c>true</c> if an update is in progress, <c>false</c> otherwise.</value>
        protected bool IsUpdating { get; set; }

        IList ElementsList => Elements as IList;

        /// <inheritdoc />
        public object this[int index]
        {
            // in Forms 5, there is a crash when a collection becomes empty
            // we opened an issue with Xamarin here: https://github.com/xamarin/Xamarin.Forms/issues/14171
            // once that is fixed, the getter here should just return `ElementsList[index]` without the try/catch
            get
            {
                try
                {
                    return ElementsList[index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    // returning null here would cause a Forms exception
                    return new object();
                }
            }
            set => ElementsList[index] = value;
        }

        /// <inheritdoc />
        public virtual async Task Refresh()
        {
            if (modelCollectionTaskAction != null)
            {
                var models = await modelCollectionTaskAction.Invoke().ConfigureAwait(false);

                if (models is IPagedResponse<TModel> pagedResponse)
                {
                    PagedResponse = pagedResponse;
                }

                IsCacheDerived = models is CacheDerivedItems;

                // _Presumably_ if we have an observable collection,
                // then we'll already have the updated data at this point.
                if (observedCollection == null)
                {
                    SyncModels(models);
                }
            }
        }

        /// <inheritdoc />
        public int Add(object value)
        {
            return ElementsList.Add(value);
        }

        /// <inheritdoc />
        public bool Contains(object value)
        {
            return ElementsList.Contains(value);
        }

        /// <inheritdoc />
        public int IndexOf(object value)
        {
            return ElementsList.IndexOf(value);
        }

        /// <inheritdoc />
        public void Insert(int index, object value)
        {
            ElementsList.Insert(index, value);
        }

        /// <inheritdoc />
        public void Remove(object value)
        {
            ElementsList.Remove(value);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            ElementsList.RemoveAt(index);
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            ElementsList.CopyTo(array, index);
        }

        /// <inheritdoc />
        protected override void OnElementsChanged()
        {
            if (IsUpdating)
            {
                return;
            }

            base.OnElementsChanged();
        }

        /// <summary>
        /// Update the models displayed in this collection.
        /// </summary>
        /// <remarks>
        /// The current implementation is naïve and just clears the live models and adds all the new models in.
        /// Subclasses should set `IsUpdating` to true while the update occurs.
        /// Subclasses should call `OnElementsChanged` if the collection has changed.
        /// </remarks>
        /// <param name="liveModels">The current live models; subclasses should modify this object to add or remove items.</param>
        /// <param name="newModels">All the models from the model collection task action (not just models that are new to the collection).</param>
        protected virtual void UpdateModels(ObservableCollection<TModel> liveModels, IEnumerable<TModel> newModels)
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
            liveModels.Clear();

            foreach (var model in newModels)
            {
                liveModels.Add(model);
            }

            IsUpdating = false;
            OnElementsChanged();
        }

        void SyncModels(IEnumerable<TModel> models)
        {
            if (models == null)
            {
                return;
            }

            if (models is INotifyCollectionChanged liveCollection)
            {
                ObserveModelCollection(liveCollection);
            }

            if (Models is ObservableCollection<TModel> collection)
            {
                UpdateModels(collection, models);
            }
        }

        void ObserveModelCollection(INotifyCollectionChanged liveCollection)
        {
            if (liveCollection == observedCollection)
            {
                return;
            }

            if (observedCollection != null)
            {
                observedCollection.CollectionChanged -= HandleCollectionChanged;
            }

            liveCollection.CollectionChanged += HandleCollectionChanged;
            observedCollection = liveCollection;
        }

        void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is IEnumerable<TModel> models)
            {
                SyncModels(models);
            }
        }
    }
}
