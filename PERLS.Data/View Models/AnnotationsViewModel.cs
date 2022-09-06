using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Collections;
using Float.Core.ViewModels;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The annotations view model.
    /// </summary>
    public class AnnotationsViewModel : BaseCollectionViewModel<IAnnotation, AnnotationViewModel>
    {
        readonly IList<AnnotationViewModel> deletedAnnotations = new List<AnnotationViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsViewModel"/> class.
        /// </summary>
        /// <param name="modelCollectionTask">The task.</param>
        public AnnotationsViewModel(Task<IEnumerable<IAnnotation>> modelCollectionTask) : base(modelCollectionTask)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsViewModel"/> class.
        /// </summary>
        /// <param name="modelCollection">The collection.</param>
        /// <param name="filter">The filter.</param>
        public AnnotationsViewModel(IEnumerable<IAnnotation> modelCollection, IFilter<IAnnotation> filter = null) : base(modelCollection, filter)
        {
        }

        /// <summary>
        /// Gets the "current" annotations which are the annotations we've received from the server except for the ones that have been locally deleted.
        /// </summary>
        /// <value>
        /// The "current" annotations which are the annotations we've received from the server except for the ones that have been locally deleted.
        /// </value>
        public IEnumerable<AnnotationViewModel> CurrentAnnotations => AllElements.Except(deletedAnnotations).OrderByDescending((arg) => arg.DateCreated);

        /// <summary>
        /// Deletes an annotation.
        /// </summary>
        /// <param name="annotationViewModel">The view model of the annotation to be deleted.</param>
        public void DeleteAnnotation(AnnotationViewModel annotationViewModel)
        {
            var model = Models.FirstOrDefault(arg => GetViewModelForModel(arg) == annotationViewModel);

            if (model != null)
            {
                DependencyService.Get<ILearnerStateProvider>().DeleteAnnotation(model);
                deletedAnnotations.Add(annotationViewModel);
                OnPropertyChanged(nameof(CurrentAnnotations));
            }
        }
    }
}
