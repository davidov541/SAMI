using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Power;
using SAMI.Test.Utilities;

namespace LightsAppTests
{
    [TestClass]
    public class PowerAppTests : BaseAppTests
    {
        [TestMethod]
        public void TestInvalidAppWhenNoSwitchesAvailable()
        {
            PowerApp app = new PowerApp();
            TestInvalidApp(app, "No valid modules are currently connected to Sammie.");
        }
    }
}
