using System;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Event arguments passed around when a new phrase has been recognized by the speech system.
    /// </summary>
    public class RecognizedNewPhraseEventArgs : EventArgs
    {
        /// <summary>
        /// The dialog that was recognized.
        /// </summary>
        public Dialog SeenDialog
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor for RecognizedNewPhraseEventArgs.
        /// </summary>
        /// <param name="dialog">The dialog that has been recognized.</param>
        public RecognizedNewPhraseEventArgs(Dialog dialog)
        {
            SeenDialog = dialog;
        }
    }
}
