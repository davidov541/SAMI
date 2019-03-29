using System;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Interface describing a component which outputs text to the user in some way.
    /// </summary>
    public interface ITextOutputController : IOutputController
    {
        /// <summary>
        /// Event which is called whenever some output is ready for user visibility.
        /// </summary>
        event EventHandler<SpeechOutputAddedEventArgs> SpeechOutputAdded;
    }
}
