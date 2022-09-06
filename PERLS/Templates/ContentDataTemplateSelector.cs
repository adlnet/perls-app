using System.Collections;
using Xamarin.Forms;

namespace PERLS.Templates
{
    /// <summary>
    /// Data template selector for a list of content.
    /// Can handle either a flat list or a list of lists.
    /// </summary>
    public class ContentDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the data template for a single dimensional list.
        /// </summary>
        /// <value>The one-dimensional list template.</value>
        public DataTemplate List { get; set; }

        /// <summary>
        /// Gets or sets the data template for a two dimensional list.
        /// </summary>
        /// <value>The two-dimensional list template.</value>
        public DataTemplate GroupedTiles { get; set; }

        /// <inheritdoc />
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is IEnumerable)
            {
                return GroupedTiles;
            }

            return List;
        }
    }
}
