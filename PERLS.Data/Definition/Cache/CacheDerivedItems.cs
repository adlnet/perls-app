using System.Collections.Generic;

namespace PERLS.Data.Definition.Cache
{
    /// <summary>
    /// A list of items that was derived from the local cache.
    /// The user of UI may wish to explicitly indicate that
    /// the list of result being shown did not come from the
    /// ultimate source of truth: the server.
    /// </summary>
    public class CacheDerivedItems : List<IItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDerivedItems"/> class.
        /// </summary>
        /// <param name="collection">A collection of items.</param>
        public CacheDerivedItems(IEnumerable<IItem> collection) : base(collection)
        {
        }
    }
}
