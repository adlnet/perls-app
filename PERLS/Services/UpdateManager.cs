using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PERLS.Updates;
using Xamarin.Essentials;

namespace PERLS.Services
{
    /// <summary>
    /// Dependency Service which checks if updates need to be ran (when version updated).
    /// </summary>
    public class UpdateManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        public UpdateManager()
        {
            VersionTracking.Track();
        }

        IEnumerable<string> PreviouslyRanUpdates
        {
            get
            {
                var updateString = Preferences.Get(nameof(PreviouslyRanUpdates), string.Empty);

                if (string.IsNullOrEmpty(updateString))
                {
                    return new List<string>();
                }

                return updateString.Split(';').ToList();
            }

            set
            {
                var updateString = string.Join(";", value);
                Preferences.Set(nameof(PreviouslyRanUpdates), updateString);
            }
        }

        /// <summary>
        /// This checks to see if the app needs to run an update process.
        /// </summary>
        /// <param name="availableUpdates">All of the updates to which to check against.</param>
        /// <returns><c>True</c> if a upgrade is required.</returns>
        public bool RequiresUpdate(IEnumerable<IVersionUpdate> availableUpdates)
        {
            // First time ever launched application
            var firstLaunch = VersionTracking.IsFirstLaunchEver;

            if (firstLaunch)
            {
                PreviouslyRanUpdates = availableUpdates.Select(v => v.GetUpdateName());
                return false;
            }

            return availableUpdates.Any(i => PreviouslyRanUpdates.Contains(i.GetUpdateName()) == false);
        }

        /// <summary>
        /// Runs the necessary updates by comparing known updates
        /// with updates registered in user preferences.
        /// </summary>
        /// <returns>A task containing the results of the update tasks. Returns null if no updates were found.</returns>
        public async Task<bool[]> RunCurrentUpdates()
        {
            var availableUpdates = GetAllUpdateTasks();

            // If we don't have an update, do nothing.
            if (!RequiresUpdate(availableUpdates))
            {
                return null;
            }

            // Reset enumerator just in case we're not where we think we are.
            availableUpdates.GetEnumerator().Reset();

            var requiredUpdates = availableUpdates.Where(u =>
            {
                return PreviouslyRanUpdates.Contains(u.GetUpdateName()) == false;
            });

            // Check to make sure we have an update.
            if (requiredUpdates.Count() == 0)
            {
                return null;
            }

            var tasks = requiredUpdates.Select(t => t.Update().ContinueWith(
                task =>
                {
                    if (task != null && task.Exception == null)
                    {
                        PreviouslyRanUpdates = PreviouslyRanUpdates.Union(requiredUpdates.Select(v => v.GetUpdateName()));
                    }

                    return true;
                }));

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        IEnumerable<IVersionUpdate> GetAllUpdateTasks()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IVersionUpdate).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(y => (IVersionUpdate)Activator.CreateInstance(y))
                .ToList();
        }
    }
}
