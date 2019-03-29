using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Volume;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Test.Utilities;

namespace VolumeAppTests
{
    [TestClass]
    public class VolumeAppTests : BaseSAMITests
    {
        [TestMethod]
        public void AppIsInvalidWithNoControllers()
        {
            VolumeApp app = new VolumeApp();
            Assert.IsFalse(app.IsValid);
        }

        [TestMethod]
        public void AppIsValidWithControllers()
        {
            Mock<IVolumeController> volumeController = new Mock<IVolumeController>();
            volumeController.Setup(s => s.IsValid).Returns(true);
            volumeController.Setup(s => s.Name).Returns("Volume Controller");
            AddComponentToConfigurationManager(volumeController.Object);
            VolumeApp app = new VolumeApp();
            app.Initialize(GetConfigurationManager());

            Assert.IsTrue(app.IsValid);
        }

        [TestMethod]
        public void AppAddsReferencesIfNone()
        {
            Mock<IVolumeController> volumeController = new Mock<IVolumeController>();
            volumeController.Setup(s => s.IsValid).Returns(true);
            volumeController.Setup(s => s.Name).Returns("Volume Controller");
            AddComponentToConfigurationManager(volumeController.Object);
            Mock<IVolumeController> volumeController2 = new Mock<IVolumeController>();
            volumeController2.Setup(s => s.IsValid).Returns(true);
            volumeController2.Setup(s => s.Name).Returns("Volume Controller Two");
            AddComponentToConfigurationManager(volumeController2.Object);
            VolumeApp app = new VolumeApp();

            app.Initialize(GetConfigurationManager());

            Assert.AreEqual(2, app.Children.OfType<IOInterfaceReference>().Count());
            Assert.AreEqual("Volume Controller", app.Children.OfType<IOInterfaceReference>().First().Name);
            Assert.AreEqual("", app.Children.OfType<IOInterfaceReference>().First().Tag);
            Assert.AreEqual("Volume Controller Two", app.Children.OfType<IOInterfaceReference>().Last().Name);
            Assert.AreEqual("", app.Children.OfType<IOInterfaceReference>().Last().Tag);
        }

        [TestMethod]
        public void AppDoesNotAddReferences()
        {
            Mock<IVolumeController> volumeController = new Mock<IVolumeController>();
            volumeController.Setup(s => s.IsValid).Returns(true);
            volumeController.Setup(s => s.Name).Returns("Volume Controller");
            AddComponentToConfigurationManager(volumeController.Object);
            Mock<IVolumeController> volumeController2 = new Mock<IVolumeController>();
            volumeController2.Setup(s => s.IsValid).Returns(true);
            volumeController2.Setup(s => s.Name).Returns("Volume Controller Two");
            AddComponentToConfigurationManager(volumeController2.Object);
            VolumeApp app = new VolumeApp();
            app.AddChild(new IOInterfaceReference("", "Volume Controller", GetConfigurationManager()));

            app.Initialize(GetConfigurationManager());

            Assert.AreEqual(1, app.Children.OfType<IOInterfaceReference>().Count());
            Assert.AreEqual("Volume Controller", app.Children.OfType<IOInterfaceReference>().First().Name);
            Assert.AreEqual("", app.Children.OfType<IOInterfaceReference>().First().Tag);
        }
    }
}
