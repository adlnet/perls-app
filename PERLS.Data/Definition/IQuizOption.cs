namespace PERLS.Data.Definition
{
    /// <summary>
    /// An answer option on a quiz item.
    /// </summary>
    public interface IQuizOption
    {
        /// <summary>
        /// Gets the text of the option.
        /// </summary>
        /// <value>The text of the option.</value>
        string Text { get; }

        /// <summary>
        /// Gets a value indicating whether the option is correct.
        /// </summary>
        /// <value><c>true</c> if this option is correct, <c>false</c> otherwise.</value>
        bool IsCorrect { get; }

        /// <summary>
        /// Gets the feedback associated with the option.
        /// </summary>
        /// <value>The associated feedback.</value>
        string Feedback { get; }
    }
}
