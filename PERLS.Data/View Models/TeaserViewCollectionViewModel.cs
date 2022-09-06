using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model composed of teaser view models.
    /// </summary>
    public class TeaserViewCollectionViewModel : PagedResponseBaseCollectionViewModel<IItem, TeaserViewModel>, IVariableItemViewModel<TeaserViewModel>
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeaserViewCollectionViewModel"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskAction">Model collection task action.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public TeaserViewCollectionViewModel(Func<Task<IEnumerable<IItem>>> modelCollectionTaskAction, IAsyncCommand<IItem> downloadContentCommand) : base(modelCollectionTaskAction)
        {
            this.downloadContentCommand = downloadContentCommand;
        }

        /// <inheritdoc />
        public string EmptyLabel => Strings.DefaultEmptyMessage;

        /// <inheritdoc/>
        public string EmptyImageName => null;

        /// <inheritdoc/>
        public string EmptyMessageTitle => string.Empty;

        /// <inheritdoc />
        protected override TeaserViewModel ConvertModelToViewModel(IItem model)
        {
            return new TeaserViewModel(model, downloadContentCommand);
        }
    }
}
