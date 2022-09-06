using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Extensions;
using Float.Core.Net;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Certificates View Model.
    /// </summary>
    public class CertificatesViewModel : BasePageViewModel, IEmptyCollectionViewModel
    {
        readonly ILearnerProvider learnerProvider;
        IEnumerable<ICertificate> certificates;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificatesViewModel"/> class.
        /// </summary>
        /// <param name="selectItemCommand">The select item command.</param>
        /// <param name="learnerProvider">The learner provider.</param>
        public CertificatesViewModel(ICommand selectItemCommand, ILearnerProvider learnerProvider = null)
        {
            SelectItemCommand = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
            Title = Strings.TabMyContentLabel;

            this.learnerProvider = learnerProvider ?? DependencyService.Get<ILearnerProvider>();
            Refresh();
        }

        /// <summary>
        /// Gets or sets the certificates.
        /// </summary>
        /// <value>
        /// The certificates.
        /// </value>
        public IEnumerable<CertificateViewModel> Certificates { get; protected set; }

        /// <summary>
        /// Gets the command to invoke when an item is selected.
        /// </summary>
        /// <value>The selected command.</value>
        public ICommand SelectItemCommand { get; }

        /// <summary>
        /// Gets the empty label.
        /// </summary>
        /// <value>
        /// The empty label.
        /// </value>
        public string EmptyLabel => Error == null ? Strings.EmptyCertificatesMessage : DependencyService.Get<INotificationHandler>().FormatException(Error);

        /// <summary>
        /// Gets the empty message title.
        /// </summary>
        /// <value>
        /// The empty message title.
        /// </value>
        public string EmptyMessageTitle => Error == null ? Strings.EmptyCertificatesTitle : Strings.EmptyViewErrorTitle;

        /// <summary>
        /// Gets the empty image name.
        /// </summary>
        /// <value>
        /// The empty image name.
        /// </value>
        public string EmptyImageName => Error == null ? "resource://PERLS.Data.Resources.empty_certificate.svg?Assembly=PERLS.Data" : "error";

        /// <summary>
        /// Gets a value indicating whether there was an error loading the certificates.
        /// </summary>
        /// <value>
        /// <c>true</c> if there was an error, <c>false</c> otherwise.
        /// </value>
        public bool IsError => Error != null;

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();
            if (IsLoading)
            {
                return;
            }

            GetCertificates().ContinueWith(
                task =>
                {
                    Error = task.Exception;
                    ContainsCachedData = certificates.Any() && task.Exception?.InnerException?.IsOfflineException() == true;
                }, TaskScheduler.Current);
        }

        async Task GetCertificates()
        {
            if (IsError || certificates == null)
            {
                IsLoading = true;
            }

            try
            {
                certificates = await learnerProvider.GetCertificates().ConfigureAwait(false);
                GenerateCertificates();

                // Ideally, it would be easier to know if the response we got from the provider was cached.
                if (!await DependencyService.Get<INetworkConnectionService>().IsReachable().ConfigureAwait(false))
                {
                    throw new HttpConnectionException(Strings.OfflineMessageBody.AddAppName());
                }

                Error = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        void GenerateCertificates()
        {
            if (certificates == null)
            {
                return;
            }

            Certificates = certificates.Select((arg) => new CertificateViewModel(arg, SelectItemCommand));
            OnPropertyChanged(nameof(Certificates));
        }
    }
}
