using System;

namespace SAMI.Apps
{
    internal class AsyncAlertEventArgs : EventArgs
    {
        public Conversation StartedConversation
        {
            get;
            set;
        }

        public AsyncAlertEventArgs(Conversation conv)
        {
            StartedConversation = conv;
        }
    }
}
