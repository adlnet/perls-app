using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using Float.Core.Commands;
using Float.Core.Definitions;
using PERLS.Data.Commands;
using PERLS.Data.Converters;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Infrastructure;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A base view model representing a card.
    /// </summary>
    public abstract class CardViewModel : Float.Core.ViewModels.SelectableViewModel<IItem>, ISelectable, INamed
    {
        DateTime? cardAppeared;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardViewModel"/> class.
        /// </summary>
        /// <param name="model">The card.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="reportingService">The reporting service to use for reporting activity.</param>
        protected CardViewModel(IItem model, IAsyncCommand<IItem> downloadContentCommand = null, IReportingService reportingService = null) : base(model)
        {
            BookmarkState = new BookmarkStateViewModel(model);
            CompletionState = new CompletionStateViewModel(model);
            MoreInfoStateViewModel = new MoreInfoStateViewModel(model);
            DownloadState = new DownloadStateViewModel(model, downloadContentCommand);
            CardAppearedCommand = new DebounceCommand(OnCardAppeared, 1000);
            CardDisappearedCommand = new DebounceCommand(OnCardDisappeared, 1000);
            ReportingService = reportingService ?? DependencyService.Get<IReportingService>();
        }

        /// <summary>
        /// Gets the name of the item being represented.
        /// </summary>
        /// <value>The item name.</value>
        public string Name => Model.Name;

        /// <summary>
        /// Gets a subtitle for the item.
        /// </summary>
        /// <remarks>
        /// This is displayed with the item when it appears in a list to help give more context.
        /// </remarks>
        /// <value>The item subtitle.</value>
        public string Subtitle
        {
            get
            {
                switch (Model.Type)
                {
                    case NodeTypeConstants.Course:
                        if (Model is ICourse course)
                        {
                            var count = course.LearningObjects.Count();
                            return count == 1 ? $"{count} {Strings.Lesson}" : $"{count} {Strings.Lessons}";
                        }

                        return string.Empty;
                    case NodeTypeConstants.FlashCard:
                        return Strings.TypeFlashCard;
                    case NodeTypeConstants.TipCard:
                        return Strings.TypeTip;
                    case NodeTypeConstants.QuizItem:
                        return Strings.TypeQuiz;
                    case NodeTypeConstants.TestItem:
                        return Strings.TypeTest;
                    case NodeTypeConstants.PodcastItem:
                        return Strings.TypePodcast;
                    case NodeTypeConstants.EpisodeItem:
                        return Strings.TypeEpisode;
                    default:
                        return Model.Topic?.Name ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the subtitle for search.
        /// </summary>
        /// <value>
        /// The subtitle for search.
        /// </value>
        public string SearchSubtitle => Model.Type switch
        {
            NodeTypeConstants.Course => Strings.TypeCourse,
            NodeTypeConstants.FlashCard => Strings.TypeFlashCard,
            NodeTypeConstants.TipCard => Strings.TypeTip,
            NodeTypeConstants.QuizItem => Strings.TypeQuiz,
            NodeTypeConstants.TestItem => Strings.TypeTest,
            NodeTypeConstants.PodcastItem => Strings.TypePodcast,
            NodeTypeConstants.EpisodeItem => Strings.TypeEpisode,
            NodeTypeConstants.LearnArticle => Strings.TypeArticle,
            _ => Model.Topic?.Name ?? string.Empty,
        };

        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        /// <value>The link.</value>
        public string Type => Model.Type;

        /// <summary>
        /// Gets a list of tags associated with the item.
        /// </summary>
        /// <value>The tags.</value>
        public string Tags => Model?.Tags?.Any() == true ? Model?.Tags?.Select(tag => $"#{tag.Name}").Aggregate((a, b) => $"{a} {b}") : string.Empty;

        /// <summary>
        /// Gets the item tags.
        /// </summary>
        /// <value>The item tags.</value>
        [NotifyWhenPropertyChanges(nameof(IItem.Tags))]
        public IEnumerable<TagViewModel> ItemTags => Model?.Tags?.Select(tag => new TagViewModel(tag)) ?? Enumerable.Empty<TagViewModel>();

        /// <summary>
        /// Gets the current bookmark state.
        /// </summary>
        /// <value>The bookmark state.</value>
        public BookmarkStateViewModel BookmarkState { get; }

        /// <summary>
        /// Gets the state of the completion.
        /// </summary>
        /// <value>The state of the completion.</value>
        public CompletionStateViewModel CompletionState { get; }

        /// <summary>
        /// Gets the state of the recommendation.
        /// </summary>
        /// <value>The state of the recommendation.</value>
        public MoreInfoStateViewModel MoreInfoStateViewModel { get; }

        /// <summary>
        /// Gets the state of the download.
        /// </summary>
        /// <value>The state of the download.</value>
        public DownloadStateViewModel DownloadState { get; }

        /// <summary>
        /// Gets a command to tell the card that it has appeared to the learner.
        /// </summary>
        /// <value>The card appearance command.</value>
        public ICommand CardAppearedCommand { get; }

        /// <summary>
        /// Gets a command to tell the card that it has disappeared from the learner.
        /// </summary>
        /// <value>The card disappearance command.</value>
        public ICommand CardDisappearedCommand { get; }

        /// <summary>
        /// Gets the alternative color for the card.
        /// </summary>
        /// <value>The alternative color.</value>
        public Color AltColor
        {
            get
            {
                var color = Model switch
                {
                    IQuiz _ => (Color)Application.Current.Resources["QuizColor"],
                    ITip _ => (Color)Application.Current.Resources["TipColor"],
                    IFlashcard _ => (Color)Application.Current.Resources["FlashCardColor"],
                    IPodcast _ => (Color)Application.Current.Resources["PodcastCardColor"],
                    _ => (Color)Application.Current.Resources["TransparentBlackColor"],
                };

                return (Color)new AltColorConverter().Convert(color, null, null, null);
            }
        }

        /// <summary>
        /// Gets the image Url.
        /// </summary>
        /// <value>The image source.</value>
        public ImageSource Image
        {
            get
            {
                if (Model.Image?.Url != null)
                {
                    return ImageSource.FromUri(Model?.Image?.Url);
                }

                return Model switch
                {
                    IQuiz _ or ITip _ or IFlashcard _ => null,
                    _ => ImageSource.FromFile("placeholder"),
                };
            }
        }

        /// <summary>
        /// Gets the image Url for the search page.
        /// </summary>
        /// <value>The image Url for the search page.</value>
        public ImageSource SearchImage
        {
            get
            {
                if (Model.Image?.Url != null)
                {
                    return ImageSource.FromUri(Model?.Image?.Url);
                }

                return Model switch
                {
                    IQuiz _ => SvgImageSource.FromResource("PERLS.Data.Resources.quiz.svg"),
                    ITip _ => SvgImageSource.FromResource("PERLS.Data.Resources.tip.svg"),
                    IFlashcard _ => SvgImageSource.FromResource("PERLS.Data.Resources.flashcard.svg"),
                    _ => ImageSource.FromFile("placeholder"),
                };
            }
        }

        /// <summary>
        /// Gets the search image corner radius.
        /// </summary>
        /// <value>
        /// The search image corner radius.
        /// </value>
        public double SearchImageCornerRadius
        {
            get
            {
                if (Model.Image?.Url != null)
                {
                    return 10.0;
                }

                return Model switch
                {
                    IQuiz _ or ITip _ or IFlashcard _ => 0.0,
                    _ => 10.0,
                };
            }
        }

        /// <summary>
        /// Gets the reporting service for reporting learner behavior.
        /// </summary>
        /// <value>The reporting service.</value>
        protected IReportingService ReportingService { get; }

        /// <summary>
        /// Gets a value indicating whether the card is currently visible.
        /// </summary>
        /// <value><c>true</c> if the card is visible.</value>
        /// <remarks>
        /// This simply keeps track of whether OnCardAppeared/OnCardDisappeared
        /// has been called. If the UI didn't invoke those methods OR a child
        /// class has overridden the methods without calling the base implementation,
        /// this property won't return the desired value.
        /// </remarks>
        protected bool IsVisible => cardAppeared != null;

        /// <summary>
        /// Gets the duration the card has been visible.
        /// </summary>
        /// <value>The duration.</value>
        protected TimeSpan Duration => (DateTime.Now - cardAppeared) ?? TimeSpan.Zero;

        /// <summary>
        /// Invoked when the card has appeared on screen.
        /// </summary>
        protected virtual void OnCardAppeared()
        {
            cardAppeared = DateTime.Now;
        }

        /// <summary>
        /// Invoked when the card is no longer visible on screen.
        /// </summary>
        protected virtual void OnCardDisappeared()
        {
            cardAppeared = null;
        }
    }
}
