using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A very simple view model to represent a remote resource with a textual representation.
    /// </summary>
    public class ChipViewModel : ViewModel<IRemoteResource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to represent.</param>
        public ChipViewModel(IRemoteResource model) : base(model)
        {
        }

        /// <summary>
        /// Gets the name of the remote resource.
        /// </summary>
        /// <value>The name of the backing model.</value>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Model.Name))
                {
                    return string.Empty;
                }

                var trimmedName = Model.Name?.Length > 30 ? $"{Model.Name.Substring(0, 26)}..." : Model.Name;

                if (Model is ITag)
                {
                    return $"#{trimmedName}";
                }

                return trimmedName;
            }
        }

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        /// <remarks>TODO: This should be internal.</remarks>
        public IRemoteResource ModelItem => Model;
    }
}
