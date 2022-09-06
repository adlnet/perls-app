using System.ComponentModel;
using Float.Core.Definitions;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A topic (category).
    /// </summary>
    public interface ITopic : INamed, IRemoteResource, INotifyPropertyChanged, ITaxonomyTerm
    {
    }
}
