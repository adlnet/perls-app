using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model composed of episode view models.  This is used to set the ContentList of episodes presented in the collection view on the podcast detail page.
    /// </summary>
    public class EpisodeViewCollectionViewModel : RefreshableBaseCollectionViewModel<IItem, EpisodeViewModel>, IVariableItemViewModel<EpisodeViewModel>
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeViewCollectionViewModel"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskAction">Model collection task action.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public EpisodeViewCollectionViewModel(Func<Task<IEnumerable<IItem>>> modelCollectionTaskAction, IAsyncCommand<IItem> downloadContentCommand) : base(modelCollectionTaskAction)
        {
            this.downloadContentCommand = downloadContentCommand;
        }

        /// <inheritdoc />
        public string EmptyLabel => Strings.DefaultEmptyMessage;

        /// <inheritdoc/>
        public string EmptyMessageTitle => string.Empty;

        /// <inheritdoc/>
        public string EmptyImageName => "error";

        /// <inheritdoc />
        protected override EpisodeViewModel ConvertModelToViewModel(IItem model)
        {
            return model switch
            {
                IEpisode episode => new EpisodeViewModel(episode, downloadContentCommand),
                _ => throw new ArgumentNullException(nameof(model)),
            };
        }
    }
}
