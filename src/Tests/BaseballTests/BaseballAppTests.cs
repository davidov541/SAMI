using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Baseball;
using SAMI.IOInterfaces.Interfaces.Baseball;
using SAMI.Test.Utilities;

namespace BaseballTests
{
    [TestClass]
    public class BaseballAppTests : BaseSAMITests
    {
        [TestMethod]
        public void AppIsInvalidWithNoScores()
        {
            BaseballApp app = new BaseballApp();
            Assert.IsFalse(app.IsValid);
            Assert.AreEqual("We cannot connect to the baseball score server currently.", app.InvalidMessage);
        }

        [TestMethod]
        public void AppIsValidWithScores()
        {
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>();
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();
            app.Initialize(GetConfigurationManager());
            Assert.IsTrue(app.IsValid);
            Assert.AreEqual(String.Empty, app.InvalidMessage);
        }

        [TestMethod]
        public void RemoteReferencesAreFilled()
        {
            BaseballApp app = new BaseballApp();
            app.Initialize(GetConfigurationManager());
            app.AddChild(new IOInterfaceReference());
            Assert.IsFalse(app.IsValid);
            Assert.AreEqual(1, app.References.Count());
        }
    }
}
