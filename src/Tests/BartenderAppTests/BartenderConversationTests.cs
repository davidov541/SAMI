using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Bartender;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.Test.Utilities;

namespace BartenderAppTests
{
    [TestClass]
    public class BartenderConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void WhatDrinksCanYouMakeConversationTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "drinkList"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                    "testLiquid3",
                });
            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            Assert.AreEqual("I can make Recipe 1. Recipe 2. and Recipe 3.", RunSingleConversation<BartenderConversation>(input));

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
        }

        [TestMethod]
        public void WhatIsInAConversationTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "ingredientList"},
                {"DrinkName", "Recipe 1"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            Assert.AreEqual("A Recipe 1 contains testLiquid1. and testLiquid2.", RunSingleConversation<BartenderConversation>(input));
        }

        [TestMethod]
        public void CleanPumpsConversationSuccessTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "cleanPumps"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            Assert.AreEqual("Make sure that the pumps are all no longer in the drinks. Pumps will run one at a time until all have been cleaned. Tell me when you are ready.", RunSingleConversation<BartenderConversation>(input, false));

            Dictionary<String, String> responseInput = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"answer", "true"},
            };
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, CurrentConversation.GrammarRuleName);
            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                    "testLiquid3",
                });
            bartender.Setup(s => s.DispenseLiquid("testLiquid1", 25));
            bartender.Setup(s => s.DispenseLiquid("testLiquid2", 25));
            bartender.Setup(s => s.DispenseLiquid("testLiquid3", 25));
            Assert.AreEqual("Cleaning the pumps now.", RunSingleConversation<BartenderConversation>(responseInput));

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
            bartender.Verify(s => s.DispenseLiquid("testLiquid1", 25), Times.Exactly(1));
            bartender.Verify(s => s.DispenseLiquid("testLiquid2", 25), Times.Exactly(1));
            bartender.Verify(s => s.DispenseLiquid("testLiquid3", 25), Times.Exactly(1));
        }

        [TestMethod]
        public void CleanPumpsConversationFailureTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "cleanPumps"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            Assert.AreEqual("Make sure that the pumps are all no longer in the drinks. Pumps will run one at a time until all have been cleaned. Tell me when you are ready.", RunSingleConversation<BartenderConversation>(input, false));

            Dictionary<String, String> responseInput = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"answer", "false"},
            };
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, CurrentConversation.GrammarRuleName);
            Assert.AreEqual("Never mind.", RunSingleConversation<BartenderConversation>(responseInput));
        }

        [TestMethod]
        public void NoControllersConversationTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "ingredientList"},
                {"DrinkName", "Recipe 1"},
            };
            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            Assert.AreEqual(String.Empty, RunSingleConversation<BartenderConversation>(input));
        }

        [TestMethod]
        public void MakeMeADrinkConversationSuccessTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"DrinkName", "Recipe 1"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                    "testLiquid3",
                });
            Assert.AreEqual("Testing request", RunSingleConversation<BartenderConversation>(input, false));

            Dictionary<String, String> responseInput = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"answer", "false"},
            };
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, CurrentConversation.GrammarRuleName);
            Assert.AreEqual("Testing Finish", RunSingleConversation<BartenderConversation>(responseInput));

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
        }

        [TestMethod]
        public void MakeMeADrinkConversationSuccessUseDefaultMessageTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"DrinkName", "Recipe 3"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                    "testLiquid3",
                });
            Assert.AreEqual("Testing request", RunSingleConversation<BartenderConversation>(input, false));

            Dictionary<String, String> responseInput = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"answer", "false"},
            };
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, CurrentConversation.GrammarRuleName);
            Assert.AreEqual("Your Recipe 3 is now being made.", RunSingleConversation<BartenderConversation>(responseInput));

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
        }

        [TestMethod]
        public void MakeMeADrinkConversationFailureTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"DrinkName", "Recipe 2"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                    "testLiquid3",
                });
            Assert.AreEqual("Testing request", RunSingleConversation<BartenderConversation>(input, false));

            Dictionary<String, String> responseInput = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"answer", "false"},
            };
            Assert.AreEqual(BartenderApp.ConfirmationRuleName, CurrentConversation.GrammarRuleName);
            Assert.AreEqual("Never mind", RunSingleConversation<BartenderConversation>(responseInput));

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
        }

        [TestMethod]
        public void MakeMeADrinkConversationFailureNoControllersTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"DrinkName", "Recipe 2"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);

            CurrentConversation = new BartenderConversation(GetConfigurationManager(), GetProvider());
            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>());
            Assert.AreEqual(String.Empty, RunSingleConversation<BartenderConversation>(input));

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(1));
        }

        private DrinkRecipeProvider GetProvider()
        {
            DrinkRecipe recipe1 = new DrinkRecipe("Recipe 1");
            recipe1.Ingredients.Add(new Ingredient("testLiquid1", 1, IngredientUnit.Part, "testLiquid1"));
            recipe1.Ingredients.Add(new Ingredient("testLiquid2", 1, IngredientUnit.Part, "testLiquid2"));
            RecipeStepCall call = new RecipeStepCall(new MockRecipeStep(String.Empty, String.Empty, true, false, true));
            recipe1.FirstStep = call;
            call.AddNextStep(new RecipeStepCall(new MockRecipeStep("Testing request", BartenderApp.ConfirmationRuleName, false, false, false)));
            call._nextStep.AddNextStep(new RecipeStepCall(new MockRecipeStep("Testing Finish", String.Empty, true, false, true)));

            DrinkRecipe recipe2 = new DrinkRecipe("Recipe 2");
            recipe2.Ingredients.Add(new Ingredient("testLiquid1", 1, IngredientUnit.Part, "testLiquid1"));
            recipe2.Ingredients.Add(new Ingredient("testLiquid2", 1, IngredientUnit.Part, "testLiquid2"));
            recipe2.Ingredients.Add(new Ingredient("testLiquid3", 1, IngredientUnit.Part, "testLiquid3"));
            call = new RecipeStepCall(new MockRecipeStep(String.Empty, String.Empty, true, false, true));
            recipe2.FirstStep = call;
            call.AddNextStep(new RecipeStepCall(new MockRecipeStep("Testing request", BartenderApp.ConfirmationRuleName, false, false, false)));
            call._nextStep.AddNextStep(new RecipeStepCall(new MockRecipeStep("Testing Cancel", String.Empty, false, true, false)));

            DrinkRecipe recipe3 = new DrinkRecipe("Recipe 3");
            recipe3.Ingredients.Add(new Ingredient("testLiquid1", 1, IngredientUnit.Part, "testLiquid1"));
            recipe3.Ingredients.Add(new Ingredient("testLiquid2", 1, IngredientUnit.Part, "testLiquid2"));
            call = new RecipeStepCall(new MockRecipeStep(String.Empty, String.Empty, true, false, true));
            recipe3.FirstStep = call;
            call.AddNextStep(new RecipeStepCall(new MockRecipeStep("Testing request", BartenderApp.ConfirmationRuleName, false, false, false)));
            call._nextStep.AddNextStep(new RecipeStepCall(new MockRecipeStep(String.Empty, String.Empty, true, false, true)));
            return new DrinkRecipeProvider(new List<DrinkRecipe>
                {
                    recipe1,
                    recipe2,
                    recipe3,
                });
        }
    }
}
