using System;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Podcast view model.
    /// </summary>
    public class PodcastViewModel : CardViewModel
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastViewModel"/> class.
        /// </summary>
        /// <param name="podcast">The podcast.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public PodcastViewModel(IPodcast podcast, IAsyncCommand<IItem> downloadContentCommand) : base(podcast)
        {
            this.downloadContentCommand = downloadContentCommand ?? throw new ArgumentNullException(nameof(downloadContentCommand));
        }

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        public IItem ModelItem => Model;

        /// <summary>
        /// Gets the podcasts's title.
        /// </summary>
        /// <value>The model name.</value>
        public string Title => Model.Name;

        /// <summary>
        /// Gets the podcast's description.
        /// </summary>
        /// <value>The model description.</value>
        public string Description => Model.Description;

        /// <summary>
        /// Gets the podcast's id.
        /// </summary>
        /// <value>The model id.</value>
        public Guid Id => Model.Id;
    }
}
