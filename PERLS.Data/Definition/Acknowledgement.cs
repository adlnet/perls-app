using System;
using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// An Acknowledgement.
    /// </summary>
    public class Acknowledgement : INotifyPropertyChanged
    {
        /// <summary>
        /// An EventHandler for when a property of this model changes.
        /// Note: This is never called. This is a requirement for this to work with a ViewModel.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>The product.</value>
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the license text.
        /// </summary>
        /// <value>The license text.</value>
        public string LicenseText { get; set; }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        public string Copyright { get; set; }

        /// <summary>
        /// Gets or sets the name of the license file.
        /// </summary>
        /// <value>The name of the license file.</value>
        public string LicenseFileName { get; set; }
    }
}
