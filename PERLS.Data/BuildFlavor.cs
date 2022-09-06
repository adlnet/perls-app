namespace PERLS
{
    /// <summary>
    /// Enumerates possible build flavors.
    /// </summary>
    public enum BuildFlavor
    {
        /// <summary>
        /// A local development build.
        /// </summary>
        Dev,

        /// <summary>
        /// A continuous integration build.
        /// </summary>
        CI,

        /// <summary>
        /// A canary build.
        /// </summary>
        Canary,

        /// <summary>
        /// A production build.
        /// </summary>
        Release,
    }
}
