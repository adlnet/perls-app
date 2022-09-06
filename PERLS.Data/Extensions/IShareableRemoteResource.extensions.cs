using System;
using PERLS.Data.Definition;
using Xamarin.Essentials;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// Allows sharing of IShareableRemoteResources.
    /// </summary>
    public static class IShareableRemoteResourceExtensions
    {
        /// <summary>
        /// Creates a share text request based off the given shareable content.
        /// </summary>
        /// <param name="shareable">The shareable content.</param>
        /// <returns>A share text request based off the shareable content.</returns>
        public static ShareTextRequest GetShareRequest(this IShareableRemoteResource shareable)
        {
            if (shareable == null)
            {
                throw new ArgumentNullException(nameof(shareable));
            }

            if (shareable.CanBeShared == false)
            {
                return null;
            }

            return new ShareTextRequest
            {
                Uri = shareable.ShareableUri.ToString(),
                Title = shareable.Name,
                Text = shareable.ShareableDescription,
            };
        }
    }
}
