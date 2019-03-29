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
    [DeploymentItem("BartenderGrammar.grxml")]
    [DeploymentItem("drinks.xml")]
    [TestClass]
    public class BartenderGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void CleanOutPumpsGrammarTest()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "cleanPumps"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);
            BartenderApp app = new BartenderApp(GetProvider());

            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                });
            TestGrammar(app, GetConfigurationManager(), "Clean out the pumps", expectedParams);

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(3));
        }

        [TestMethod]
        public void WhatDrinksCanYouMakeGrammarTest()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "drinkList"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);
            BartenderApp app = new BartenderApp(GetProvider());

            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                });
            TestGrammar(app, GetConfigurationManager(), "What drinks can you make", expectedParams);

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(3));
        }

        [TestMethod]
        public void MakeMeADrinkGrammarTest()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"DrinkName", "Recipe 1"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);
            BartenderApp app = new BartenderApp(GetProvider());

            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                });
            TestGrammar(app, GetConfigurationManager(), "Make me a Recipe 1", expectedParams);
            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(3));
            bartender.ResetCalls();

            TestGrammar(app, GetConfigurationManager(), "Give me a Recipe 1", expectedParams);
            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(3));
        }

        [TestMethod]
        public void WhatIsInAGrammarTest()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Bartender"},
                {"Subcommand", "ingredientList"},
                {"DrinkName", "Recipe 1"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IBartenderController> bartender = new Mock<IBartenderController>(MockBehavior.Strict);
            bartender.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(bartender.Object);
            BartenderApp app = new BartenderApp(GetProvider());

            bartender.Setup(s => s.AvailableLiquids).Returns(new List<String>
                {
                    "testLiquid1",
                    "testLiquid2",
                });
            TestGrammar(app, GetConfigurationManager(), "What is in a Recipe 1", expectedParams);

            bartender.Verify(s => s.AvailableLiquids, Times.Exactly(3));
        }

        private DrinkRecipeProvider GetProvider()
        {
            DrinkRecipe recipe1 = new DrinkRecipe("Recipe 1");
            recipe1.Ingredients.Add(new Ingredient("testLiquid1", 1, IngredientUnit.Part, "testLiquid1"));
            recipe1.Ingredients.Add(new Ingredient("testLiquid2", 1, IngredientUnit.Part, "testLiquid2"));
            RecipeStepCall call = new RecipeStepCall(new StirStep());
            recipe1.FirstStep = call;
            DrinkRecipe recipe2 = new DrinkRecipe("Recipe 2");
            recipe2.Ingredients.Add(new Ingredient("testLiquid1", 1, IngredientUnit.Part, "testLiquid1"));
            recipe2.Ingredients.Add(new Ingredient("testLiquid2", 1, IngredientUnit.Part, "testLiquid2"));
            recipe2.Ingredients.Add(new Ingredient("testLiquid3", 1, IngredientUnit.Part, "testLiquid3"));
            call = new RecipeStepCall(new StirStep());
            recipe2.FirstStep = call;
            return new DrinkRecipeProvider(new List<DrinkRecipe>
                {
                    recipe1,
                    recipe2,
                });
        }
    }
}
