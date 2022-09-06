using System.Threading.Tasks;

namespace PERLS.Updates
{
    /// <summary>
    /// Base class for version updates.
    /// </summary>
    public abstract class VersionUpdate : IVersionUpdate
    {
        /// <inheritdoc/>
        public string GetUpdateName()
        {
            return this.GetType().Name;
        }

        /// <inheritdoc/>
        public abstract Task Update();
    }
}
