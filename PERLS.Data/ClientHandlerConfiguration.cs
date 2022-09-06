using System;

namespace PERLS.Data
{
    /// <summary>
    /// Configurations to be set in the Http Client Handler.
    /// </summary>
    public class ClientHandlerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHandlerConfiguration"/> class.
        /// </summary>
        public ClientHandlerConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether allowing of cookies.
        /// </summary>
        /// <value>Boolean to allow cookies.</value>
        public bool AllowCookies { get; set; }

        /// <summary>
        /// Gets or sets an integer indicating the request timeout.
        /// </summary>
        /// <value>The timeout interval to use when waiting for additional data.</value>
        public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// Gets or sets an integer indicating the connection timeout.
        /// Android Only.
        /// </summary>
        /// <value>The timeout interval to use when waiting for a connection.</value>
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(120);

        /// <summary>
        /// Gets or sets an integer indicating the timeout to download entire resource.
        /// Apple Only.
        /// </summary>
        /// <value>The timeout interval to use when waiting for entire resource.</value>
        public TimeSpan ResourceTimeout { get; set; } = TimeSpan.FromHours(24);
    }
}
