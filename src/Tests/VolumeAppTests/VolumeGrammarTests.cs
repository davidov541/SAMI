using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Volume;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Test.Utilities;

namespace VolumeAppTests
{
    [DeploymentItem("VolumeGrammar.grxml")]
    [DeploymentItem("AdjustmentGrammar.grxml")]
    [TestClass]
    public class VolumeGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TurnUpTheVolumeGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn up the volume", expectedVals);
        }

        [TestMethod]
        public void TurnUpTheVolumeOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn up the volume on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeUpGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn the volume up", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeUpOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn the volume up on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnUpTheVolumeLevelsGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn up the volume five levels", expectedVals);
        }

        [TestMethod]
        public void TurnUpTheVolumeLevelsOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn up the volume five levels on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeUpLevelsGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn the volume up five levels", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeUpLevelsOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn the volume up five levels on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnDownTheVolumeGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn down the volume", expectedVals);
        }

        [TestMethod]
        public void TurnDownTheVolumeOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn down the volume on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeDownGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn the volume down", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeDownOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn the volume down on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnDownTheVolumeLevelsGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn down the volume five levels", expectedVals);
        }

        [TestMethod]
        public void TurnDownTheVolumeLevelsOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn down the volume five levels on the Test Source", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeDownLevelsGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("turn the volume down five levels", expectedVals);
        }

        [TestMethod]
        public void TurnTheVolumeDownLevelsOnSourceGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "turn the volume down five levels on the Test Source", expectedVals);
        }

        [TestMethod]
        public void MuteGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "mute"},
                {"levelNum", "1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            TestGrammar<VolumeApp>("mute", expectedVals);
        }

        [TestMethod]
        public void MuteGrammarOnSource()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "mute"},
                {"levelNum", "1"},
                {"source", "Test Source"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            IOInterfaceReference ref1 = new IOInterfaceReference("", "Test Source", GetConfigurationManager());
            app.AddChild(ref1);
            TestGrammar(app, GetConfigurationManager(), "mute the Test Source", expectedVals);
        }

        [TestMethod]
        public void AdjustmentLouderGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("louder", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentMoreGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("more", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentSofterGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("softer", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentQuiterGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("quieter", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentLessGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("less", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentPerfectGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"levelNum", "-1"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("perfect", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentGoodGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"levelNum", "-1"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("good", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentFineGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"levelNum", "-1"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("fine", expectedVals, VolumeApp.VolumeGrammarName);
        }

        [TestMethod]
        public void AdjustmentYesGrammar()
        {
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"levelNum", "-1"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(s => s.IsValid).Returns(true);
            controller.Setup(s => s.Name).Returns("Test Volume Controller");
            AddComponentToConfigurationManager(controller.Object);
            VolumeApp app = new VolumeApp();
            TestGrammar<VolumeApp>("yes", expectedVals, VolumeApp.VolumeGrammarName);
        }
    }
}
