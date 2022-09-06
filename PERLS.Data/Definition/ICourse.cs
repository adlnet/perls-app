using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A sequential list of learning objects.
    /// </summary>
    public interface ICourse : IItem
    {
        /// <summary>
        /// Gets sequential list of learning objects in this course.
        /// </summary>
        /// <value>The learning objects.</value>
        IEnumerable<IItem> LearningObjects { get; }
    }
}
