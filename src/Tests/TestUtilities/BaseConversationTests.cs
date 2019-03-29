using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Test.Utilities
{
    public class BaseConversationTests : BaseSAMITests
    {
        protected Conversation CurrentConversation
        {
            get;
            set;
        }

        protected String RunSingleConversation<T>(Dictionary<String, String> dialogParameters, bool conversationIsOver = true)
            where T : Conversation
        {
            MockConfigurationManager configManager = GetConfigurationManager() as MockConfigurationManager;
            if (CurrentConversation == null)
            {
                CurrentConversation = typeof(T).GetConstructor(new Type[] { typeof(IConfigurationManager) }).Invoke(new Object[] { configManager }) as T;
            }
            Assert.IsNotNull(CurrentConversation);
            Assert.IsTrue(CurrentConversation.TryAddDialog(new Dialog(dialogParameters, String.Empty)));
            String response = CurrentConversation.Speak();
            Assert.AreEqual(conversationIsOver, CurrentConversation.ConversationIsOver);
            return response;
        }
    }
}
