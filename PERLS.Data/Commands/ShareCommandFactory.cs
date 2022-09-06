using Float.Core.Commands;
using Float.Core.Extensions;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// Command factory to generate share resource commands.
    /// </summary>
    public class ShareCommandFactory : UserRequestCommandFactory<IShareableRemoteResource>
    {
    /// <summary>
    /// Initializes a new instance of the <see cref="ShareCommandFactory"/> class.
    /// </summary>
    /// <param name="notificationHandler">The app's notification handler.</param>
        public ShareCommandFactory(INotificationHandler notificationHandler) : base(notificationHandler)
        {
        }

        /// <inheritdoc />
        protected override string ExceptionImplication => Strings.ShareError;

        /// <inheritdoc />
        protected override void Execute(IShareableRemoteResource parameter)
        {
            Share.RequestAsync(parameter.GetShareRequest()).OnSuccess((task) =>
            {
                DependencyService.Get<IReportingService>().ReportItemShared(parameter);
            });
        }
    }
}
