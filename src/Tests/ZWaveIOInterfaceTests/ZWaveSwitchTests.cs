using System;
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
    public class ZWaveSwitchTests : BaseSAMITests
    {
        [TestMethod]
        public void ZWaveSwitchConstructor()
        {
            ZWaveSwitch zWaveSwitch = new ZWaveSwitch();

            Assert.AreEqual(SwitchType.Misc, zWaveSwitch.Type);
            Assert.AreEqual(2, zWaveSwitch.Properties.Count());
            PersistentProperty prop = zWaveSwitch.Properties.SingleOrDefault(s => s.Name.Equals("Name"));
            Assert.IsNotNull(prop);
        }

        [TestMethod]
        public void ZWaveSwitchNameSet()
        {
            ZWaveSwitch zWaveSwitch = new ZWaveSwitch();
            PersistentProperty prop = zWaveSwitch.Properties.SingleOrDefault(s => s.Name.Equals("Name"));
            Assert.IsNotNull(prop);

            prop.Setter("Test Name");

            Assert.AreEqual("Test Name", zWaveSwitch.Name);
        }

        [TestMethod]
        public void ZWaveSwitchTurnOn()
        {
            ZWValueID valueId = new ZWValueID(1, 3, ZWValueID.ValueGenre.User, 0x25, 10, 0, ZWValueID.ValueType.Bool, 2);
            Mock<IZWaveController> zWaveController = new Mock<IZWaveController>();
            zWaveController.Setup(s => s.SetValue(valueId, true));
            ZWaveNode.SetController(zWaveController.Object);
            ZWaveSwitch zWaveSwitch = new ZWaveSwitch();
            zWaveSwitch.AddValueId(valueId);

            zWaveSwitch.TurnOn();

            zWaveController.Verify(s => s.SetValue(valueId, true), Times.Exactly(1));
        }

        [TestMethod]
        public void ZWaveSwitchTurnOff()
        {
            ZWValueID valueId = new ZWValueID(1, 3, ZWValueID.ValueGenre.User, 0x25, 10, 0, ZWValueID.ValueType.Bool, 2);
            Mock<IZWaveController> zWaveController = new Mock<IZWaveController>();
            zWaveController.Setup(s => s.SetValue(valueId, false));
            ZWaveNode.SetController(zWaveController.Object);
            ZWaveSwitch zWaveSwitch = new ZWaveSwitch();
            zWaveSwitch.AddValueId(valueId);

            zWaveSwitch.TurnOff();

            zWaveController.Verify(s => s.SetValue(valueId, false), Times.Exactly(1));
        }
    }
}
