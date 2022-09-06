using System;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;

namespace PERLS.Data.Factories
{
    /// <summary>
    /// Item to view model factory.
    /// </summary>
    public static class ItemToViewModelFactory
    {
        /// <summary>
        /// Creates the view model from item.
        /// </summary>
        /// <returns>The view model from item.</returns>
        /// <param name="model">Model.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public static CardViewModel CreateViewModelFromItem(IItem model, IAsyncCommand<IItem> downloadContentCommand)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return model switch
            {
                IQuiz quiz => new QuizViewModel(quiz),
                ITip tip => new TipViewModel(tip),
                ICourse course => new CourseViewModel(course, downloadContentCommand),
                IFlashcard flashcard => new FlashcardViewModel(flashcard),
                ITest test => new TestViewModel(test),
                IPodcast podcast => new PodcastViewModel(podcast, downloadContentCommand),
                IEpisode episode => new EpisodeViewModel(episode, downloadContentCommand),
                _ => new TeaserViewModel(model, downloadContentCommand),
            };
        }
    }
}
