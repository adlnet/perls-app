using System;
using System.Threading.Tasks;
using Float.Core.ViewModels;
using Float.FileDownloader;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Certificate Sharing ViewModel.
    /// </summary>
    public class CertificateSharingViewModel : ViewModel<ICertificate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateSharingViewModel"/> class.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        public CertificateSharingViewModel(ICertificate certificate) : base(certificate)
        {
            Model = certificate ?? throw new ArgumentNullException(nameof(certificate));
            CertificateFile = new EmbeddedFile(certificate.ShareableImageUri);
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public new ICertificate Model { get; protected set; }

        /// <summary>
        /// Gets the certified item.
        /// </summary>
        /// <value>
        /// The certified item.
        /// </value>
        public string CertifiedItem => Model.CertificateName;

        /// <summary>
        /// Gets the certificate file.
        /// </summary>
        /// <value>
        /// The certificate file.
        /// </value>
        public EmbeddedFile CertificateFile { get; }

        /// <summary>
        /// Gets a value indicating whether this file is downloaded.
        /// </summary>
        /// <value>
        /// A value indicating whether this file is downloaded.
        /// </value>
        public bool IsDownloaded => CertificateFile.IsDownloaded;

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DownloadFile()
        {
            if (IsDownloaded)
            {
                return;
            }

            var downloadStatus = new DownloadStatus(CertificateFile.Name);
            var service = DependencyService.Get<IDownloaderService>();
            await service.DownloadFileToPath(CertificateFile, downloadStatus).ConfigureAwait(false);
        }
    }
}
