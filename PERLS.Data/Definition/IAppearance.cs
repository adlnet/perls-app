namespace PERLS.Data.Definition
{
    /// <summary>
    /// The appearance interface.
    /// </summary>
    public interface IAppearance
    {
        /// <summary>
        /// Gets the theme.
        /// </summary>
        /// <value>The theme.</value>
        ITheme Theme { get; }

        /// <summary>
        /// Gets the Logo.
        /// </summary>
        /// <value>The logo.</value>
        string Logo { get; }
    }
}
