using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Bartender;

namespace BartenderAppTests
{
    [TestClass]
    public class RecipeStepFactoryTests
    {
        [TestMethod]
        public void RecipeStepGetInitialStepTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            InitialStep step = factory.GetInitialStep("Test Drink", "Old Fashioned") as InitialStep;
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public void RecipeGetAddRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Add");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is AddStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);
            
            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Add", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is AddStep);
            Assert.IsTrue(recipeStepCall is RecipeStepCall<Ingredient>);
        }

        [TestMethod]
        public void RecipeGetPourRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Pour");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is PourStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Pour", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is PourStep);
            Assert.IsTrue(recipeStepCall is RecipeStepCall<Ingredient>);
        }

        [TestMethod]
        public void RecipeGetShakeRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Shake");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is ShakeStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Shake", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNull(recipeStepCall._step);
        }

        [TestMethod]
        public void RecipeGetStrainRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Strain");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is StrainStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Strain", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNull(recipeStepCall._step);
        }

        [TestMethod]
        public void RecipeGetCrushRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Crush");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is CrushStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Crush", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is CrushStep);
            Assert.IsTrue(recipeStepCall is RecipeStepCall<Ingredient>);
        }

        [TestMethod]
        public void RecipeGetIgniteRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Ignite");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is IgniteStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Ignite", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNull(recipeStepCall._step);
        }

        [TestMethod]
        public void RecipeGetGarnishRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Garnish");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is GarnishStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Garnish", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNull(recipeStepCall._step);
        }

        [TestMethod]
        public void RecipeGetStirRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Stir");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is StirStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Stir", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNull(recipeStepCall._step);
        }

        [TestMethod]
        public void RecipeGetBlendRecipeStepCallTest()
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            RecipeStepCall recipeStepCall = factory.GetRecipeStepCall("Blend");
            Assert.IsNotNull(recipeStepCall._step);
            Assert.IsTrue(recipeStepCall._step is BlendStep);
            Assert.IsFalse(recipeStepCall is RecipeStepCall<Ingredient>);

            recipeStepCall = factory.GetRecipeStepCall<Ingredient>("Blend", new Ingredient("TestIngredient", 1, IngredientUnit.Dash, "Test Ingredient"));
            Assert.IsNull(recipeStepCall._step);
        }
    }
}
