using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Extensions;
using Float.Core.ViewModels;
using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The base view model for representing a page in the app.
    /// </summary>
    public abstract class BasePageViewModel : BaseViewModel, IPageViewModel
    {
        bool isLoading;
        bool containsCachedData;
        Exception error;
        string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePageViewModel"/> class.
        /// </summary>
        protected BasePageViewModel()
        {
            RefreshCommand = new DebounceCommand(Refresh);
        }

        /// <inheritdoc />
        public bool IsLoading
        {
            get => isLoading;
            set => SetField(ref isLoading, value);
        }

        /// <inheritdoc />
        public bool IsReady => !IsLoading;

        /// <inheritdoc />
        public Exception Error
        {
            get => error;
            set => SetField(ref error, value);
        }

        /// <inheritdoc />
        public string Title
        {
            get => title;
            set => SetField(ref title, value);
        }

        /// <summary>
        /// Gets and formats the offline message body.
        /// </summary>
        /// <value>The offline message body.</value>
        public string OfflineMessageBody => Strings.OfflineMessageBody.AddAppName();

        /// <summary>
        /// Gets or sets a value indicating whether the page contains cached data.
        /// </summary>
        /// <value><c>true</c> if the data on the page came from the local cache.</value>
        public bool ContainsCachedData
        {
            get => containsCachedData;
            set => SetField(ref containsCachedData, value);
        }

        /// <summary>
        /// Gets the command for refreshing the data.
        /// </summary>
        /// <value>The command to refresh the view.</value>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Request to refresh the data on this page.
        /// </summary>
        public virtual void Refresh()
        {
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsLoading))
            {
                OnPropertyChanged(nameof(IsReady));
            }

            if ((propertyName == nameof(Error) || propertyName == nameof(IsLoading)) && this is IEmptyCollectionViewModel)
            {
                OnPropertyChanged(nameof(IEmptyCollectionViewModel.EmptyImageName));
                OnPropertyChanged(nameof(IEmptyCollectionViewModel.EmptyMessageTitle));
                OnPropertyChanged(nameof(IEmptyCollectionViewModel.EmptyLabel));
            }
        }

        /// <summary>
        /// Creates a command for an async task.
        /// When the command is executing, it activates the loading indicator
        /// and handles any exception that occurs.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="performTask">The task to perform.</param>
        /// <returns>A command.</returns>
        protected Command<T> CreateCommand<T>(Func<T, Task> performTask)
        {
            return new Command<T>(
                (arg) =>
                {
                    if (arg is not T value)
                    {
                        return;
                    }

                    IsLoading = true;
                    performTask(value)
                        .OnSuccess((task) => IsLoading = false)
                        .OnFailure((task) =>
                        {
                            Error = task.Exception;
                            IsLoading = false;
                        });
                },
                (arg) => arg is T && !IsLoading);
        }

        /// <summary>
        /// Creates a command for an async task.
        /// When the command is executing, it activates the loading indicator
        /// and handles any exception that occurs.
        /// </summary>
        /// <param name="performTask">The task to perform.</param>
        /// <returns>A command.</returns>
        protected Command CreateCommand(Func<Task> performTask)
        {
            return new Command(
                (arg) =>
                {
                    IsLoading = true;
                    performTask()
                        .OnSuccess((task) => IsLoading = false)
                        .OnFailure((task) =>
                        {
                            Error = task.Exception;
                            IsLoading = false;
                        });
                },
                (arg) => !IsLoading);
        }

        /// <summary>
        /// Sets the value of a field and executes a command with the value.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="field">The field to set.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="value">The value to set of the field and pass to the command.</param>
        /// <param name="propertyName">The property name being set (will invoke a property changed event).</param>
        protected void SetAndExecute<T>(ref T field, ICommand command, T value, [CallerMemberName] string propertyName = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            SetField(ref field, value, propertyName);
            if (command.CanExecute(value))
            {
                command.Execute(value);
            }
        }
    }
}
