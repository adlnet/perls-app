using System;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// Represents an object that is an xAPI activity.
    /// </summary>
    public interface IExperienceAPIActivity
    {
        /// <summary>
        /// Gets the xAPI activity ID.
        /// </summary>
        /// <value>The xAPI activity ID.</value>
        Uri Id { get; }
    }
}
