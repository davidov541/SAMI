using System;

namespace SAMI.Error
{
    internal class SAMIUserException : Exception
    {
        public String MessageToUser
        {
            get;
            set;
        }

        public SAMIUserException(String messageToUser)
        {
            MessageToUser = messageToUser;
        }
    }
}
