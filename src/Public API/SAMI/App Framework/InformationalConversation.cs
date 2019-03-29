using System;
using SAMI.Configuration;

namespace SAMI.Apps
{
    /// <summary>
    /// Represents a conversation which just states something, and then ends.
    /// This is useful for conversations that are just information for the user,
    /// or alerts to the user from previous input.
    /// </summary>
    public class InformationalConversation : Conversation
    {
        /// <summary>
        /// Value of Command, supplied by the grammar, for which
        /// this conversation should be created.
        /// </summary>
        protected override string CommandName
        {
            get 
            {
                return String.Empty;
            }
        }

        private String _information;
        /// <summary>
        /// Basic constructor for <see cref="InformationalConversation"/>
        /// </summary>
        /// <param name="information">Message to pass back to the user.</param>
        public InformationalConversation(IConfigurationManager configManager, String information)
            : base(configManager)
        {
            _information = information;
            ReadyToSpeak = true;
        }

        /// <summary>
        /// Processes the last input, returning what should be said back to the user.
        /// </summary>
        /// <returns>Phrase which should be said back to the user.</returns>
        public override String Speak()
        {
            base.Speak();
            ConversationIsOver = true;
            return _information;
        }
    }
}
