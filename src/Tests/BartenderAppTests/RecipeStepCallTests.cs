using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Bartender;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace BartenderAppTests
{
    [TestClass]
    public class RecipeStepCallTests
    {
        [TestMethod]
        public void RecipeStepCallDefaultConstructorTest()
        {
            RecipeStepCall stepCall = new RecipeStepCall(null);
            Assert.AreEqual(false, stepCall.IsDone);
            Assert.AreEqual(false, stepCall.IsValid);
            Assert.AreEqual(String.Empty, stepCall.MessageToUser);
            Assert.AreEqual(String.Empty, stepCall.NextGrammarNeeded);
            Assert.AreEqual(false, stepCall.ShouldCancel);
        }

        [TestMethod]
        public void RecipeStepCallNoNextConstructorTest()
        {
            MockRecipeStep mockStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", true, true, true);
            RecipeStepCall stepCall = new RecipeStepCall(mockStep);
            Assert.AreEqual(true, stepCall.IsDone);
            Assert.AreEqual(true, stepCall.IsValid);
            Assert.AreEqual(mockStep.MessageToUser, stepCall.MessageToUser);
            Assert.AreEqual(mockStep.NextGrammarNeeded, stepCall.NextGrammarNeeded);
            Assert.AreEqual(mockStep.ShouldCancel, stepCall.ShouldCancel);
        }

        [TestMethod]
        public void RecipeStepCallValidNextConstructorTest()
        {
            MockRecipeStep mockNextStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", true, true, true);
            RecipeStepCall nextStepCall = new RecipeStepCall(mockNextStep);
            MockRecipeStep mockStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", true, true, true);
            RecipeStepCall stepCall = new RecipeStepCall(mockStep, nextStepCall);
            Assert.AreEqual(false, stepCall.IsDone);
            Assert.AreEqual(true, stepCall.IsValid);
            Assert.AreEqual(mockStep.MessageToUser, stepCall.MessageToUser);
            Assert.AreEqual(mockStep.NextGrammarNeeded, stepCall.NextGrammarNeeded);
            Assert.AreEqual(mockStep.ShouldCancel, stepCall.ShouldCancel);
        }

        [TestMethod]
        public void RecipeStepCallInvalidNextConstructorTest()
        {
            RecipeStepCall nextStepCall = new RecipeStepCall(null);
            MockRecipeStep mockStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", true, true, false);
            RecipeStepCall stepCall = new RecipeStepCall(mockStep, nextStepCall);
            Assert.AreEqual(false, stepCall.IsDone);
            Assert.AreEqual(false, stepCall.IsValid);
            Assert.AreEqual(mockStep.MessageToUser, stepCall.MessageToUser);
            Assert.AreEqual(mockStep.NextGrammarNeeded, stepCall.NextGrammarNeeded);
            Assert.AreEqual(mockStep.ShouldCancel, stepCall.ShouldCancel);
        }

        [TestMethod]
        public void RecipeStepCallCallAndAdvanceTest()
        {
            MockRecipeStep mockNextStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", false, false, false);
            RecipeStepCall nextStepCall = new RecipeStepCall(mockNextStep);
            MockRecipeStep mockStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", false, false, true);
            RecipeStepCall stepCall = new RecipeStepCall(mockStep, nextStepCall);
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            Assert.AreEqual(nextStepCall, stepCall.Call(new Dialog(new Dictionary<String, String>(), "Phrase"), bartender.Object));
            Assert.AreEqual(true, mockStep.DoStepIsCalled);
            Assert.AreEqual(true, mockNextStep.DoStepIsCalled);
        }

        [TestMethod]
        public void RecipeStepCallCallNoAdvanceTest()
        {
            MockRecipeStep mockNextStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", false, false, false);
            RecipeStepCall nextStepCall = new RecipeStepCall(mockNextStep);
            MockRecipeStep mockStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", false, false, false);
            RecipeStepCall stepCall = new RecipeStepCall(mockStep, nextStepCall);
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            Assert.AreEqual(stepCall, stepCall.Call(new Dialog(new Dictionary<String, String>(), "Phrase"), bartender.Object));
            Assert.AreEqual(true, mockStep.DoStepIsCalled);
            Assert.AreEqual(false, mockNextStep.DoStepIsCalled);
        }

        [TestMethod]
        public void RecipeStepCallCloneTest()
        {
            MockRecipeStep mockNextStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", false, false, true);
            RecipeStepCall nextStepCall = new RecipeStepCall(mockNextStep);
            MockRecipeStep mockStep = new MockRecipeStep("messageToUser", "nextGrammarNeeded", false, false, true);
            RecipeStepCall stepCall = new RecipeStepCall(mockStep, nextStepCall);

            RecipeStepCall clonedCall = stepCall.Clone() as RecipeStepCall;
            Assert.AreNotSame(clonedCall, stepCall);

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            clonedCall.Call(new Dialog(new Dictionary<String, String>(), "Phrase"), bartender.Object);
            Assert.IsFalse(mockStep.DoStepIsCalled);
            Assert.IsFalse(mockNextStep.DoStepIsCalled);
        }
    }
}
