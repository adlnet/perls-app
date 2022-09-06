using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.Sections
{
    /// <summary>
    /// A view model for a section, composed of multiple blocks.
    /// </summary>
    public abstract class SectionViewModel : ViewModel<ISection>, IEmptyCollectionViewModel
    {
        Exception error;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionViewModel"/> class.
        /// </summary>
        /// <param name="section">The section to represent in this view model.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public SectionViewModel(ISection section, ICommand selectItemCommand) : base(section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Blocks = new BlockCollectionViewModel(section.Blocks, selectItemCommand);
            RefreshCommand = new Command(Refresh);
        }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        /// <value>
        /// The refresh command.
        /// </value>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Gets the error in this section, or an error from an inner block, if one exists.
        /// </summary>
        /// <value>The error related to this section.</value>
        public Exception Error
        {
            get
            {
                if (error is Exception ownError)
                {
                    return ownError;
                }
                else if (Blocks.Select(vm => vm.Error).FirstOrDefault() is Exception blockError)
                {
                    return blockError;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is an error.
        /// </summary>
        /// <value><c>true</c> if there is an error, <c>false</c> otherwise.</value>
        public bool IsError => Error != null;

        /// <summary>
        /// Gets a value indicating whether this section is loading.
        /// </summary>
        /// <value><c>true</c> if any block in this section is loading, <c>false</c> otherwise.</value>
        public bool IsLoading => Blocks.Select(block => block.IsLoading).Any();

        /// <summary>
        /// Gets the blocks to display in this section.
        /// </summary>
        /// <value>An enumerable of blocks.</value>
        public BlockCollectionViewModel Blocks { get; }

        /// <summary>
        /// Gets the error title.
        /// </summary>
        /// <value>
        /// The error title.
        /// </value>
        public string ErrorTitle => Strings.DefaultErrorTitle;

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage => Error?.Message;

        /// <inheritdoc />
        public string EmptyLabel
        {
            get
            {
                if (Error is AggregateException ae && ae.InnerException?.Message is string msg)
                {
                    return msg;
                }
                else
                {
                    return Error?.Message ?? Strings.UnknownErrorMessage;
                }
            }
        }

        /// <inheritdoc />
        public string EmptyMessageTitle => Strings.EmptyViewErrorTitle;

        /// <inheritdoc />
        public string EmptyImageName => IsError ? "error" : "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data";

        /// <summary>
        /// Gets a value indicating whether this section is empty.
        /// </summary>
        /// <value>
        /// A value indicating whether this section is empty.
        /// </value>
        public bool IsEmpty => !Blocks.Any();

        /// <summary>
        /// A method to refresh the blocks in this section.
        /// </summary>
        public virtual void Refresh()
        {
            Blocks.Refresh().ContinueWith(
                task =>
                {
                    error = task.Exception;
                    OnPropertyChanged(nameof(ErrorMessage));
                    OnPropertyChanged(nameof(EmptyLabel));
                }, TaskScheduler.Current);
        }
    }
}
