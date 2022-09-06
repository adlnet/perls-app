using System;
using System.Globalization;
using System.Windows.Input;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The certificate view model.
    /// </summary>
    public class CertificateViewModel : BasePageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="shareCertificateCommand">The share certificate command.</param>
        public CertificateViewModel(ICertificate model, ICommand shareCertificateCommand)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            ShareCertificateCommand = shareCertificateCommand ?? throw new ArgumentNullException(nameof(shareCertificateCommand));
            DownloadAndShareCertificateCommand = new Command(DownloadAndShareCertificate);
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public ICertificate Model { get; protected set; }

        /// <summary>
        /// Gets or sets the share certificate command.
        /// </summary>
        /// <value>
        /// The share certificate command.
        /// </value>
        public ICommand ShareCertificateCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the download and share certificate command.
        /// </summary>
        /// <value>
        /// The download and share certificate command.
        /// </value>
        public ICommand DownloadAndShareCertificateCommand { get; protected set; }

        /// <summary>
        /// Gets the certified item.
        /// </summary>
        /// <value>
        /// The certified item.
        /// </value>
        public string CertifiedItem => Model.CertificateName;

        /// <summary>
        /// Gets the earned date.
        /// </summary>
        /// <value>
        /// The earned date.
        /// </value>
        public string EarnedDate => Model.ReceivedTime.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);

        /// <summary>
        /// Gets the earned date.
        /// </summary>
        /// <value>
        /// The earned date.
        /// </value>
        public string EarnedMessage => $"{Strings.EarnedMessage} {EarnedDate}";

        /// <summary>
        /// Gets the certificate image for display on the device.
        /// </summary>
        /// <value>
        /// The certificate image for display on the device.
        /// </value>
        public string CertificateImage => Model.ThumbnailImageUri?.OriginalString;

        /// <summary>
        /// Gets the certificate sharing view model.
        /// </summary>
        /// <value>
        /// The certificate sharing view model.
        /// </value>
        public CertificateSharingViewModel SharingViewModel => new CertificateSharingViewModel(Model);

        void DownloadAndShareCertificate()
        {
            if (SharingViewModel.IsDownloaded)
            {
                ShareCertificateCommand.Execute(SharingViewModel);
                return;
            }

            IsLoading = true;

            SharingViewModel.DownloadFile().OnSuccess((task) =>
            {
                Error = null;
                IsLoading = false;
                ShareCertificateCommand.Execute(SharingViewModel);
            }).OnFailure((task) =>
            {
                Error = task.Exception;
                IsLoading = false;
            });
        }
    }
}
