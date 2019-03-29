namespace SAMI.Persistence
{
    /// <summary>
    /// Describes what type of parseable element this is.
    /// </summary>
    public enum ParseableElementType
    {
        /// <summary>
        /// This parseable element is an app.
        /// </summary>
        App,
        /// <summary>
        /// This parseable element is an IO interface
        /// </summary>
        IOInterface,
        /// <summary>
        /// This parseable element is neither an app, nor an IO interface,
        /// and instead serves to preserve information needed by an app or IO interface.
        /// These should not be persisted at the top level, but should instead be contained
        /// inside of an app or IO interface.
        /// </summary>
        Support,
    }
}
