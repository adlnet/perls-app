using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Interface for navigation from a notification.
    /// </summary>
    public interface INotificationNavigation
    {
        /// <summary>
        /// Gets the navigation command.
        /// </summary>
        /// <value>The navgiation command.</value>
        ICommand ItemSelected { get; }

        /// <summary>
        /// Navigates to the page specified by the action.
        /// </summary>
        /// <param name="action">The action of the notification.</param>
        /// <param name="item">The dictionary holding data from the notification.</param>
        /// <param name="relatedItems">The dictionary holding related data from the notification.</param>
        void NavigateByAction(string action, Dictionary<string, string> item = null, Dictionary<string, string> relatedItems = null);
    }
}
