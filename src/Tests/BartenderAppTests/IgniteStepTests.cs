using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Bartender;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Test.Utilities;

namespace BartenderAppTests
{
    [TestClass]
    public class IgniteStepTests : BaseSAMITests
    {
        [TestMethod]
        public void IgniteStepDoStepNullInputTest()
        {
            IgniteStep step = new IgniteStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            step.DoStep(null, bartender.Object);
            Assert.AreEqual("Ignite the drink.", step.MessageToUser);
            Assert.AreEqual(false, step.IsStepDone);
            Assert.AreEqual("Ignite", step.Name);
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void IgniteStepDoStepInvalidUserInputTest()
        {
            IgniteStep step = new IgniteStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            step.DoStep(new Dialog(new Dictionary<String, String>(), "Test Phrase"), bartender.Object);
            Assert.AreEqual("Ignite the drink.", step.MessageToUser);
            Assert.AreEqual(false, step.IsStepDone);
            Assert.AreEqual("Ignite", step.Name);
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void IgniteStepDoStepTrueAnswerTest()
        {
            IgniteStep step = new IgniteStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"answer", "true"},
            };
            step.DoStep(new Dialog(input, "Test Phrase"), bartender.Object);
            Assert.AreEqual(null, step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Ignite", step.Name);
            Assert.AreEqual(null, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void IgniteStepDoStepFalseAnswerTest()
        {
            IgniteStep step = new IgniteStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"answer", "false"},
            };
            step.DoStep(new Dialog(input, "Test Phrase"), bartender.Object);
            Assert.AreEqual(null, step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Ignite", step.Name);
            Assert.AreEqual(null, step.NextGrammarNeeded);
            Assert.AreEqual(true, step.ShouldCancel);
        }

        [TestMethod]
        public void IgniteStepCloneTest()
        {
            IgniteStep originalStep = new IgniteStep();
            IgniteStep clonedStep = originalStep.Clone() as IgniteStep;
            Assert.IsNotNull(clonedStep);
            Assert.AreEqual(originalStep.IsStepDone, clonedStep.IsStepDone);
            Assert.AreEqual(originalStep.MessageToUser, clonedStep.MessageToUser);
            Assert.AreEqual(originalStep.Name, clonedStep.Name);
            Assert.AreEqual(originalStep.NextGrammarNeeded, clonedStep.NextGrammarNeeded);
            Assert.AreEqual(originalStep.ShouldCancel, clonedStep.ShouldCancel);
        }
    }
}
