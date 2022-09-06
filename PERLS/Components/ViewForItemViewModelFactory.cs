using Float.Core.ViewModels;
using PERLS.Components.Cards;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// View for item view model factory.
    /// </summary>
    public static class ViewForItemViewModelFactory
    {
        /// <summary>
        /// Creates the view.
        /// </summary>
        /// <returns>The view.</returns>
        /// <param name="viewModel">View model.</param>
        public static View CreateView(ViewModel<IItem> viewModel) => viewModel switch
        {
            TipViewModel _ => new TipCard(),
            QuizViewModel _ => new QuizCard(),
            CourseViewModel _ => new CourseCard(),
            FlashcardViewModel _ => new FlashCardCard(),
            TestViewModel _ => new StackOfCardsCard(),
            TestResultsViewModel _ => new TestResultsCard(),
            _ => new ObjectCard(),
        };
    }
}
