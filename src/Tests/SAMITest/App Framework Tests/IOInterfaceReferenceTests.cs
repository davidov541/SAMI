using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces;
using SAMI.Test.Utilities;

namespace SAMI.Test.App_Framework_Tests
{
    [TestClass]
    public class IOInterfaceReferenceTests : BaseSAMITests
    {
        [TestMethod]
        public void FullConstructor()
        {
            IOInterfaceReference reference = new IOInterfaceReference("DNE Tag", "DNE IO Interface", GetConfigurationManager());

            Assert.AreEqual("DNE Tag", reference.Tag);
            Assert.AreEqual("DNE IO Interface", reference.Name);
        }

        [TestMethod]
        public void PropertiesCorrectlySet()
        {
            IOInterfaceReference reference = new IOInterfaceReference();

            reference.Properties.Single(s => s.Name.Equals("References")).Setter("DNE IO Interface");
            reference.Properties.Single(s => s.Name.Equals("Tag")).Setter("DNE Tag");

            Assert.AreEqual("DNE Tag", reference.Tag);
            Assert.AreEqual("DNE IO Interface", reference.Name);
        }

        [TestMethod]
        public void IsInvalidWithIncorrectName()
        {
            Mock<IIOInterface> ioInterface = new Mock<IIOInterface>();
            ioInterface.Setup(s => s.IsValid).Returns(true);
            ioInterface.Setup(s => s.Name).Returns("Fake IO Interface");
            AddComponentToConfigurationManager(ioInterface.Object);
            IOInterfaceReference reference = new IOInterfaceReference();

            reference.Properties.Single(s => s.Name.Equals("References")).Setter("DNE IO Interface");

            Assert.IsFalse(reference.IsValid);
        }

        [TestMethod]
        public void IsValidWithCorrectName()
        {
            Mock<IIOInterface> ioInterface = new Mock<IIOInterface>();
            ioInterface.Setup(s => s.IsValid).Returns(true);
            ioInterface.Setup(s => s.Name).Returns("Fake IO Interface");
            AddComponentToConfigurationManager(ioInterface.Object);
            IOInterfaceReference reference = new IOInterfaceReference();
            reference.Initialize(GetConfigurationManager());

            reference.Properties.Single(s => s.Name.Equals("References")).Setter("Fake IO Interface");

            Assert.IsTrue(reference.IsValid);
        }
    }
}
