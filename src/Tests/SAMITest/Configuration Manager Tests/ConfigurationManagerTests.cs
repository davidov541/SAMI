using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces;
using SAMI.Persistence;

namespace SAMI.Test.Infrastructure
{
    [TestClass]
    public class ConfigurationManagerTests
    {
        #region FindAllComponentsOfType Tests
        [TestMethod]
        public void TestConfigurationManagerFindAllComponentsOfTypeSingleApp()
        {
            ConfigurationManager manager = new ConfigurationManager();

            Mock<IApp> app = new Mock<IApp>(MockBehavior.Strict);
            app.Setup(s => s.IsValid).Returns(true);
            manager.AddComponent(app.Object);

            Mock<IIOInterface> ioInterface = new Mock<IIOInterface>(MockBehavior.Strict);
            manager.AddComponent(ioInterface.Object);

            IEnumerable<IParseable> foundComponents = manager.FindAllComponentsOfType<IApp>().ToList();
            Assert.AreEqual(1, foundComponents.Count());
            Assert.AreEqual(app.Object, foundComponents.First());

            app.Verify(s => s.IsValid, Times.Exactly(1));
        }

        [TestMethod]
        public void TestConfigurationManagerFindAllComponentsOfTypeMultipleApps()
        {
            ConfigurationManager manager = new ConfigurationManager();

            Mock<IApp> app1 = new Mock<IApp>(MockBehavior.Strict);
            app1.Setup(s => s.IsValid).Returns(true);
            manager.AddComponent(app1.Object);

            Mock<IApp> app2 = new Mock<IApp>(MockBehavior.Strict);
            app2.Setup(s => s.IsValid).Returns(true);
            manager.AddComponent(app2.Object);

            Mock<IIOInterface> ioInterface = new Mock<IIOInterface>(MockBehavior.Strict);
            manager.AddComponent(ioInterface.Object);

            IEnumerable<IParseable> foundComponents = manager.FindAllComponentsOfType<IApp>().ToList();
            Assert.AreEqual(2, foundComponents.Count());
            Assert.IsTrue(foundComponents.Contains(app1.Object));
            Assert.IsTrue(foundComponents.Contains(app2.Object));

            app1.Verify(s => s.IsValid, Times.Exactly(1));
            app2.Verify(s => s.IsValid, Times.Exactly(1));
        }

        [TestMethod]
        public void TestConfigurationManagerFindAllComponentsOfTypeInvalidApp()
        {
            ConfigurationManager manager = new ConfigurationManager();

            Mock<IApp> app1 = new Mock<IApp>(MockBehavior.Strict);
            app1.Setup(s => s.IsValid).Returns(false);
            manager.AddComponent(app1.Object);

            Mock<IApp> app2 = new Mock<IApp>(MockBehavior.Strict);
            app2.Setup(s => s.IsValid).Returns(false);
            manager.AddComponent(app2.Object);

            Mock<IIOInterface> ioInterface = new Mock<IIOInterface>(MockBehavior.Strict);
            manager.AddComponent(ioInterface.Object);

            IEnumerable<IParseable> foundComponents = manager.FindAllComponentsOfType<IApp>().ToList();
            Assert.AreEqual(0, foundComponents.Count());

            app1.Verify(s => s.IsValid, Times.Exactly(1));
            app2.Verify(s => s.IsValid, Times.Exactly(1));
        }

        [TestMethod]
        public void TestConfigurationManagerFindAllComponentsOfTypeSingleIOInterface()
        {
            ConfigurationManager manager = new ConfigurationManager();

            Mock<IApp> app = new Mock<IApp>(MockBehavior.Strict);
            manager.AddComponent(app.Object);

            Mock<IIOInterface> ioInterface = new Mock<IIOInterface>(MockBehavior.Strict);
            ioInterface.Setup(s => s.IsValid).Returns(true);
            manager.AddComponent(ioInterface.Object);

            IEnumerable<IParseable> foundComponents = manager.FindAllComponentsOfType<IIOInterface>().ToList();
            Assert.AreEqual(1, foundComponents.Count());
            Assert.AreEqual(ioInterface.Object, foundComponents.First());

            ioInterface.Verify(s => s.IsValid, Times.Exactly(1));
        }

