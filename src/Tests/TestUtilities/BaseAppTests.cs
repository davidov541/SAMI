using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Test.Utilities
{
    public class BaseAppTests : BaseSAMITests
    {
        public void TestInvalidApp(VoiceActivatedApp app, String invalidMessage)
        {
            Assert.IsFalse(app.IsValid);
            Assert.AreEqual(invalidMessage, app.InvalidMessage);

            app.Initialize(GetConfigurationManager());
            if (app._provider != null)
            {
                Assert.IsNull(app._provider.MainGrammar);
            }
            Assert.IsFalse(app.IsValid);
            Assert.AreEqual(invalidMessage, app.InvalidMessage);
        }

        public void TestInvalidAppAfterInitialization(VoiceActivatedApp app, String invalidMessage)
        {
            Assert.IsTrue(app.IsValid);
            Assert.AreEqual(String.Empty, app.InvalidMessage);

            app.Initialize(GetConfigurationManager());
            Assert.IsNotNull(app._provider);
            Assert.IsNull(app._provider.MainGrammar);
            Assert.IsFalse(app.IsValid);
            Assert.AreEqual(invalidMessage, app.InvalidMessage);
        }
    }
}
