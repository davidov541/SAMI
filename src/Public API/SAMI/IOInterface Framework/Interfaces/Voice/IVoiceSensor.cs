using System;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Interface that represents a component which recognizes 
    /// phrases through voice or text. 
    /// </summary>
    public interface IVoiceSensor : IIOInterface
    {
        /// <summary>
        /// Sets the current grammar that should be used when listening for input.
        /// This may be accessed anywhere, but should only be set from the UI thread,
        /// since setting the grammar requires restarting the recognition engine.
        /// </summary>
        String GrammarName
        {
            get;
            set;
        }

        /// <summary>
        /// Event that is triggered when a new phrase is recognized.
        /// </summary>
        event EventHandler<RecognizedNewPhraseEventArgs> RecognizedNewPhrase;

        /// <summary>
        /// Returns the dialog that would be created for the given phrase.
        /// </summary>
        /// <param name="input">Phrase that the dialog represents.</param>
        /// <returns>Dialog representing the phrase.</returns>
        Dialog GetDialogForInput(String input);

        /// <summary>
        /// Makes the component pretend that the given phrase was recognized.
        /// The resulting dialog is dissemenated the same way it would be from
        /// a real recognition.
        /// </summary>
        /// <param name="input">Phrase to recognize and process.</param>
        void SimulateInput(String input);

        /// <summary>
        /// Adds a grammar provider to the list of grammar providers that can be parsed.
        /// </summary>
        /// <param name="provider">Provider to add.</param>
        void AddGrammarProvider(GrammarProvider provider);

        /// <summary>
        /// Removes a grammar provider to the list of grammar providers that can be parsed.
        /// If it does not exist in the list, nothing happens.
        /// </summary>
        /// <param name="provider">Provider to remove.</param>
        void RemoveGrammarProvider(GrammarProvider provider);

        /// <summary>
        /// Checks to see if any grammar providers need to update the grammars.
        /// You should call this regularly in order to ensure that grammars
        /// are up to date.
        /// </summary>
        void CheckForUpdates();
    }
}
