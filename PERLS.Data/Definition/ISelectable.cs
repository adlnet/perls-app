namespace PERLS.Data.Definition
{
    /// <summary>
    /// Selectable.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISelectable"/> is selected.
        /// </summary>
        /// <value><c>true</c> if is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }
    }
}
