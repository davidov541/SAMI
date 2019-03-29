using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenZWaveDotNet;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Persistence;

namespace SAMI.Test.ZWave
{
    [TestClass]
    public class ZWaveNodeTests
    {
        [TestMethod]
        public void ZWaveNodeConstructor()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.IsTrue(node.IsValid);
            Assert.AreEqual(0, node.Children.Count());
            Assert.AreEqual(0, node.MockValueIds.Count());
        }

        [TestMethod]
        public void ZWaveNodeProperties()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.AreEqual(1, node.Properties.Count());
            PersistentProperty prop = node.Properties.Single();
            Assert.AreEqual("NodeId", prop.Name);
            prop.Setter("100");
            Assert.AreEqual(100, node.NodeId);
            Assert.AreEqual("100", prop.Getter());
        }

        [TestMethod]
        public void ZWaveNodeIsValidInitializedAndConnected()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(true);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();

            node.IsConnected = true;

            Assert.IsTrue(node.IsValid);
        }

        [TestMethod]
        public void ZWaveNodeIsValidInitialized()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(true);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();

            node.IsConnected = false;

            Assert.IsFalse(node.IsValid);
        }

        [TestMethod]
        public void ZWaveNodeIsValidConnected()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();

            node.IsConnected = true;

            Assert.IsTrue(node.IsValid);
        }

        [TestMethod]
        public void ZWaveNodeIsValidNotInitializedOrConnected()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();

            node.IsConnected = false;

            Assert.IsTrue(node.IsValid);
        }

        [TestMethod]
        public void ZWaveNodeTryStartPairingReturnsTrue()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(true);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            controller.Setup(s => s.TryStartPairing(node, null)).Returns(true);

            Assert.IsTrue(node.TryStartPairing(null));

            Assert.IsFalse(node.IsValid);
            controller.Verify(s => s.TryStartPairing(node, null), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveNodeTryStartPairingReturnsFalse()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(true);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            controller.Setup(s => s.TryStartPairing(node, null)).Returns(false);

            Assert.IsFalse(node.TryStartPairing(null));

            Assert.IsFalse(node.IsValid);
            controller.Verify(s => s.TryStartPairing(node, null), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveNodeTryStartPairingCallback()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(true);
            ZWaveNode.SetController(controller.Object);
            Mock<IZWavePairingMonitor> monitor = new Mock<IZWavePairingMonitor>(MockBehavior.Strict);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>(MockBehavior.Strict);
            MockZWaveNode node = new MockZWaveNode();
            controller.Setup(s => s.TryStartPairing(node, monitor.Object)).Returns(true);

            Assert.IsTrue(node.TryStartPairing(monitor.Object));
            node.NodeId = 1;

            Assert.AreEqual(1, node.NodeId);
        }

        [TestMethod]
        public void ZWaveNodeTryStartPairingNoCallback()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(true);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            controller.Setup(s => s.TryStartPairing(node, null)).Returns(true);

            Assert.IsTrue(node.TryStartPairing(null));
            node.NodeId = 1;

            Assert.AreEqual(1, node.NodeId);
            controller.Verify(s => s.TryStartPairing(node, null), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveNodesAddValueId()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.AreEqual(0, node.MockValueIds.Count());

            uint homeId = 0;
            byte nodeId = 1;
            ZWValueID.ValueGenre genre = ZWValueID.ValueGenre.Basic;
            byte commandClass = 2;
            byte instance = 3;
            byte valueIndex = 4;
            ZWValueID.ValueType type = ZWValueID.ValueType.Bool;
            byte pollIntensity = 5;
            ZWValueID id = new ZWValueID(homeId, nodeId, genre, commandClass, instance, valueIndex, type, pollIntensity);
            node.AddValueId(id);

            Assert.AreEqual(1, node.MockValueIds.Count());
            Assert.AreSame(id, node.MockValueIds.Single());
        }

        [TestMethod]
        public void ZWaveNodesAddValueIdDuplicate()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.AreEqual(0, node.MockValueIds.Count());

            uint homeId = 0;
            byte nodeId = 1;
            ZWValueID.ValueGenre genre = ZWValueID.ValueGenre.Basic;
            byte commandClass = 2;
            byte instance = 3;
            byte valueIndex = 4;
            ZWValueID.ValueType type = ZWValueID.ValueType.Bool;
            byte pollIntensity = 5;
            ZWValueID id = new ZWValueID(homeId, nodeId, genre, commandClass, instance, valueIndex, type, pollIntensity);
            node.AddValueId(id);
            node.AddValueId(id);

            Assert.AreEqual(1, node.MockValueIds.Count());
            Assert.AreSame(id, node.MockValueIds.Single());
        }

        [TestMethod]
        public void ZWaveNodesRemoveValueId()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.AreEqual(0, node.MockValueIds.Count());

            uint homeId = 0;
            byte nodeId = 1;
            ZWValueID.ValueGenre genre = ZWValueID.ValueGenre.Basic;
            byte commandClass = 2;
            byte instance = 3;
            byte valueIndex = 4;
            ZWValueID.ValueType type = ZWValueID.ValueType.Bool;
            byte pollIntensity = 5;
            ZWValueID id = new ZWValueID(homeId, nodeId, genre, commandClass, instance, valueIndex, type, pollIntensity);
            node.AddValueId(id);

            node.RemoveValueId(id);
            Assert.AreEqual(0, node.MockValueIds.Count());
        }

        [TestMethod]
        public void ZWaveNodesRemoveValueIdNotAvailable()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.AreEqual(0, node.MockValueIds.Count());

            uint homeId = 0;
            byte nodeId = 1;
            ZWValueID.ValueGenre genre = ZWValueID.ValueGenre.Basic;
            byte commandClass = 2;
            byte instance = 3;
            byte valueIndex = 4;
            ZWValueID.ValueType type = ZWValueID.ValueType.Bool;
            byte pollIntensity = 5;
            ZWValueID id = new ZWValueID(homeId, nodeId, genre, commandClass, instance, valueIndex, type, pollIntensity);

            node.RemoveValueId(id);
            Assert.AreEqual(0, node.MockValueIds.Count());
        }

        [TestMethod]
        public void ZWaveNodeAddChildren()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.IsInitialized).Returns(false);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            Assert.AreEqual(0, node.Children.Count());

            Mock<IParseable> mockParseable = new Mock<IParseable>();
            node.AddChild(mockParseable.Object);

            Assert.AreEqual(1, node.Children.Count());
            Assert.AreSame(mockParseable.Object, node.Children.First());
        }

        [TestMethod]
        public void ZWaveNodeSetValueBool()
        {
            uint homeId = 0;
            byte nodeId = 1;
            ZWValueID.ValueGenre genre = ZWValueID.ValueGenre.Basic;
            byte commandClass = 2;
            byte instance = 3;
            byte valueIndex = 4;
            ZWValueID.ValueType type = ZWValueID.ValueType.Bool;
            byte pollIntensity = 5;
            ZWValueID id = new ZWValueID(homeId, nodeId, genre, commandClass, instance, valueIndex, type, pollIntensity);

            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.SetValue(id, true));
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            node.AddValueId(id);

            node.MockSetValue(valueIndex, commandClass, true);

            controller.Verify(s => s.SetValue(id, true), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveNodeSetValueByte()
        {
            uint homeId = 0;
            byte nodeId = 1;
            ZWValueID.ValueGenre genre = ZWValueID.ValueGenre.Basic;
            byte commandClass = 2;
            byte instance = 3;
            byte valueIndex = 4;
            ZWValueID.ValueType type = ZWValueID.ValueType.Byte;
            byte pollIntensity = 5;
            ZWValueID id = new ZWValueID(homeId, nodeId, genre, commandClass, instance, valueIndex, type, pollIntensity);

            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            controller.Setup(s => s.SetValue(id, 0x25));
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();
            node.AddValueId(id);

            node.MockSetValue(valueIndex, commandClass, 0x25);

            controller.Verify(s => s.SetValue(id, 0x25), Times.Exactly(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZWaveNodeSetValueBoolNotFound()
        {
            Mock<IZWaveController> controller = new Mock<IZWaveController>(MockBehavior.Strict);
            ZWaveNode.SetController(controller.Object);
            Mock<IInternalConfigurationManager> configManager = new Mock<IInternalConfigurationManager>();
            MockZWaveNode node = new MockZWaveNode();

            node.MockSetValue(0, 1, true);
        }
    }
}
