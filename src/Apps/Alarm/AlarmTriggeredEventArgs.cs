using System;

namespace SAMI.Apps.Alarm
{
    internal class AlarmTriggeredEventArgs : EventArgs
    {
        public Conversation StartedConversation
        {
            get;
            set;
        }

        public AlarmTriggeredEventArgs(Conversation conv)
        {
            StartedConversation = conv;
        }
    }
}
