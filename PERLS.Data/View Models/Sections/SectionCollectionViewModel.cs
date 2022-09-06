using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels.Sections
{
    /// <summary>
    /// A view model for a collection of sections.
    /// </summary>
    public class SectionCollectionViewModel : RefreshableClearlessBaseCollectionViewModel<ISection, SectionViewModel>, IEmptyCollectionViewModel
    {
        readonly ICommand selectItemCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionCollectionViewModel"/> class.
        /// </summary>
        /// <param name="modelCollectionTask">The task to retrieve data for this view model. Invoked on init.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public SectionCollectionViewModel(Func<Task<IEnumerable<ISection>>> modelCollectionTask, ICommand selectItemCommand) : base(modelCollectionTask)
        {
            this.selectItemCommand = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
        }

        /// <summary>
        /// Gets a value indicating whether this collection is loading.
        /// </summary>
        /// <value><c>true</c> if this collection is loading, <c>false</c> otherwise.</value>
        public bool IsLoading => Elements.Where(section => section.IsLoading).Any();

        /// <summary>
        /// Gets a value indicating whether this collection is ready (i.e. not loading).
        /// </summary>
        /// <value><c>true</c> if this collection is ready, <c>false</c> otherwise.</value>
        public bool IsReady => !IsLoading;

        /// <summary>
        /// Gets a value indicating whether any sections are in an error state.
        /// </summary>
        /// <value><c>true</c> if there is an error state, <c>false</c> otherwise.</value>
        public bool IsError => Elements.Any(vm => vm.IsError);

        /// <inheritdoc />
        public string EmptyMessageTitle => Strings.EmptyEnhancedDashboardTitle;

        /// <inheritdoc />
        public string EmptyLabel
        {
            get
            {
                var sectionErrors = Elements.Select(vm => vm.Error).OfType<Exception>();

                foreach (var error in sectionErrors)
                {
                    if (error is AggregateException aggregate)
                    {
                        return error.InnerException?.Message;
                    }
                    else
                    {
                        return error.Message;
                    }
                }

                return Strings.UnknownErrorMessage;
            }
        }

        /// <inheritdoc />
        public string EmptyImageName => IsError ? "error" : "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data";

        /// <inheritdoc />
        public override async Task Refresh()
        {
            await base.Refresh().ConfigureAwait(false);
            await Task.WhenAll(Elements.Select(section => Task.Run(section.Refresh))).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override SectionViewModel ConvertModelToViewModel(ISection model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return model.Layout switch
            {
                SectionLayout.OneColumn => new OneColumnSectionViewModel(model, selectItemCommand),
                SectionLayout.Tabs => new TabbedSectionViewModel(model, selectItemCommand),
                _ => throw new InvalidEnumArgumentException(),
            };
        }
    }
}
