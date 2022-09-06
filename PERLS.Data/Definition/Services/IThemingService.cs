using System.Threading.Tasks;
using Xamarin.Forms;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Allows for custom theming of the application.
    /// </summary>
    public interface IThemingService
    {
        /// <summary>
        /// Applies a theme to the specified application.
        /// </summary>
        /// <param name="application">The application to theme.</param>
        void ApplyTheme(Application application);

        /// <summary>
        /// Updates the Theme.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IAppearance> UpdateTheme();

        /// <summary>
        /// Resets the theme.
        /// </summary>
        /// <param name="application">The application to reset the theme on.</param>
        void ResetTheme(Application application);
    }
}
