using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Bartender;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.Test.Utilities;

namespace BartenderAppTests
{
    [TestClass]
    public class GarnishStepTests : BaseSAMITests
    {
        [TestMethod]
        public void GarnishStepDoStepTest()
        {
            GarnishStep step = new GarnishStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            step.DoStep(null, bartender.Object, "Test Ingredient");
            Assert.AreEqual("Garnish the drink with Test Ingredient.", step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Garnish", step.Name);
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void GarnishStepCloneTest()
        {
            GarnishStep originalStep = new GarnishStep();
            GarnishStep clonedStep = originalStep.Clone() as GarnishStep;
            Assert.IsNotNull(clonedStep);
            Assert.AreEqual(originalStep.IsStepDone, clonedStep.IsStepDone);
            Assert.AreEqual(originalStep.MessageToUser, clonedStep.MessageToUser);
            Assert.AreEqual(originalStep.Name, clonedStep.Name);
            Assert.AreEqual(originalStep.NextGrammarNeeded, clonedStep.NextGrammarNeeded);
            Assert.AreEqual(originalStep.ShouldCancel, clonedStep.ShouldCancel);
        }
    }
}
