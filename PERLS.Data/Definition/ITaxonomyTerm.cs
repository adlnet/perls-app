namespace PERLS.Data.Definition
{
    /// <summary>
    /// Any term (currently, tags and topics) from the server.
    /// </summary>
    public interface ITaxonomyTerm : IRemoteResource
    {
        /// <summary>
        /// Gets the term ID for this tag.
        /// </summary>
        /// <value>The term id.</value>
        int Tid { get; }
    }
}
