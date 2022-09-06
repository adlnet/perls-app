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
    /// Subclass to handle loading more items of a paged response.
    /// </summary>
    /// <typeparam name="TModel">The type of the model object associated with the viewmodel in this collection.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewmodel in this collection.</typeparam>
    public class PagedResponseBaseCollectionViewModel<TModel, TViewModel> : RefreshableBaseCollectionViewModel<TModel, TViewModel> where TModel : IItem where TViewModel : ViewModel<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResponseBaseCollectionViewModel{TModel, TViewModel}"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskAction">Model collection task action.</param>
        public PagedResponseBaseCollectionViewModel(Func<Task<IEnumerable<TModel>>> modelCollectionTaskAction) : base(modelCollectionTaskAction)
        {
        }

        /// <summary>
        /// Task to load more items on the page.
        /// </summary>
        /// <returns>A task to load more items.</returns>
        public virtual async Task<IEnumerable<TViewModel>> LoadMore()
        {
            var response = await DependencyService.Get<IPagedResponseHandler>().LoadMore(PagedResponse).ConfigureAwait(false);

            PagedResponse = response;

            return response.Select(ConvertModelToViewModel);
        }
    }
}
