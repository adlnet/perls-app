using System;
using PERLS.Data.ViewModels.Sections;
using Xamarin.Forms;

namespace PERLS.Templates
{
    /// <summary>
    /// A data template selector for a collection view composed of sections.
    /// </summary>
    public class SectionDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for a tabbed section.
        /// </summary>
        /// <value>The tabbed section template.</value>
        public DataTemplate TabbedSectionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a one column section.
        /// </summary>
        /// <value>The one column section template.</value>
        public DataTemplate OneColumnSectionTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => item switch
        {
            TabbedSectionViewModel _ => TabbedSectionTemplate,
            OneColumnSectionViewModel _ => OneColumnSectionTemplate,
            _ => throw new ArgumentException(),
        };
    }
}
