using System;
using System.Windows.Input;
using PERLS.Data.Commands;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Navigation command provider.
    /// </summary>
    public interface INavigationCommandProvider
    {
        /// <summary>
        /// Gets or sets the navigation command.
        /// </summary>
        /// <value>The navigation command.</value>
        ICommand ItemSelected { get; set; }

        /// <summary>
        /// Gets or sets the setting selected.
        /// </summary>
        /// <value>The setting selected.</value>
        ICommand SettingSelected { get; set; }

        /// <summary>
        /// Gets or sets a command to execute when the user requests a download.
        /// </summary>
        /// <value>The download requested commmand.</value>
        IAsyncCommand<IItem> DownloadRequested { get; set; }

        /// <summary>
        /// Gets or sets a command to execute when the user requests the next article in a course.
        /// </summary>
        /// <value>The next article command.</value>
        ICommand<Uri> NextArticleSelected { get; set; }

        /// <summary>
        /// Gets or sets a command to execute when the user requests to navigate to the custom goals.
        /// </summary>
        /// <value>
        /// A command to execute when the user requests to navigate to the custom goals.
        /// </value>
        ICommand GotoCustomGoalsSelected { get; set; }
    }
}
