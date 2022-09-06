using System;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Exceptions thrown by <see cref="SetServerViewModel"/>.
    /// </summary>
    public class SetServerViewModelException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetServerViewModelException"/> class.
        /// </summary>
        public SetServerViewModelException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetServerViewModelException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the error.</param>
        public SetServerViewModelException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetServerViewModelException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public SetServerViewModelException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
