using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Dimmer;
using SAMI.Test.Utilities;

namespace DimmerAppTests
{
    [TestClass]
    public class DimmerAppTests : BaseAppTests
    {
        [TestMethod]
        public void TestInvalidAppWhenNoSwitchesAvailable()
        {
            DimmerApp app = new DimmerApp();
            TestInvalidApp(app, "No valid modules are currently connected to Sammie.");
        }
    }
}
