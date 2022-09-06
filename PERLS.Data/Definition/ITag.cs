using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A tag.
    /// </summary>
    public interface ITag : IRemoteResource, INotifyPropertyChanged, ITaxonomyTerm
    {
        /// <summary>
        /// Gets the vid.
        /// </summary>
        /// <value>The vid.</value>
        int Vid { get; }
    }
}
