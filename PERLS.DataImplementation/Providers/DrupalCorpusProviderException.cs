using System;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// An exception from <see cref="DrupalCorpusProvider"/>.
    /// </summary>
    public class DrupalCorpusProviderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrupalCorpusProviderException"/> class.
        /// </summary>
        public DrupalCorpusProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrupalCorpusProviderException"/> class.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        public DrupalCorpusProviderException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrupalCorpusProviderException"/> class.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="innerException">An inner exception within this exception.</param>
        public DrupalCorpusProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
