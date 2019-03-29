using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Bartender;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.Test.Utilities;

namespace BartenderAppTests
{
    [TestClass]
    public class PourStepTests : BaseSAMITests
    {
        [TestMethod]
        public void PourStepDoStepTest()
        {
            PourStep step = new PourStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String> { "TestIngredient" });
            bartender.Setup(s => s.DispenseLiquid("TestIngredient", 1));
            step.DoStep(null, bartender.Object, new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.AreEqual(null, step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Pour", step.Name);
            Assert.AreEqual(null, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
            bartender.Verify(s => s.DispenseLiquid("TestIngredient", 1), Times.Exactly(1));
        }

        [TestMethod]
        public void PourStepCloneTest()
        {
            PourStep originalStep = new PourStep();
            PourStep clonedStep = originalStep.Clone() as PourStep;
            Assert.IsNotNull(clonedStep);
            Assert.AreEqual(originalStep.IsStepDone, clonedStep.IsStepDone);
            Assert.AreEqual(originalStep.MessageToUser, clonedStep.MessageToUser);
            Assert.AreEqual(originalStep.Name, clonedStep.Name);
            Assert.AreEqual(originalStep.NextGrammarNeeded, clonedStep.NextGrammarNeeded);
            Assert.AreEqual(originalStep.ShouldCancel, clonedStep.ShouldCancel);
        }
    }
}
