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
    public class ShakeStepTests : BaseSAMITests
    {
        [TestMethod]
        public void ShakeStepDoStepFirstStateTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepAllTrueAnswers(step, bartender.Object, 0, "Pour the drink into a shaker.", false);
        }

        [TestMethod]
        public void ShakeStepDoStepSecondStateTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepAllTrueAnswers(step, bartender.Object, 1, "Shake the drink.", false);
        }

        [TestMethod]
        public void ShakeStepDoStepThirdStateTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepAllTrueAnswers(step, bartender.Object, 2, "Pour the drink back into the glass.", false);
        }

        [TestMethod]
        public void ShakeStepDoStepLastStateTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepAllTrueAnswers(step, bartender.Object, 3, null, true);
        }

        [TestMethod]
        public void ShakeStepDoStepFalseFirstAnswerTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepFalseAnswer(step, bartender.Object, 0);
        }

        [TestMethod]
        public void ShakeStepDoStepFalseSecondAnswerTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepFalseAnswer(step, bartender.Object, 1);
        }

        [TestMethod]
        public void ShakeStepDoStepFalseThirdAnswerTest()
        {
            ShakeStep step = new ShakeStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            TestDoStepFalseAnswer(step, bartender.Object, 2);
        }

        private static void TestDoStepAllTrueAnswers(ShakeStep step, IBartenderController bartender, int numTrueAnswers, String expectedAnswer, bool shouldBeFinished)
        {
            Dictionary<String, String> trueInput = new Dictionary<string, string>
            {
                {"answer", "true"},
            };
            for (int i = 0; i < numTrueAnswers + 1; i++)
            {
                step.DoStep(new Dialog(trueInput, "Test Phrase"), bartender);
            }
            Assert.AreEqual(expectedAnswer, step.MessageToUser);
            Assert.AreEqual(shouldBeFinished, step.IsStepDone);
            Assert.AreEqual("Shake", step.Name);
            Assert.AreEqual(shouldBeFinished ? null : BartenderApp.ConfirmationRuleName, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        private static void TestDoStepFalseAnswer(ShakeStep step, IBartenderController bartender, int numTrueAnswers)
        {
            Dictionary<String, String> trueInput = new Dictionary<string, string>
            {
                {"answer", "true"},
            };
            Dictionary<String, String> falseInput = new Dictionary<string, string>
            {
                {"answer", "false"},
            };
            for (int i = 0; i < numTrueAnswers + 1; i++)
            {
                step.DoStep(new Dialog(trueInput, "Test Phrase"), bartender);
            }
            step.DoStep(new Dialog(falseInput, "Test Phrase"), bartender);
            Assert.AreEqual(null, step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Shake", step.Name);
            Assert.AreEqual(null, step.NextGrammarNeeded);
            Assert.AreEqual(true, step.ShouldCancel);
        }

        [TestMethod]
        public void ShakeStepCloneTest()
        {
            ShakeStep originalStep = new ShakeStep();
            ShakeStep clonedStep = originalStep.Clone() as ShakeStep;
            Assert.IsNotNull(clonedStep);
            Assert.AreEqual(originalStep.IsStepDone, clonedStep.IsStepDone);
            Assert.AreEqual(originalStep.MessageToUser, clonedStep.MessageToUser);
            Assert.AreEqual(originalStep.Name, clonedStep.Name);
            Assert.AreEqual(originalStep.NextGrammarNeeded, clonedStep.NextGrammarNeeded);
            Assert.AreEqual(originalStep.ShouldCancel, clonedStep.ShouldCancel);
        }
    }
}