        [TestMethod]
        public void TestConfigurationManagerFindAllComponentsOfTypeMultipleIOInterfaces()
        {
            ConfigurationManager manager = new ConfigurationManager();

            Mock<IApp> app = new Mock<IApp>(MockBehavior.Strict);
            manager.AddComponent(app.Object);

            Mock<IIOInterface> ioInterface1 = new Mock<IIOInterface>(MockBehavior.Strict);
            ioInterface1.Setup(s => s.IsValid).Returns(true);
            manager.AddComponent(ioInterface1.Object);

            Mock<IIOInterface> ioInterface2 = new Mock<IIOInterface>(MockBehavior.Strict);
            ioInterface2.Setup(s => s.IsValid).Returns(true);
            manager.AddComponent(ioInterface2.Object);

            IEnumerable<IParseable> foundComponents = manager.FindAllComponentsOfType<IIOInterface>().ToList();
            Assert.AreEqual(2, foundComponents.Count());
            Assert.IsTrue(foundComponents.Contains(ioInterface1.Object));
            Assert.IsTrue(foundComponents.Contains(ioInterface2.Object));

            ioInterface1.Verify(s => s.IsValid, Times.Exactly(1));
            ioInterface2.Verify(s => s.IsValid, Times.Exactly(1));
        }

        [TestMethod]
        public void TestConfigurationManagerFindAllComponentsOfTypeInvalidIOInterfaces()
        {
            ConfigurationManager manager = new ConfigurationManager();

            Mock<IApp> app = new Mock<IApp>(MockBehavior.Strict);
            manager.AddComponent(app.Object);

            Mock<IIOInterface> ioInterface1 = new Mock<IIOInterface>(MockBehavior.Strict);
            ioInterface1.Setup(s => s.IsValid).Returns(false);
            manager.AddComponent(ioInterface1.Object);

            Mock<IIOInterface> ioInterface2 = new Mock<IIOInterface>(MockBehavior.Strict);
            ioInterface2.Setup(s => s.IsValid).Returns(false);
            manager.AddComponent(ioInterface2.Object);

            IEnumerable<IParseable> foundComponents = manager.FindAllComponentsOfType<IIOInterface>().ToList();
            Assert.AreEqual(0, foundComponents.Count());

            ioInterface1.Verify(s => s.IsValid, Times.Exactly(1));
            ioInterface2.Verify(s => s.IsValid, Times.Exactly(1));
        }
        #endregion

        #region Configuration File Parsing Tests
        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorLocationMetadata()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            Assert.AreEqual(new Location("Austin", "TX", 78759), manager.LocalLocation);

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorXBeeCOMMetadata()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            Assert.AreEqual("COM4", manager.XBeeCOM);

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorApp()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(3, foundComponents.OfType<MockApp>().Count());
            Assert.AreEqual(1, foundComponents.OfType<MockApp>().Count(c => c.Children.Any()));

