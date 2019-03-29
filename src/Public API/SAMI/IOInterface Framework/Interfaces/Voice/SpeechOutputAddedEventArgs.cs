using System;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Event arguments for the SpeechOutputAdded event.
    /// </summary>
    public class SpeechOutputAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates what text should be output to the user.
        /// </summary>
        public String TextOutput
        {
            get;
            private set;
        }

        /// <summary>
        /// Main constructor for <see cref="SpeechOutputAddedEventArgs"/>
        /// </summary>
        /// <param name="textOutput">What text to output to the user.</param>
        public SpeechOutputAddedEventArgs(String textOutput)
        {
            TextOutput = textOutput;
        }
    }
}
