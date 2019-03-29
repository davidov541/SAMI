using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Remote;
using SAMI.Persistence;
using SAMI.Test.Utilities;

namespace RemoteAppTests
{
    [TestClass]
    public class RemoteAppTests : BaseSAMITests
    {
        [TestMethod]
        public void ReferencesAppearInProperty()
        {
            RemoteApp app = new RemoteApp();
            IOInterfaceReference ref1 = new IOInterfaceReference();
            app.AddChild(ref1);
            IOInterfaceReference ref2 = new IOInterfaceReference();
            app.AddChild(ref2);
            Mock<IParseable> notARef = new Mock<IParseable>(MockBehavior.Strict);
            notARef.Setup(s => s.IsValid).Returns(true);

            IEnumerable<IOInterfaceReference> references = app.References;

            Assert.AreEqual(2, references.Count());
            Assert.IsTrue(references.Contains(ref1));
            Assert.IsTrue(references.Contains(ref2));
        }

        [TestMethod]
        public void IsInvalidNoReferences()
        {
            RemoteApp app = new RemoteApp();

            Assert.IsFalse(app.IsValid);
            Assert.AreEqual("No remote controls are connected to Sammie. Please make sure that your remote is plugged into Sammie.", app.InvalidMessage);
        }

        [TestMethod]
        public void IsValid()
        {
            RemoteApp app = new RemoteApp();
            IOInterfaceReference ref1 = new IOInterfaceReference();
            app.AddChild(ref1);

            Assert.IsTrue(app.IsValid);
            Assert.AreEqual(String.Empty, app.InvalidMessage);
        }

        [TestMethod]
        public void AddChannelRemoteReference()
        {
            RemoteApp app = new RemoteApp();
            ChannelRemoteReference ref1 = new ChannelRemoteReference();
            ref1.Properties.Single(r => r.Name.Equals("RemoteName")).Setter("Test Remote");
            app.AddChild(ref1);

            Assert.AreEqual(1, app.Children.Count());
            Assert.IsInstanceOfType(app.Children.First(), typeof(IOInterfaceReference));
            Assert.AreEqual("Channel", (app.Children.First() as IOInterfaceReference).Tag);
            Assert.AreEqual("Test Remote", (app.Children.First() as IOInterfaceReference).Name);
        }

        [TestMethod]
        public void AddVolumeRemoteReference()
        {
            RemoteApp app = new RemoteApp();
            VolumeRemoteReference ref1 = new VolumeRemoteReference();
            ref1.Properties.Single(r => r.Name.Equals("RemoteName")).Setter("Test Remote");
            app.AddChild(ref1);

            Assert.AreEqual(1, app.Children.Count());
            Assert.IsInstanceOfType(app.Children.First(), typeof(IOInterfaceReference));
            Assert.AreEqual("Volume", (app.Children.First() as IOInterfaceReference).Tag);
            Assert.AreEqual("Test Remote", (app.Children.First() as IOInterfaceReference).Name);
        }

        [TestMethod]
        public void AddPowerRemoteReference()
        {
            RemoteApp app = new RemoteApp();
            PowerRemoteReference ref1 = new PowerRemoteReference();
            ref1.Properties.Single(r => r.Name.Equals("RemoteName")).Setter("Test Remote");
            app.AddChild(ref1);

            Assert.AreEqual(1, app.Children.Count());
            Assert.IsInstanceOfType(app.Children.First(), typeof(IOInterfaceReference));
            Assert.AreEqual("Power", (app.Children.First() as IOInterfaceReference).Tag);
            Assert.AreEqual("Test Remote", (app.Children.First() as IOInterfaceReference).Name);
        }
    }
}
