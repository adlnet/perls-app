using System;
using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Extensions on the entity type enumeration.
    /// </summary>
    public static class EntityTypeExtensions
    {
        /// <summary>
        /// Returns the type appropriate for the given entity type; IItem, ITaxonomyTerm, or IGroup.
        /// </summary>
        /// <param name="entityType">The entity type for which to get the type.</param>
        /// <returns>The type appropriate for the given entity type.</returns>
        public static Type GetInterfaceType(this EntityType entityType)
        {
            return entityType switch
            {
                EntityType.Node => typeof(IItem),
                EntityType.TaxonomyTerm => typeof(ITaxonomyTerm),
                EntityType.Group => typeof(IGroup),
                _ => throw new InvalidEnumArgumentException(),
            };
        }
    }
}
