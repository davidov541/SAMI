﻿using System;
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
    public class CrushStepTests : BaseSAMITests
    {
        [TestMethod]
        public void CrushStepDoStepNullInputTest()
        {
            CrushStep step = new CrushStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            step.DoStep(null, bartender.Object, new Ingredient("TestIngredient", 1, IngredientUnit.Part, "Test Ingredient"));
            Assert.AreEqual("Crush Test Ingredient and add to the glass and tell me when you are done.", step.MessageToUser);
            Assert.AreEqual(false, step.IsStepDone);
            Assert.AreEqual("Crush", step.Name);
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void CrushStepDoStepInvalidUserInputTest()
        {
            CrushStep step = new CrushStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            step.DoStep(new Dialog(new Dictionary<String, String>(), "Test Phrase"), bartender.Object, new Ingredient("TestIngredient", 1, IngredientUnit.Part, "Test Ingredient"));
            Assert.AreEqual("Crush Test Ingredient and add to the glass and tell me when you are done.", step.MessageToUser);
            Assert.AreEqual(false, step.IsStepDone);
            Assert.AreEqual("Crush", step.Name);
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void CrushStepDoStepTrueAnswerTest()
        {
            CrushStep step = new CrushStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"answer", "true"},
            };
            step.DoStep(new Dialog(input, "Test Phrase"), bartender.Object, new Ingredient("TestIngredient", 1, IngredientUnit.Part, "Test Ingredient"));
            Assert.AreEqual(null, step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Crush", step.Name);
            Assert.AreEqual(null, step.NextGrammarNeeded);
            Assert.AreEqual(false, step.ShouldCancel);
        }

        [TestMethod]
        public void CrushStepDoStepFalseAnswerTest()
        {
            CrushStep step = new CrushStep();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"answer", "false"},
            };
            step.DoStep(new Dialog(input, "Test Phrase"), bartender.Object, new Ingredient("TestIngredient", 1, IngredientUnit.Part, "Test Ingredient"));
            Assert.AreEqual(null, step.MessageToUser);
            Assert.AreEqual(true, step.IsStepDone);
            Assert.AreEqual("Crush", step.Name);
            Assert.AreEqual(null, step.NextGrammarNeeded);
            Assert.AreEqual(true, step.ShouldCancel);
        }

        [TestMethod]
        public void CrushStepCloneTest()
        {
            CrushStep originalStep = new CrushStep();
            CrushStep clonedStep = originalStep.Clone() as CrushStep;
            Assert.IsNotNull(clonedStep);
            Assert.AreEqual(originalStep.IsStepDone, clonedStep.IsStepDone);
            Assert.AreEqual(originalStep.MessageToUser, clonedStep.MessageToUser);
            Assert.AreEqual(originalStep.Name, clonedStep.Name);
            Assert.AreEqual(originalStep.NextGrammarNeeded, clonedStep.NextGrammarNeeded);
            Assert.AreEqual(originalStep.ShouldCancel, clonedStep.ShouldCancel);
        }
    }
}
