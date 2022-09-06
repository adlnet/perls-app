using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// Template selector to allow a deck of cards choose which card template to use.
    /// </summary>
    public class CardDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the data template for quiz items.
        /// </summary>
        /// <value>The quiz template.</value>
        public DataTemplate QuizTemplate { get; set; }

        /// <summary>
        /// Gets or sets the data template for tip items.
        /// </summary>
        /// <value>The tip template.</value>
        public DataTemplate TipTemplate { get; set; }

        /// <summary>
        /// Gets or sets the data template for course items.
        /// </summary>
        /// <value>The course template.</value>
        public DataTemplate CourseTemplate { get; set; }

        /// <summary>
        /// Gets or sets the flashcard template.
        /// </summary>
        /// <value>The flashcard template.</value>
        public DataTemplate FlashcardTemplate { get; set; }

        /// <summary>
        /// Gets or sets the test template.
        /// </summary>
        /// <value>The test template.</value>
        public DataTemplate TestTemplate { get; set; }

        /// <summary>
        /// Gets or sets the test results template.
        /// </summary>
        /// <value>The test results template.</value>
        public DataTemplate TestResultsTemplate { get; set; }

        /// <summary>
        /// Gets or sets the prompt data template.
        /// </summary>
        /// <value>The prompt template.</value>
        public DataTemplate PromptTemplate { get; set; }

        /// <summary>
        /// Gets or sets the prompt result template.
        /// </summary>
        /// <value>The prompt template.</value>
        public DataTemplate PromptResultTemplate { get; set; }

        /// <summary>
        /// Gets or sets the podcast data template.
        /// </summary>
        /// <value>The podcast data template.</value>
        public DataTemplate PodcastTemplate { get; set; }

        /// <summary>
        /// Gets or sets the default data template.
        /// </summary>
        /// <value>The default data template.</value>
        public DataTemplate DefaultTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                QuizViewModel _ => QuizTemplate,
                TipViewModel _ => TipTemplate,
                CourseViewModel _ => CourseTemplate,
                FlashcardViewModel _ => FlashcardTemplate,
                TestViewModel _ => TestTemplate,
                TestResultsViewModel _ => TestResultsTemplate,
                PromptCollectionViewModel _ => PromptTemplate,
                PromptViewModel _ => PromptTemplate,
                PromptResultViewModel _ => PromptResultTemplate,
                PodcastViewModel _ => PodcastTemplate,
                _ => DefaultTemplate,
            };
        }
    }
}
