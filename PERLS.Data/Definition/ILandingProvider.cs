using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The Landing provider interface.
    /// </summary>
    public interface ILandingProvider
    {
        /// <summary>
        /// Gets the landing data.
        /// </summary>
        /// <returns>The landing data.</returns>
        IEnumerable<ILandingData> GetLandingData();
    }
}
