using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenZWaveDotNet;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.ZWave;

namespace SAMI.Test.ZWave
{
    [TestClass]
    public class ZWaveControllerTests
    {
        [TestMethod]
        public void ZWaveControllerConstructor()
        {
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>(MockBehavior.Strict);

            ZWaveController controller = new ZWaveController(configManager.Object);

            Assert.IsFalse(controller.IsInitialized);
        }

        [TestMethod]
        public void ZWaveControllerInitialization()
        {
            ZWaveController controller = SetupInitializedController();

            VerifyInitializedController(controller);
        }

        [TestMethod]
        public void ZWaveControllerInitializationCalledTwice()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);
        }

        [TestMethod]
        public void ZWaveControllerSetBoolValueId()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(1, 2, ZWValueID.ValueGenre.Basic, 3, 4, 5, ZWValueID.ValueType.Bool, 6);
            _zWaveManager.Setup(s => s.SetValue(valueId, true));
            controller.SetValue(valueId, true);
            _zWaveManager.Verify(s => s.SetValue(valueId, true), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerSetByteValueId()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(1, 2, ZWValueID.ValueGenre.Basic, 3, 4, 5, ZWValueID.ValueType.Byte, 6);
            _zWaveManager.Setup(s => s.SetValue(valueId, 0x25));
            controller.SetValue(valueId, 0x25);
            _zWaveManager.Verify(s => s.SetValue(valueId, 0x25), Times.Exactly(1));
        }

        #region Notifications
        [TestMethod]
        public void ZWaveControllerAllQueriesComplete()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            RaiseNotificationNoNodes(ZWNotification.Type.AllNodesQueried, new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5));

            Assert.IsTrue(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerAllQueriesCompleteSomeDead()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            RaiseNotificationNoNodes(ZWNotification.Type.AllNodesQueriedSomeDead, new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5));

            Assert.IsTrue(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerNodeAddedNotConnected()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            MockZWaveNode node = RaiseNotification(ZWNotification.Type.NodeAdded, false, new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5));

            Assert.IsFalse(controller.IsInitialized);
            Assert.IsTrue(node.IsConnected);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerNodeAddedPairing()
        {
            ZWaveController controller = SetupInitializedController();
            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);
            VerifyInitializedController(controller);
            MockZWaveNode pairingNode = new MockZWaveNode();
            Assert.AreEqual(0, pairingNode.NodeId);
            _zWaveManager.Setup(s => s.BeginControllerCommand(0, ZWControllerCommand.AddDevice, false, 0)).Returns(true);
            Mock<IZWavePairingMonitor> monitor = new Mock<IZWavePairingMonitor>(MockBehavior.Strict);
            monitor.Setup(m => m.PairingStarted());

            Assert.IsTrue(controller.TryStartPairing(pairingNode, monitor.Object));

            _configManager.Setup(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>()).Returns(new List<ZWaveNode>());
            _zWaveManager.Raise(s => s.OnNotification += null, new NotificationEventArgs(_zWaveManager.Object, (byte)5, ZWNotification.Type.NodeAdded, null, 2));

            Assert.IsTrue(pairingNode.IsConnected);
            Assert.AreEqual(5, pairingNode.NodeId);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
            monitor.Verify(m => m.PairingStarted(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerNodeAddedNoNodes()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            RaiseNotificationNoNodes(ZWNotification.Type.NodeAdded, new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5));

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerNodeRemoved()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            MockZWaveNode node = RaiseNotification(ZWNotification.Type.NodeRemoved, true, new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5));

            Assert.IsFalse(controller.IsInitialized);
            Assert.IsFalse(node.IsConnected);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerNodeRemovedNoNodes()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            RaiseNotificationNoNodes(ZWNotification.Type.NodeRemoved, new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5));

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerValueAdded()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            MockZWaveNode node = RaiseNotification(ZWNotification.Type.ValueAdded, false, valueId);

            Assert.IsFalse(controller.IsInitialized);
            Assert.AreEqual(1, node.MockValueIds.Count());
            Assert.AreSame(valueId, node.MockValueIds.First());
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerValueAddedNoNodes()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            RaiseNotificationNoNodes(ZWNotification.Type.ValueAdded, valueId);

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerValueRemoved()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            MockZWaveNode node = new MockZWaveNode();
            node.NodeId = 1;
            node.AddValueId(valueId);
            RaiseNotification(ZWNotification.Type.ValueRemoved, valueId, node);

            Assert.IsFalse(controller.IsInitialized);
            Assert.AreEqual(0, node.MockValueIds.Count());
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerValueRemovedNoNodes()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            RaiseNotificationNoNodes(ZWNotification.Type.ValueRemoved, valueId);

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerTryStartPairingTrue()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            RaiseNotificationNoNodes(ZWNotification.Type.DriverReady, valueId);

            _zWaveManager.Setup(s => s.BeginControllerCommand(2, ZWControllerCommand.AddDevice, false, 0)).Returns(true);

            Mock<IZWavePairingMonitor> monitor = new Mock<IZWavePairingMonitor>(MockBehavior.Strict);
            monitor.Setup(m => m.PairingStarted());

            MockZWaveNode node = new MockZWaveNode();

            Assert.IsTrue(controller.TryStartPairing(node, monitor.Object));

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
            _zWaveManager.Verify(s => s.BeginControllerCommand(2, ZWControllerCommand.AddDevice, false, 0), Times.Exactly(1));
            monitor.Verify(m => m.PairingStarted(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerTryStartPairingAlreadyPairing()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            RaiseNotificationNoNodes(ZWNotification.Type.DriverReady, valueId);

            _zWaveManager.Setup(s => s.BeginControllerCommand(2, ZWControllerCommand.AddDevice, false, 0)).Returns(true);

            Mock<IZWavePairingMonitor> monitor = new Mock<IZWavePairingMonitor>(MockBehavior.Strict);
            monitor.Setup(m => m.PairingStarted());

            MockZWaveNode node = new MockZWaveNode();
            Assert.IsTrue(controller.TryStartPairing(node, monitor.Object));

            MockZWaveNode node2 = new MockZWaveNode();
            Assert.IsFalse(controller.TryStartPairing(node2, monitor.Object));

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
            _zWaveManager.Verify(s => s.BeginControllerCommand(2, ZWControllerCommand.AddDevice, false, 0), Times.Exactly(1));
            monitor.Verify(m => m.PairingStarted(), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerTryStartPairingFalse()
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);
            RaiseNotificationNoNodes(ZWNotification.Type.DriverReady, valueId);

            _zWaveManager.Setup(s => s.BeginControllerCommand(2, ZWControllerCommand.AddDevice, false, 0)).Returns(false);

            MockZWaveNode node = new MockZWaveNode();

            Mock<IZWavePairingMonitor> monitor = new Mock<IZWavePairingMonitor>(MockBehavior.Strict);

            Assert.IsFalse(controller.TryStartPairing(node, monitor.Object));

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
            _zWaveManager.Verify(s => s.BeginControllerCommand(2, ZWControllerCommand.AddDevice, false, 0), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveControllerValueChanged()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.ValueChanged);
        }

        [TestMethod]
        public void ZWaveControllerValueRefreshed()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.ValueRefreshed);
        }

        [TestMethod]
        public void ZWaveController()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.NodeQueriesComplete);
        }

        [TestMethod]
        public void ZWaveControllerNodeProtocolInfo()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.NodeProtocolInfo);
        }

        [TestMethod]
        public void ZWaveControllerNodeNew()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.NodeNew);
        }

        [TestMethod]
        public void ZWaveControllerNodeNaming()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.NodeNaming);
        }

        [TestMethod]
        public void ZWaveControllerNodeEvent()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.NodeEvent);
        }

        [TestMethod]
        public void ZWaveControllerSceneEvent()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.SceneEvent);
        }

        [TestMethod]
        public void ZWaveControllerPollingEnabled()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.PollingEnabled);
        }

        [TestMethod]
        public void ZWaveControllerPollingDisabled()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.PollingDisabled);
        }

        [TestMethod]
        public void ZWaveControllerNotification()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.Notification);
        }

        [TestMethod]
        public void ZWaveControllerGroup()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.Group);
        }

        [TestMethod]
        public void ZWaveControllerEssentialNodeQueriesComplete()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.EssentialNodeQueriesComplete);
        }

        [TestMethod]
        public void ZWaveControllerDriverReset()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.DriverReset);
        }

        [TestMethod]
        public void ZWaveControllerDriverReady()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.DriverReady);
        }

        [TestMethod]
        public void ZWaveControllerDriverFailed()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.DriverFailed);
        }

        [TestMethod]
        public void ZWaveControllerDeleteButton()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.DeleteButton);
        }

        [TestMethod]
        public void ZWaveControllerCreateButton()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.CreateButton);
        }

        [TestMethod]
        public void ZWaveControllerButtonOn()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.ButtonOn);
        }

        [TestMethod]
        public void ZWaveControllerButtonOff()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.ButtonOff);
        }

        [TestMethod]
        public void ZWaveControllerAwakeNodesQueried()
        {
            TestNoChangeAfterNotification(ZWNotification.Type.AwakeNodesQueried);
        }

        private void TestNoChangeAfterNotification(ZWNotification.Type notificationType)
        {
            ZWaveController controller = SetupInitializedController();

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            VerifyInitializedController(controller);

            ZWValueID valueId = new ZWValueID(0, 1, ZWValueID.ValueGenre.Basic, 2, 3, 4, ZWValueID.ValueType.Bool, 5);

            RaiseNotificationNoNodes(ZWNotification.Type.ValueChanged, valueId);

            Assert.IsFalse(controller.IsInitialized);
            _configManager.Verify(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>(), Times.Exactly(1));
        }

        private void RaiseNotificationNoNodes(ZWNotification.Type type, ZWValueID valueId)
        {
            _configManager.Setup(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>()).Returns(new List<ZWaveNode>());
            _zWaveManager.Raise(s => s.OnNotification += null, new NotificationEventArgs(_zWaveManager.Object, (byte)1, type, valueId, 2));
        }

        private MockZWaveNode RaiseNotification(ZWNotification.Type type, bool nodeIsConnected, ZWValueID valueId)
        {
            MockZWaveNode node = new MockZWaveNode();
            node.NodeId = 1;
            node.IsConnected = nodeIsConnected;
            RaiseNotification(type, valueId, node);
            return node;
        }

        private void RaiseNotification(ZWNotification.Type type, ZWValueID valueId, MockZWaveNode node)
        {
            _configManager.Setup(s => s.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>()).Returns(new List<ZWaveNode> { node });
            _zWaveManager.Raise(s => s.OnNotification += null, new NotificationEventArgs(_zWaveManager.Object, (byte)1, type, valueId, 2));
        }

        #endregion

        #region Utilities

        private const String TestCOMPort = "TestCOM";
        private Mock<IInternalConfigurationManager> _configManager;
        private Mock<IZWaveManager> _zWaveManager;
        private Mock<IZWaveOptions> _zWaveOptions;
        private readonly String ConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ZWave");
        private ZWaveController SetupInitializedController()
        {
            _configManager = new Mock<IInternalConfigurationManager>(MockBehavior.Strict);
            _configManager.Setup(s => s.ZWaveCOM).Returns(TestCOMPort);

            _zWaveManager = new Mock<IZWaveManager>(MockBehavior.Strict);
            _zWaveManager.Setup(m => m.Create());
            _zWaveManager.Setup(m => m.AddDriver(TestCOMPort, ZWControllerInterface.Serial));

            _zWaveOptions = new Mock<IZWaveOptions>(MockBehavior.Strict);
            _zWaveOptions.Setup(s => s.Create(ConfigPath, ConfigPath, ""));
            _zWaveOptions.Setup(s => s.Lock());

            ZWaveController controller = new ZWaveController(_configManager.Object, _zWaveManager.Object, _zWaveOptions.Object);

            _configManager.Raise(s => s.InitializationComplete += null, EventArgs.Empty);

            return controller;
        }

        private void VerifyInitializedController(ZWaveController controller)
        {
            Assert.IsFalse(controller.IsInitialized);

            _zWaveOptions.Verify(s => s.Create(ConfigPath, ConfigPath, ""), Times.Exactly(1));
            _zWaveOptions.Verify(s => s.Lock(), Times.Exactly(1));
            _zWaveManager.Verify(m => m.Create(), Times.Exactly(1));
            _zWaveManager.Verify(m => m.AddDriver(TestCOMPort, ZWControllerInterface.Serial), Times.Exactly(1));
            _configManager.Verify(c => c.ZWaveCOM, Times.Exactly(1));
        }
        #endregion
    }
}
