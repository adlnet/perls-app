using System;
using System.Windows.Input;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Providers
{
    /// <summary>
    /// Navigation command provider.
    /// </summary>
    public class NavigationCommandProvider : INavigationCommandProvider
    {
        /// <inheritdoc />
        public ICommand ItemSelected { get; set; }

        /// <inheritdoc />
        public ICommand SettingSelected { get; set; }

        /// <inheritdoc />
        public IAsyncCommand<IItem> DownloadRequested { get; set; }

        /// <inheritdoc />
        public ICommand<Uri> NextArticleSelected { get; set; }

        /// <inheritdoc/>
        public ICommand GotoCustomGoalsSelected { get; set; }
    }
}
