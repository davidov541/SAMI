using System;
using System.Linq;
using System.Reflection;
using SAMI.Apps;
using SAMI.Configuration;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Base class for SAMI apps which are activated through voice.
    /// This class adds functionality that is needed for speech recognition.
    /// This is the preferred class to inherit from for all apps, if you have a custom
    /// conversation class to use.
    /// </summary>
    /// <typeparam name="T">Conversation type which is created by this app.</typeparam>
    public abstract class VoiceActivatedApp<T> : VoiceActivatedApp where T : Conversation
    {
        public VoiceActivatedApp()
            : base()
        {
            Provider = new GrammarProvider(null);
        }

        /// <summary>
        /// Indicates whether the app can create a conversation from the given initial dialog.
        /// A conversation should be created, if one may be created.
        /// </summary>
        /// <param name="phrase">Initial dialog that the user is trying to create a conversation from.</param>
        /// <param name="createdConversation">If a conversation could be created, this is a pointer to the conversation. Otherwise, it is assumed to be null.</param>
        /// <returns>True if a conversation could be created. False otherwise.</returns>
        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Single(ci => ci.GetParameters().Count() == 1).Invoke(new Object[1] { ConfigManager }) as Conversation;
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }
    }
}
