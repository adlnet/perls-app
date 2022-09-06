using System.Collections;
using System.Collections.Generic;
using Float.Core.Definitions;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model that has a variable number of items in it.
    /// </summary>
    public interface IVariableItemViewModel : IEmptyCollectionViewModel
    {
        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        IEnumerable Elements { get; }
    }

    /// <summary>
    /// A view model that has a variable number of items in it.
    /// </summary>
    /// <typeparam name="T">The type of the collection of view model.</typeparam>
    public interface IVariableItemViewModel<T> : IEmptyCollectionViewModel
    {
        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        IEnumerable<T> Elements { get; }
    }

    /// <summary>
    /// A named view model that has a variable number of elements in it.
    /// </summary>
    /// <typeparam name="T">The type of the collection of view model.</typeparam>
    public interface INamedVariableItemViewModel<T> : IVariableItemViewModel<T>, INamed
    {
    }
}
