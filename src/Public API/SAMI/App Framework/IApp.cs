using System;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps
{
    /// <summary>
    /// Public interface for all SAMI apps.
    /// This interface should not be inherited directly from, but instead you should either
    /// inherit from BaseSAMIApp, or VoiceActivatedApp.
    /// </summary>
    public interface IApp : IParseable
    {
        /// <summary>
        /// Message which indicates why the app is invalid.
        /// If the app is valid, this may be null or String.Empty.
        /// The message should be user visible, and able to be spoken.
        /// </summary>
        String InvalidMessage
        {
            get;
        }

        /// <summary>
        /// Indicates whether the app can create a conversation from the given initial dialog.
        /// A conversation should be created, if one may be created.
        /// </summary>
        /// <param name="phrase">Initial dialog that the user is trying to create a conversation from.</param>
        /// <param name="createdConversation">If a conversation could be created, this is a pointer to the conversation. Otherwise, it is assumed to be null.</param>
        /// <returns>True if a conversation could be created. False otherwise.</returns>
        bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation);
    }
}