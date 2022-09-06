using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Extensions;
using Float.Core.Net;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The note view model.
    /// </summary>
    public class LearnerNotesViewModel : BasePageViewModel, IEmptyCollectionViewModel
    {
        readonly ILearnerStateProvider learnerStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerNotesViewModel"/> class.
        /// </summary>
        /// <param name="learnerStateProvider">The provider.</param>
        /// <param name="gotoAnnotationCommand">The command for going to annotations.</param>
        public LearnerNotesViewModel(ILearnerStateProvider learnerStateProvider, ICommand gotoAnnotationCommand)
        {
            if (learnerStateProvider == null)
            {
                throw new ArgumentNullException(nameof(learnerStateProvider));
            }

            if (gotoAnnotationCommand == null)
            {
                throw new ArgumentNullException(nameof(gotoAnnotationCommand));
            }

            this.learnerStateProvider = learnerStateProvider;
            DeleteAnnotationSelectedCommand = new Command(HandleAskToDelete);
            AnnotationSelectedCommand = new Command(HandleAnnotationSelected);
            GoToAnnotationCommand = gotoAnnotationCommand;
            Title = Strings.TabMyContentLabel;
            Refresh();
        }

        /// <summary>
        /// Gets the go to annotation command.
        /// </summary>
        /// <value>
        /// The go to annotation command.
        /// </value>
        public ICommand GoToAnnotationCommand { get; }

        /// <summary>
        /// Gets the command for going to the annotation.
        /// </summary>
        /// <value>
        /// The command for going to the annotation.
        /// </value>
        public ICommand AnnotationSelectedCommand { get; }

        /// <summary>
        /// Gets the command for deleting an annotation.
        /// </summary>
        /// <value>
        /// The command for deleting an annotation.
        /// </value>
        public ICommand DeleteAnnotationSelectedCommand { get; }

        /// <summary>
        /// Gets or sets the annotations view model.
        /// </summary>
        /// <value>
        /// The annotations view model.
        /// </value>
        public AnnotationsViewModel Annotations { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether there was an error loading the annotations.
        /// </summary>
        /// <value>
        /// <c>true</c> if there was an error, <c>false</c> otherwise.
        /// </value>
        public bool IsError => Error != null;

        /// <summary>
        /// Gets the empty label.
        /// </summary>
        /// <value>
        /// The empty label.
        /// </value>
        public string EmptyLabel => Error == null ? Strings.EmptyNotesMessage : DependencyService.Get<INotificationHandler>().FormatException(Error);

        /// <summary>
        /// Gets the empty message title.
        /// </summary>
        /// <value>
        /// The empty message title.
        /// </value>
        public string EmptyMessageTitle => Error == null ? Strings.EmptyNotesTitle : Strings.EmptyViewErrorTitle;

        /// <summary>
        /// Gets the empty image name.
        /// </summary>
        /// <value>
        /// The empty image name.
        /// </value>
        public string EmptyImageName => Error == null ? "resource://PERLS.Data.Resources.empty_note.svg?Assembly=PERLS.Data" : "error";

        /// <inheritdoc/>
        public override void Refresh()
        {
            base.Refresh();

            GetAnnotations()
                .ContinueWith(
                task =>
                {
                    if (task.Exception is AggregateException aggregateException)
                    {
                        ContainsCachedData = task.Exception.InnerException.IsOfflineException();
                        Error = aggregateException;
                        return;
                    }

                    ContainsCachedData = task.Exception != null && task.Exception.IsOfflineException();
                    Error = task.Exception;
                }, TaskScheduler.Current);
        }

        async Task GetAnnotations()
        {
            if (IsError || Annotations == null)
            {
                IsLoading = true;
            }

            try
            {
                var annotations = await learnerStateProvider.GetAnnotations().ConfigureAwait(false);
                Annotations = new AnnotationsViewModel(annotations);
                OnPropertyChanged(nameof(Annotations));
                OnPropertyChanged(nameof(Annotations.CurrentAnnotations));

                if (!await DependencyService.Get<INetworkConnectionService>().IsReachable().ConfigureAwait(false))
                {
                    throw new HttpConnectionException(Strings.OfflineMessageBody.AddAppName());
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        void HandleAnnotationSelected(object arg)
        {
            if (arg is not AnnotationViewModel annotationViewModel)
            {
                return;
            }

            // Go to annotationViewModel
            GoToAnnotationCommand?.Execute(annotationViewModel);
        }

        async void HandleAskToDelete(object arg)
        {
            var result = await Application.Current.MainPage
                    .DisplayAlert(Strings.DeleteAnnotationTitle, Strings.DeleteAnnotationMessage, Strings.Okay, Strings.No)
                    .ConfigureAwait(false);

            if (result)
            {
                HandleDelete(arg);
            }
        }

        void HandleDelete(object arg)
        {
            if (arg is not AnnotationViewModel annotationViewModel)
            {
                return;
            }

            Annotations.DeleteAnnotation(annotationViewModel);
        }
    }
}
