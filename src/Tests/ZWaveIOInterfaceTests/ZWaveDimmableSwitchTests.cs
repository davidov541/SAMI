using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenZWaveDotNet;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Persistence;
using SAMI.Test.Utilities;
using ZWave.Switch;

namespace ZWaveIOInterfaceTests
{
    [TestClass]
    public class ZWaveDimmableSwitchTests : BaseSAMITests
    {
        [TestMethod]
        public void ZWaveDimmableSwitchConstructor()
        {
            ZWaveDimmableSwitch zWaveSwitch = new ZWaveDimmableSwitch();

            Assert.AreEqual(SwitchType.Misc, zWaveSwitch.Type);
            Assert.AreEqual(2, zWaveSwitch.Properties.Count());
            PersistentProperty prop = zWaveSwitch.Properties.SingleOrDefault(s => s.Name.Equals("Name"));
            Assert.IsNotNull(prop);
        }

        [TestMethod]
        public void ZWaveDimmableSwitchNameSet()
        {
            ZWaveDimmableSwitch zWaveSwitch = new ZWaveDimmableSwitch();
            PersistentProperty prop = zWaveSwitch.Properties.SingleOrDefault(s => s.Name.Equals("Name"));
            Assert.IsNotNull(prop);

            prop.Setter("Test Name");

            Assert.AreEqual("Test Name", zWaveSwitch.Name);
        }

        [TestMethod]
        public void ZWaveDimmableSwitchTurnOn()
        {
            ZWValueID valueId = new ZWValueID(1, 3, ZWValueID.ValueGenre.User, 0x26, 10, 0, ZWValueID.ValueType.Byte, 2);
            Mock<IZWaveController> zWaveController = new Mock<IZWaveController>();
            zWaveController.Setup(s => s.SetValue(valueId, 255));
            ZWaveNode.SetController(zWaveController.Object);
            ZWaveDimmableSwitch zWaveSwitch = new ZWaveDimmableSwitch();
            zWaveSwitch.AddValueId(valueId);

            zWaveSwitch.TurnOn();

            zWaveController.Verify(s => s.SetValue(valueId, 255), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveDimmableSwitchTurnOff()
        {
            ZWValueID valueId = new ZWValueID(1, 3, ZWValueID.ValueGenre.User, 0x26, 10, 0, ZWValueID.ValueType.Byte, 2);
            Mock<IZWaveController> zWaveController = new Mock<IZWaveController>();
            zWaveController.Setup(s => s.SetValue(valueId, 0));
            ZWaveNode.SetController(zWaveController.Object);
            ZWaveDimmableSwitch zWaveSwitch = new ZWaveDimmableSwitch();
            zWaveSwitch.AddValueId(valueId);

            zWaveSwitch.TurnOff();

            zWaveController.Verify(s => s.SetValue(valueId, 0), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveDimmableSwitchSetLevel()
        {
            ZWValueID valueId = new ZWValueID(1, 3, ZWValueID.ValueGenre.User, 0x26, 10, 0, ZWValueID.ValueType.Byte, 2);
            Mock<IZWaveController> zWaveController = new Mock<IZWaveController>(MockBehavior.Strict);
            zWaveController.Setup(s => s.SetValue(valueId, 63));
            ZWaveNode.SetController(zWaveController.Object);
            ZWaveDimmableSwitch zWaveSwitch = new ZWaveDimmableSwitch();
            zWaveSwitch.AddValueId(valueId);

            zWaveSwitch.SetLightLevel(0.25);

            zWaveController.Verify(s => s.SetValue(valueId, 63), Times.Exactly(1));
        }
    }
}
