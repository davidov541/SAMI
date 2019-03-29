using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Volume;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Test.Utilities;

namespace VolumeAppTests
{
    [TestClass]
    public class VolumeConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TurnUpVolumeLevels()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference>
                {
                    new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),
                });

            Assert.AreEqual("OK", RunSingleConversation<VolumeConversation>(expectedVals));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnUpVolumeLevelsUseSource()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
                {"source", "Test Volume Controller"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            Mock<IVolumeController> controller2 = new Mock<IVolumeController>(MockBehavior.Strict);
            controller2.Setup(v => v.IsValid).Returns(true);
            controller2.Setup(v => v.Name).Returns("Test Volume Controller Two");
            AddComponentToConfigurationManager(controller2.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference>
                {
                    new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),
                    new IOInterfaceReference("", "Test Volume Controller Two", GetConfigurationManager()),
                });

            Assert.AreEqual("OK", RunSingleConversation<VolumeConversation>(expectedVals));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnUpVolumeAdjustmentStart()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedVals, false));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnUpVolumeAdjustmentLouder()
        {
            Semaphore volumeSetLock = new Semaphore(0, 2);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedValsSecond, false));

            volumeSetLock.WaitOne();
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(2));
        }

        [TestMethod]
        public void TurnUpVolumeAdjustmentLouderUseSource()
        {
            Semaphore volumeSetLock = new Semaphore(0, 2);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", "Test Volume Controller"},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            Mock<IVolumeController> controller2 = new Mock<IVolumeController>(MockBehavior.Strict);
            controller2.Setup(v => v.IsValid).Returns(true);
            controller2.Setup(v => v.Name).Returns("Test Volume Controller Two");
            AddComponentToConfigurationManager(controller2.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> 
            { 
                new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()), 
                new IOInterfaceReference("", "Test Volume Controller Two", GetConfigurationManager()), 
            });
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedValsSecond, false));

            volumeSetLock.WaitOne();
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(2));
        }

        [TestMethod]
        public void TurnUpVolumeAdjustmentSofter()
        {
            Semaphore volumeSetLock = new Semaphore(0, 2);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));
            controller.SetupSet(v => v.Volume = 0.45).Callback(() => volumeSetLock.Release());

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedValsSecond, false));

            volumeSetLock.WaitOne();
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnUpVolumeAdjustmentPerfect()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"levelNum", "-1"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));

            Assert.AreEqual("Good", RunSingleConversation<VolumeConversation>(expectedValsSecond));
        }

        [TestMethod]
        public void TurnDownVolumeLevels()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.45).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});

            Assert.AreEqual("OK", RunSingleConversation<VolumeConversation>(expectedVals));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnDownVolumeAdjustmentStart()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.45).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedVals, false));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnDownVolumeAdjustmentLouder()
        {
            Semaphore volumeSetLock = new Semaphore(0, 2);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "up"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.45).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(1));
            controller.SetupSet(v => v.Volume = 0.55).Callback(() => volumeSetLock.Release());

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedValsSecond, false));

            volumeSetLock.WaitOne();
            controller.VerifySet(v => v.Volume = 0.55, Times.Exactly(1));
        }

        [TestMethod]
        public void TurnDownVolumeAdjustmentSofter()
        {
            Semaphore volumeSetLock = new Semaphore(0, 2);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "5"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.45).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(1));

            Assert.AreEqual("How is that?", RunSingleConversation<VolumeConversation>(expectedValsSecond, false));

            volumeSetLock.WaitOne();
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(2));
        }

        [TestMethod]
        public void TurnDownVolumeAdjustmentPerfect()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedValsFirst = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "down"},
                {"levelNum", "-1"},
                {"source", ""},
            };
            Dictionary<String, String> expectedValsSecond = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"levelNum", "-1"},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.45).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});
            RunSingleConversation<VolumeConversation>(expectedValsFirst, false);
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.45, Times.Exactly(1));

            Assert.AreEqual("Good", RunSingleConversation<VolumeConversation>(expectedValsSecond));
        }

        [TestMethod]
        public void MuteVolumeLevels()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "mute"},
                {"levelNum", "0"},
                {"source", ""},
            };
            Mock<IVolumeController> controller = new Mock<IVolumeController>(MockBehavior.Strict);
            controller.Setup(v => v.IsValid).Returns(true);
            controller.Setup(v => v.Name).Returns("Test Volume Controller");
            controller.SetupGet(v => v.Volume).Returns(0.5);
            controller.SetupSet(v => v.Volume = 0.0).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> { new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),});

            Assert.AreEqual("OK", RunSingleConversation<VolumeConversation>(expectedVals));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller.VerifySet(v => v.Volume = 0.0, Times.Exactly(1));
        }

        [TestMethod]
        public void MuteVolumeLevelsMultipleControllers()
        {
            Semaphore volumeSetLock = new Semaphore(0, 2);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "mute"},
                {"levelNum", "0"},
                {"source", ""},
            };
            Mock<IVolumeController> controller1 = new Mock<IVolumeController>(MockBehavior.Strict);
            controller1.Setup(v => v.IsValid).Returns(true);
            controller1.Setup(v => v.Name).Returns("Test Volume Controller");
            controller1.SetupGet(v => v.Volume).Returns(0.5);
            controller1.SetupSet(v => v.Volume = 0.0).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller1.Object);
            Mock<IVolumeController> controller2 = new Mock<IVolumeController>(MockBehavior.Strict);
            controller2.Setup(v => v.IsValid).Returns(true);
            controller2.Setup(v => v.Name).Returns("Test Volume Controller Two");
            controller2.SetupGet(v => v.Volume).Returns(0.5);
            controller2.SetupSet(v => v.Volume = 0.0).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller2.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> 
            { 
                new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),
                new IOInterfaceReference("", "Test Volume Controller Two", GetConfigurationManager()),
            });

            Assert.AreEqual("OK", RunSingleConversation<VolumeConversation>(expectedVals));

            Assert.IsTrue(volumeSetLock.WaitOne());
            Assert.IsTrue(volumeSetLock.WaitOne());
            controller1.VerifySet(v => v.Volume = 0.0, Times.Exactly(1));
            controller2.VerifySet(v => v.Volume = 0.0, Times.Exactly(1));
        }

        [TestMethod]
        public void MuteVolumeLevelsMultipleControllersUseSource()
        {
            Semaphore volumeSetLock = new Semaphore(0, 1);
            Dictionary<String, String> expectedVals = new Dictionary<string, string>
            {
                {"Command", "volume"},
                {"direction", "mute"},
                {"levelNum", "0"},
                {"source", "Test Volume Controller"},
            };
            Mock<IVolumeController> controller1 = new Mock<IVolumeController>(MockBehavior.Strict);
            controller1.Setup(v => v.IsValid).Returns(true);
            controller1.Setup(v => v.Name).Returns("Test Volume Controller");
            controller1.SetupGet(v => v.Volume).Returns(0.5);
            controller1.SetupSet(v => v.Volume = 0.0).Callback(() => volumeSetLock.Release());
            AddComponentToConfigurationManager(controller1.Object);
            Mock<IVolumeController> controller2 = new Mock<IVolumeController>(MockBehavior.Strict);
            controller2.Setup(v => v.IsValid).Returns(true);
            controller2.Setup(v => v.Name).Returns("Test Volume Controller Two");
            AddComponentToConfigurationManager(controller2.Object);
            CurrentConversation = new VolumeConversation(GetConfigurationManager(), new List<IOInterfaceReference> 
            { 
                new IOInterfaceReference("", "Test Volume Controller", GetConfigurationManager()),
                new IOInterfaceReference("", "Test Volume Controller Two", GetConfigurationManager()),
            });

            Assert.AreEqual("OK", RunSingleConversation<VolumeConversation>(expectedVals));

            Assert.IsTrue(volumeSetLock.WaitOne());
            controller1.VerifySet(v => v.Volume = 0.0, Times.Exactly(1));
        }
    }
}
