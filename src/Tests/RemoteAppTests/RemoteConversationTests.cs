using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Remote;
using SAMI.Configuration;
using SAMI.IOInterfaces;
using SAMI.IOInterfaces.Interfaces.Power;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Remote;
using SAMI.Test.Utilities;

namespace RemoteAppTests
{
    [TestClass]
    public class RemoteConversationTests : BaseConversationTests
    {
        #region TV Tests
        [TestMethod]
        public void ShowInfo()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestTVInfoButtons("info", "None", r => r.Setup(s => s.ToggleInfo()).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.ToggleInfo(), Times.Exactly(1));
            });
        }

        [TestMethod]
        public void TurnToChannelNumeric()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestTVInfoButtons("channel", "123.4", r => r.Setup(s => s.SendChannel("123.4")).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.SendChannel("123.4"), Times.Exactly(1));
            });
        }

        [TestMethod]
        public void TurnToChannelNameSuccess()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestTVInfoButtons("channel", "NBC", r => r.Setup(s => s.TrySendChannel("NBC")).Returns(true).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.TrySendChannel("NBC"), Times.Exactly(1));
            });
        }

        [TestMethod]
        public void TurnToChannelNameFailure()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestTVInfoButtons("channel", "NBC", r => r.Setup(s => s.TrySendChannel("NBC")).Returns(false).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.TrySendChannel("NBC"), Times.Exactly(1));
            });
        }

        private void TestTVInfoButtons(String command, String channel, Action<Mock<ITVInfoRemote>> setup, Action<Mock<ITVInfoRemote>> verify)
        {
            Dictionary<String, String> inputVals = new Dictionary<string, string>
            {
                {"Command", "remote"},
                {"remoteButton", command},
                {"channel", channel},
            };
            Mock<ITVInfoRemote> remote = SetupRemote<ITVInfoRemote>("Channel Remote");
            setup(remote);
            SetupRemote<IVolumeController>("Volume Remote");
            IOInterfaceReference ref2 = new IOInterfaceReference();
            ref2.Properties.Single(r => r.Name.Equals("References")).Setter("Channel Remote");
            ref2.Properties.Single(r => r.Name.Equals("Tag")).Setter("Channel");
            IOInterfaceReference ref3 = new IOInterfaceReference();
            ref3.Properties.Single(r => r.Name.Equals("References")).Setter("Volume Remote");
            List<IOInterfaceReference> refs = new List<IOInterfaceReference>
            {
                ref2,
                ref3,
            };
            CurrentConversation = new RemoteConversation(GetConfigurationManager(), refs);

            Assert.AreEqual("OK", RunSingleConversation<RemoteConversation>(inputVals));

            verify(remote);
        }
        #endregion

        #region DVR Tests
        [TestMethod]
        public void Play()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestDVRButtons("play", r => r.Setup(s => s.Play()).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.Play(), Times.Exactly(1));
            });
        }

        [TestMethod]
        public void Record()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestDVRButtons("record", r => r.Setup(s => s.Record()).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.Record(), Times.Exactly(1));
            });
        }

        [TestMethod]
        public void Pause()
        {
            Semaphore buttonLock = new Semaphore(0, 1);
            TestDVRButtons("pause", r => r.Setup(s => s.Pause()).Callback(() => buttonLock.Release()), s =>
            {
                buttonLock.WaitOne();
                s.Verify(r => r.Pause(), Times.Exactly(1));
            });
        }

        private void TestDVRButtons(String command, Action<Mock<IDVRRemote>> setup, Action<Mock<IDVRRemote>> verify)
        {
            Dictionary<String, String> inputVals = new Dictionary<string, string>
            {
                {"Command", "remote"},
                {"remoteButton", command},
                {"channel", "None"},
            };
            SetupRemote<ITVRemote>("Power Remote");
            Mock<IDVRRemote> remote = SetupRemote<IDVRRemote>("Channel Remote");
            setup(remote);
            SetupRemote<IVolumeController>("Volume Remote");
            IOInterfaceReference ref1 = new IOInterfaceReference();
            ref1.Properties.Single(r => r.Name.Equals("References")).Setter("Power Remote");
            IOInterfaceReference ref2 = new IOInterfaceReference();
            ref2.Properties.Single(r => r.Name.Equals("References")).Setter("Channel Remote");
            IOInterfaceReference ref3 = new IOInterfaceReference();
            ref3.Properties.Single(r => r.Name.Equals("References")).Setter("Volume Remote");
            List<IOInterfaceReference> refs = new List<IOInterfaceReference>
            {
                ref1,
                ref2,
                ref3,
            };
            CurrentConversation = new RemoteConversation(GetConfigurationManager(), refs);

            Assert.AreEqual("OK", RunSingleConversation<RemoteConversation>(inputVals));

            verify(remote);
        }
        #endregion

        #region Power Tests
        [TestMethod]
        public void PowerUpWithChannel()
        {
            Semaphore powerLock = new Semaphore(0, 1);
            Semaphore channelLock = new Semaphore(0, 1);
            Dictionary<String, String> inputVals = new Dictionary<string, string>
            {
                {"Command", "remote"},
                {"remoteButton", "powerup"},
                {"channel", "Channel"},
            };

            Mock<IPowerController> powerRemote = new Mock<IPowerController>();
            Mock<ITVRemote> tvRemote = powerRemote.As<ITVRemote>();
            powerRemote.Setup(s => s.IsValid).Returns(true);
            powerRemote.Setup(s => s.Name).Returns("Power Remote");
            AddComponentToConfigurationManager(powerRemote.Object);
            powerRemote.Setup(s => s.TurnOn()).Callback(() => powerLock.Release());
            tvRemote.Setup(s => s.TrySendChannel("Channel")).Returns(true).Callback(() => channelLock.Release());
            SetupRemote<IDVRRemote>("Channel Remote");
            SetupRemote<IVolumeController>("Volume Remote");
            IOInterfaceReference ref1 = new IOInterfaceReference();
            ref1.Properties.Single(r => r.Name.Equals("References")).Setter("Power Remote");
            ref1.Properties.Single(r => r.Name.Equals("Tag")).Setter("Power");
            IOInterfaceReference ref2 = new IOInterfaceReference();
            ref2.Properties.Single(r => r.Name.Equals("References")).Setter("Channel Remote");
            IOInterfaceReference ref3 = new IOInterfaceReference();
            ref3.Properties.Single(r => r.Name.Equals("References")).Setter("Volume Remote");
            List<IOInterfaceReference> refs = new List<IOInterfaceReference>
            {
                ref1,
                ref2,
                ref3,
            };
            CurrentConversation = new RemoteConversation(GetConfigurationManager(), refs);

            Assert.AreEqual("OK", RunSingleConversation<RemoteConversation>(inputVals));

            powerLock.WaitOne();
            powerRemote.Verify(s => s.TurnOn(), Times.Exactly(1));
            channelLock.WaitOne();
            tvRemote.Verify(s => s.TrySendChannel("Channel"), Times.Exactly(1));
        }
        #endregion

        private Mock<T> SetupRemote<T>(String Name) where T : class, IIOInterface
        {
            Mock<T> remote = new Mock<T>();
            remote.Setup(s => s.IsValid).Returns(true);
            remote.Setup(s => s.Name).Returns(Name);
            AddComponentToConfigurationManager(remote.Object);
            return remote;
        }
    }
}
