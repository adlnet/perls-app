using System;
using System.Collections.Generic;
using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Acknowledgement list view model.
    /// </summary>
    public class AcknowledgementListViewModel : BaseCollectionViewModel<Acknowledgement, AcknowledgementViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgementListViewModel"/> class.
        /// </summary>
        /// <param name="acknowledgements">The Acknowledgements list.</param>
        public AcknowledgementListViewModel(IEnumerable<Acknowledgement> acknowledgements) : base(acknowledgements)
        {
        }
    }
}
