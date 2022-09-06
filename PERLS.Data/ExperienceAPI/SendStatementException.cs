using System;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// An exception that may occur when sending a statement fails.
    /// </summary>
    public class SendStatementException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendStatementException"/> class.
        /// </summary>
        public SendStatementException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendStatementException"/> class.
        /// </summary>
        /// <param name="message">The message for this exception.</param>
        public SendStatementException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendStatementException"/> class.
        /// </summary>
        /// <param name="message">The message for this exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public SendStatementException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
