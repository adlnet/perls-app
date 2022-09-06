using System.Windows.Input;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Tag view model.
    /// </summary>
    public class TagViewModel : Float.Core.ViewModels.SelectableViewModel<ITag>, ISelectable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagViewModel"/> class.
        /// </summary>
        /// <param name="tag">Tag.</param>
        public TagViewModel(ITag tag) : base(tag)
        {
            TagSelected = DependencyService.Get<INavigationCommandProvider>().ItemSelected;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => Model.Name;

        /// <summary>
        /// Gets the tag display.
        /// </summary>
        /// <value>The tag display.</value>
        public string TagDisplay => $"#{Model.Name}";

        /// <summary>
        /// Gets the tag selected.
        /// </summary>
        /// <value>The tag selected.</value>
        public ICommand TagSelected { get; }

        /// <summary>
        /// Gets the term ID for the tag.
        /// </summary>
        /// <value>The term id.</value>
        public int Tid => Model.Tid;

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        /// <remarks>TODO: This should be internal.</remarks>
        public ITag ModelItem => Model;
    }
}
