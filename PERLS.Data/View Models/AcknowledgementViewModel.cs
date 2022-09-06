using System;
using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Acknowledgement view model.
    /// </summary>
    public class AcknowledgementViewModel : ViewModel<Acknowledgement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgementViewModel"/> class.
        /// </summary>
        /// <param name="acknowledgement">The Acknowledgement.</param>
        public AcknowledgementViewModel(Acknowledgement acknowledgement) : base(acknowledgement)
        {
        }

        /// <summary>
        /// Gets the product.
        /// </summary>
        /// <value>The product.</value>
        public string Product => Model.Product;

        /// <summary>
        /// Gets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        public string Copyright => Model.Copyright;

        /// <summary>
        /// Gets the license text.
        /// </summary>
        /// <value>The license text.</value>
        public string LicenseText => Model.LicenseText;

        /// <summary>
        /// Gets the name of the license file.
        /// </summary>
        /// <value>The name of the license file.</value>
        public string LicenseFileName => Model.LicenseFileName;
    }
}