            MockApp app = foundComponents.OfType<MockApp>().Single(c => c.Children.Any());
            Assert.AreEqual(2, app.Children.Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorAppAttributes()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            IEnumerable<MockApp> apps = foundComponents.OfType<MockApp>();

            Assert.AreEqual(3, apps.Count(a => !String.IsNullOrEmpty(a.TestProp)));
            Assert.AreEqual(1, apps.Count(a => a.TestProp.Equals("Test")));
            Assert.AreEqual(1, apps.Count(a => a.TestProp.Equals("Test1")));
            Assert.AreEqual(1, apps.Count(a => a.TestProp.Equals("Test2")));

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\IncludedMockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationIncludeTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorAppIncludeTagAtTopLevel()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationIncludeTag.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(3, foundComponents.OfType<MockApp>().Count());
            Assert.AreEqual(1, foundComponents.OfType<MockApp>().Count(c => c.Children.Any()));

            MockApp app = foundComponents.OfType<MockApp>().Single(c => c.Children.Any());
            Assert.AreEqual(2, app.Children.Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\NestedIncludedMockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationNestedIncludeTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorAppNestedIncludeTagAtTopLevel()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationNestedIncludeTag.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(6, foundComponents.Count());

            Assert.AreEqual(5, foundComponents.OfType<MockApp>().Count());
            Assert.AreEqual(1, foundComponents.OfType<MockApp>().Count(c => c.Children.Any()));

            MockApp app = foundComponents.OfType<MockApp>().Single(c => c.Children.Any());
            Assert.AreEqual(2, app.Children.Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorIOInterface()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(2, foundComponents.OfType<IIOInterface>().Count());
            Assert.AreEqual(1, foundComponents.OfType<IIOInterface>().Count(c => c.Children.Any()));

            IIOInterface ioInterface = foundComponents.OfType<IIOInterface>().Single(c => c.Children.Any());
            Assert.AreEqual(1, ioInterface.Children.Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorIOInterfaceAttributes()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            IEnumerable<MockIOInterface> interfaces = foundComponents.OfType<MockIOInterface>();

            Assert.AreEqual(2, interfaces.Count(i => !String.IsNullOrEmpty(i.TestProp)));
            Assert.AreEqual(1, interfaces.Count(i => i.TestProp.Equals("Test5")));
            Assert.AreEqual(1, interfaces.Count(i => i.TestProp.Equals("Test6")));

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorSupportAttributes()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfiguration.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            IEnumerable<MockSupport> supportNodes = foundComponents.Where(a => a.Children.Any()).SelectMany(a => a.Children).OfType<MockSupport>();

            Assert.AreEqual(3, supportNodes.Count(s => !String.IsNullOrEmpty(s.TestProp)));
            Assert.IsTrue(supportNodes.All(s => !String.IsNullOrEmpty(s.TestProp)));
            Assert.AreEqual(1, supportNodes.Count(s => s.TestProp.Equals("Test3")));
            Assert.AreEqual(1, supportNodes.Count(s => s.TestProp.Equals("Test4")));
            Assert.AreEqual(1, supportNodes.Count(s => s.TestProp.Equals("Test7")));

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\IncludedMockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationIncludeTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorIOInterfaceIncludeTagAtLowerLevel()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationIncludeTag.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(2, foundComponents.OfType<IIOInterface>().Count());
            Assert.AreEqual(1, foundComponents.OfType<IIOInterface>().Count(c => c.Children.Any()));

            IIOInterface ioInterface = foundComponents.OfType<IIOInterface>().Single(c => c.Children.Any());
            Assert.AreEqual(1, ioInterface.Children.Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationComponentsTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorIOInterfaceComponentsTag()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationComponentsTag.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(2, foundComponents.OfType<IIOInterface>().Count());
            Assert.AreEqual(1, foundComponents.OfType<IIOInterface>().Count(c => c.Children.Any()));

            IIOInterface ioInterface = foundComponents.OfType<IIOInterface>().Single(c => c.Children.Any());
            Assert.AreEqual(1, ioInterface.Children.Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationNestedComponentsTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorAppNestedComponentsTag()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationNestedComponentsTag.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(3, foundComponents.OfType<MockApp>().Count());
            Assert.AreEqual(1, foundComponents.OfType<MockApp>().Count(c => c.Children.Any()));

            MockApp app = foundComponents.OfType<MockApp>().Single(c => c.Children.Any());
            Assert.AreEqual(2, app.Children.Count());
            Assert.AreEqual(1, app.Children.Count(c => c.Children.Any()));

            MockSupport support = app.Children.First(c => c.Children.Any()) as MockSupport;
            Assert.IsNotNull(support);
            Assert.AreEqual(1, support.Children.Count());
            Assert.AreEqual(1, support.Children.OfType<MockSupport>().Count());

            manager.Dispose();
        }

        [TestMethod]
        public void TestConfigurationManagerConstructorIOInterfaceNestedComponentsTag()
        {
            ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationNestedComponentsTag.xml");

            IEnumerable<IParseable> foundComponents = manager.Components;
            Assert.AreEqual(5, foundComponents.Count());

            Assert.AreEqual(2, foundComponents.OfType<IIOInterface>().Count());
            Assert.AreEqual(1, foundComponents.OfType<IIOInterface>().Count(c => c.Children.Any()));

            IIOInterface ioInterface = foundComponents.OfType<IIOInterface>().Single(c => c.Children.Any());
            Assert.AreEqual(1, ioInterface.Children.Count());

            MockSupport support = ioInterface.Children.First() as MockSupport;
            Assert.IsNotNull(support);
            Assert.AreEqual(1, support.Children.Count());
            Assert.AreEqual(1, support.Children.OfType<MockSupport>().Count());

            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationInvalidNestedAppTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorInvalidNestedAppTag()
        {
            InvalidOperationException exp = null;
            try
            {
                ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationInvalidNestedAppTag.xml");
            }
            catch (InvalidOperationException e)
            {
                exp = e;
            }
            Assert.IsNotNull(exp);
            Assert.AreEqual("There was an error while processing a declaration for the app MockApp. Ensure that MockApp is declared at the top level of the Apps tag in the configuration file.", exp.Message);
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationInvalidTopLevelSupportTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorInvalidTopLevelSupportTag()
        {
            InvalidOperationException exp = null;
            try
            {
                ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationInvalidTopLevelSupportTag.xml");
            }
            catch (InvalidOperationException e)
            {
                exp = e;
            }
            Assert.IsNotNull(exp);
            Assert.AreEqual("There was an error while processing a declaration for the support component MockSupport. Ensure that MockSupport is declared underneath an app or an IO interface.", exp.Message);
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationInvalidNestedIOInterfaceTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorInvalidNestedIOInterfaceTag()
        {
            InvalidOperationException exp = null;
            try
            {
                ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationInvalidNestedIOInterfaceTag.xml");
            }
            catch (InvalidOperationException e)
            {
                exp = e;
            }
            Assert.IsNotNull(exp);
            Assert.AreEqual("There was an error while processing a declaration for the IO interface MockIOInterface. Ensure that MockIOInterface is declared at the top level of the IO Interfaces tag in the configuration file.", exp.Message);
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationInvalidAppTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorInvalidAppNameTag()
        {
            InvalidOperationException exp = null;
            try
            {
                ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationInvalidAppTag.xml");
            }
            catch (InvalidOperationException e)
            {
                exp = e;
            }
            Assert.IsNotNull(exp);
            Assert.AreEqual("Could not find any components with the name MockApp2.", exp.Message);
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationInvalidIncorrectlyPlacedAppTag.xml")]
        [TestMethod]
        public void TestConfigurationManagerConstructorInvalidIncorrectlyPlacedAppTag()
        {
            InvalidOperationException exp = null;
            try
            {
                ConfigurationManager manager = new ConfigurationManager(@"MockConfigurationInvalidIncorrectlyPlacedAppTag.xml");
            }
            catch (InvalidOperationException e)
            {
                exp = e;
            }
            Assert.IsNotNull(exp);
            Assert.AreEqual("There was an error while processing a declaration for the app MockApp. Ensure that MockApp is declared at the top level of the Apps tag in the configuration file.", exp.Message);
        }
        #endregion

        #region Save Configuration Tests
        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationSavedNoChanges.xml")]
        [TestMethod]
        public void TestConfigurationManagerSaveConfigurationNoChanges()
        {
            String newFilePath = "MockConfigurationTestConfigurationManagerSaveConfigurationNoChanges.xml";
            File.Copy("MockConfiguration.xml", newFilePath);
            ConfigurationManager manager = new ConfigurationManager(newFilePath);

            manager.SaveConfiguration();

            String inputFile = File.ReadAllText("MockConfigurationSavedNoChanges.xml").Replace(" ", "").Replace("\t", "");
            String outputFile = File.ReadAllText(newFilePath).Replace(" ", "").Replace("\t", "");
            Assert.AreEqual(inputFile, outputFile);
            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationSavedAttributeChanges.xml")]
        [TestMethod]
        public void TestConfigurationManagerSaveConfigurationAttributeChanges()
        {
            String newFilePath = "MockConfigurationTestConfigurationManagerSaveConfigurationAttributeChanges.xml";
            File.Copy("MockConfiguration.xml", newFilePath);
            ConfigurationManager manager = new ConfigurationManager(newFilePath);
            MockApp app = manager.FindAllComponentsOfType<MockApp>().Single(a => a.TestProp.Equals("Test"));
            app.TestProp = "ChangedAttribute";

            manager.SaveConfiguration();

            String inputFile = File.ReadAllText("MockConfigurationSavedAttributeChanges.xml").Replace(" ", "").Replace("\t", "");
            String outputFile = File.ReadAllText(newFilePath).Replace(" ", "").Replace("\t", "");
            Assert.AreEqual(inputFile, outputFile);
            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationSavedNodeAddition.xml")]
        [TestMethod]
        public void TestConfigurationManagerSaveConfigurationNodeAddition()
        {
            String newFilePath = "MockConfigurationTestConfigurationManagerSaveConfigurationNodeAddition.xml";
            File.Copy("MockConfiguration.xml", newFilePath);
            ConfigurationManager manager = new ConfigurationManager(newFilePath);
            MockApp app = manager.FindAllComponentsOfType<MockApp>().Single(a => a.TestProp.Equals("Test2"));
            MockSupport support = new MockSupport();
            support.TestProp = "AddedNode";
            app.AddChild(support);

            manager.SaveConfiguration();

            String inputFile = File.ReadAllText("MockConfigurationSavedNodeAddition.xml").Replace(" ", "").Replace("\t", "");
            String outputFile = File.ReadAllText(newFilePath).Replace(" ", "").Replace("\t", "");
            Assert.AreEqual(inputFile, outputFile);
            manager.Dispose();
        }

        [DeploymentItem(@"Configuration Manager Tests\MockConfiguration.xml")]
        [DeploymentItem(@"Configuration Manager Tests\MockConfigurationSavedNodeRemoval.xml")]
        [TestMethod]
        public void TestConfigurationManagerSaveConfigurationNodeRemoval()
        {
            String newFilePath = "MockConfigurationTestConfigurationManagerSaveConfigurationNodeRemoval.xml";
            File.Copy("MockConfiguration.xml", newFilePath);
            ConfigurationManager manager = new ConfigurationManager(newFilePath);
            MockApp app = manager.FindAllComponentsOfType<MockApp>().Single(a => a.TestProp.Equals("Test2"));
            app.RemoveChild("Test4");

            manager.SaveConfiguration();

            String inputFile = File.ReadAllText("MockConfigurationSavedNodeRemoval.xml").Replace(" ", "").Replace("\t", "");
            String outputFile = File.ReadAllText(newFilePath).Replace(" ", "").Replace("\t", "");
            Assert.AreEqual(inputFile, outputFile);
            manager.Dispose();
        }
        #endregion

        #region Helper Function Tests
        [TestMethod]
        public void TestConfigurationManagerGetPathForFile()
        {
            ConfigurationManager manager = new ConfigurationManager();
            String path = manager.GetPathForFile("TestFile", GetType());
            Assert.IsTrue(path.EndsWith("TestFile"));
        }
        #endregion
    }
}
