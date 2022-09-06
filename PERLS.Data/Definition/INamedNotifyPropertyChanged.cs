using System.ComponentModel;
using Float.Core.Definitions;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// An interface that conforms to both <see cref="INamed"/> and <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public interface INamedNotifyPropertyChanged : INamed, INotifyPropertyChanged
    {
    }
}
