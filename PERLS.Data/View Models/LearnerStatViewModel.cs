using System;
using Float.Core.ViewModels;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The item type that the stat represents.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// A learning object.
        /// </summary>
        LearningObject,

        /// <summary>
        /// A course.
        /// </summary>
        Course,
    }

    /// <summary>
    /// The Learner State view model.
    /// </summary>
    public class LearnerStatViewModel : BaseViewModel
    {
        readonly int amount;
        readonly string verb;
        readonly ItemType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerStatViewModel"/> class.
        /// </summary>
        /// <param name="amount">The amount for the stat.</param>
        /// <param name="verb">The verb for the stat.</param>
        /// <param name="type">The type.</param>
        public LearnerStatViewModel(int amount, string verb, ItemType type = ItemType.LearningObject)
        {
            if (string.IsNullOrWhiteSpace(verb))
            {
                throw new ArgumentNullException(nameof(verb));
            }

            this.amount = amount;
            this.verb = verb;
            this.type = type;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description => $"{TypeString} {verb}";

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value => $"{amount}";

        /// <summary>
        /// Gets the type (i.e. course or item) associated with this stat.
        /// </summary>
        /// <value>
        /// The type of data measured by the stat.
        /// </value>
        public string TypeString
        {
            get
            {
                return type switch
                {
                    ItemType.LearningObject => amount == 1 ? Strings.ItemSingular : Strings.ItemPlural,
                    ItemType.Course => amount == 1 ? Strings.TypeCourse : Strings.CoursePlural,
                    _ => string.Empty,
                };
            }
        }
    }
}
