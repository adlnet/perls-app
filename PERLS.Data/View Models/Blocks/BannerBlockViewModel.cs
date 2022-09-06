using System;
using System.Windows.Input;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels.Blocks
{
    /// <summary>
    /// The prompt block view model.
    /// </summary>
    public class BannerBlockViewModel : BlockViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BannerBlockViewModel"/> class.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="selectBannerCommand">The banner's selection command.</param>
        public BannerBlockViewModel(IBlock block, ICommand selectBannerCommand) : base(block)
        {
            BannerSelectedCommand = selectBannerCommand;
        }

        /// <summary>
        /// Gets the banner selection command.
        /// </summary>
        /// <value>The banner selection command.</value>
        public ICommand BannerSelectedCommand { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message => Model.ContentsDictionary.Text;

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public string Image => Model.ContentsDictionary.Icon.AbsoluteUri;

        /// <inheritdoc/>
        public override void Refresh()
        {
        }
    }
}
