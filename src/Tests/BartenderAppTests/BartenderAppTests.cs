using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Bartender;
using SAMI.Test.Utilities;

namespace BartenderAppTests
{
    [TestClass]
    public class BartenderAppTests : BaseAppTests
    {
        [TestMethod]
        public void BartenderAppInvalidWithoutControllersTest()
        {
            BartenderApp app = new BartenderApp();
            TestInvalidApp(app, "No bartenders are available to use.");
        }
    }
}
