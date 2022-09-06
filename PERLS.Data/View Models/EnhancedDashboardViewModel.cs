using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels.Sections;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A content-agnostic dashboard with presentation order dictated by the CMS.
    /// </summary>
    public class EnhancedDashboardViewModel : BasePageViewModel, IPageViewModel
    {
        readonly ICorpusProvider corpusProvider = DependencyService.Get<ICorpusProvider>();
        bool isRefreshing;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnhancedDashboardViewModel"/> class.
        /// </summary>
        /// <param name="selectItemCommand">The command to invoke when a content item is selected.</param>
        public EnhancedDashboardViewModel(ICommand selectItemCommand)
        {
            Sections = new SectionCollectionViewModel(corpusProvider.GetEnhancedDashboard, selectItemCommand);
        }

        /// <summary>
        /// Gets the section collection to show on the enhanced dashboard.
        /// </summary>
        /// <value>The sections view model.</value>
        public SectionCollectionViewModel Sections { get; }

        /// <summary>
        /// Refresh this instance.
        /// </summary>
        public override void Refresh()
        {
            if (isRefreshing)
            {
                return;
            }

            isRefreshing = true;
            base.Refresh();

            IsLoading = !Sections.Any();

            Sections.Refresh().ContinueWith(
                task =>
                {
                    Error = task.Exception;
                    IsLoading = false;
                    isRefreshing = false;
                }, TaskScheduler.Current);
        }
    }
}
