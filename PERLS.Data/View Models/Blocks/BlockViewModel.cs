using System;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels.Blocks
{
    /// <summary>
    /// A view model to represent a single block and its contents.
    /// </summary>
    public abstract class BlockViewModel : ViewModel<IBlock>, IEmptyCollectionViewModel
    {
        bool isVisible;
        bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockViewModel"/> class.
        /// </summary>
        /// <param name="block">The block to represent in this view model.</param>
        protected BlockViewModel(IBlock block) : base(block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this block should be visible in the UI.
        /// </summary>
        /// <remarks>
        /// This is primarily to control which sections are visible in a tabbed section view.
        /// </remarks>
        /// <value><c>true</c> if the block is visible, <c>false</c> otherwise.</value>
        public bool IsVisible
        {
            get => isVisible;
            set => SetField(ref isVisible, value);
        }

        /// <summary>
        /// Gets a value indicating whether this has more items.
        /// </summary>
        /// <value>
        /// A value indicating whether this has more items.
        /// </value>
        public bool HasMore => Model.More != null;

        /// <summary>
        /// Gets the Uri to load More.
        /// </summary>
        /// <value>
        /// The Uri to load More.
        /// </value>
        public Uri MoreUri => Model.More;

        /// <summary>
        /// Gets the URL to invoke in response to user action (such as opening a prompt).
        /// </summary>
        /// <value>The action URI.</value>
        public Uri ActionUri => Model.ContentsDictionary?.Url;

        /// <summary>
        /// Gets or sets the selection changed command.
        /// </summary>
        /// <value>The selection changed command.</value>
        public ICommand OnSelectionChanged { get; protected set; }

        /// <summary>
        /// Gets the block name.
        /// </summary>
        /// <value>The name of the block.</value>
        public string Name => Model.Name;

        /// <summary>
        /// Gets or sets the error in this block, if one exists.
        /// </summary>
        /// <value>The error related to this block.</value>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is an error.
        /// </summary>
        /// <value><c>true</c> if there is an error, <c>false</c> otherwise.</value>
        public bool IsError => Error != null;

        /// <summary>
        /// Gets a value indicating whether this block is loading.
        /// </summary>
        /// <value><c>true</c> if loading, <c>false</c> otherwise.</value>
        public bool IsLoading
        {
            get => isLoading;
            internal set => SetField(ref isLoading, value);
        }

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
        public string EmptyMessageTitle => null;

        /// <inheritdoc />
        public string EmptyImageName => IsError ? "error" : "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data";

        /// <summary>
        /// Subclasses should implement this in order to get updated block contents from the server.
        /// </summary>
        public abstract void Refresh();
    }
}
