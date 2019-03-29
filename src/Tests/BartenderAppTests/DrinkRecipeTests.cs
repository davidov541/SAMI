using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Bartender;
using SAMI.Test.Utilities;

namespace BartenderAppTests
{
    [TestClass]
    public class DrinkRecipeTests : BaseSAMITests
    {
        [TestMethod]
        public void DrinkRecipeConstructorTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            Assert.AreEqual("Test Recipe", recipe.Name);
            Assert.IsFalse(recipe.Ingredients.Any());
            Assert.IsNull(recipe.FirstStep);
        }

        [TestMethod]
        public void DrinkRecipeStartTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(new AddStep());
            RecipeStepCall firstStep = recipe.Start();
            Assert.AreNotSame(recipe.FirstStep, firstStep);
            Assert.AreEqual(recipe.FirstStep, firstStep);
        }

        [TestMethod]
        public void DrinkRecipeCanMakeRecipeTrueTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(new AddStep());
            recipe.Ingredients.Add(new Ingredient("TestIngredient1", 1, IngredientUnit.Dash, "Test Ingredient 1"));
            recipe.Ingredients.Add(new Ingredient("TestIngredient2", 1, IngredientUnit.Dash, "Test Ingredient 2"));
            Assert.IsTrue(recipe.CanMakeFromRecipes(new List<String> { "TestIngredient1", "TestIngredient2", "TestIngredient3" }));
        }

        [TestMethod]
        public void DrinkRecipeCanMakeRecipeTrueWithIceTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(new AddStep());
            recipe.Ingredients.Add(new Ingredient("TestIngredient1", 1, IngredientUnit.Dash, "Test Ingredient 1"));
            recipe.Ingredients.Add(new Ingredient("TestIngredient2", 1, IngredientUnit.Dash, "Test Ingredient 2"));
            recipe.Ingredients.Add(new Ingredient("IceIngredient", 1, IngredientUnit.Ice, "Test Ingredient 2"));
            Assert.IsTrue(recipe.CanMakeFromRecipes(new List<String> { "TestIngredient1", "TestIngredient2", "TestIngredient3" }));
        }

        [TestMethod]
        public void DrinkRecipeCanMakeRecipeTrueWithWhippedToppingTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(new AddStep());
            recipe.Ingredients.Add(new Ingredient("TestIngredient1", 1, IngredientUnit.Dash, "Test Ingredient 1"));
            recipe.Ingredients.Add(new Ingredient("TestIngredient2", 1, IngredientUnit.Dash, "Test Ingredient 2"));
            recipe.Ingredients.Add(new Ingredient("WhippedCreamIngredient", 1, IngredientUnit.Whipped_Cream, "Test Ingredient 2"));
            Assert.IsTrue(recipe.CanMakeFromRecipes(new List<String> { "TestIngredient1", "TestIngredient2", "TestIngredient3" }));
        }

        [TestMethod]
        public void DrinkRecipeCanMakeRecipeTrueNoIngredentsTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(new AddStep());
            Assert.IsTrue(recipe.CanMakeFromRecipes(new List<String> { "TestIngredient1", "TestIngredient2", "TestIngredient3" }));
        }

        [TestMethod]
        public void DrinkRecipeCanMakeRecipeFalseNoIngredientTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(new AddStep());
            recipe.Ingredients.Add(new Ingredient("TestIngredient1", 1, IngredientUnit.Dash, "Test Ingredient 1"));
            recipe.Ingredients.Add(new Ingredient("NonexistentIngredient", 1, IngredientUnit.Dash, "Test Ingredient 2"));
            Assert.IsFalse(recipe.CanMakeFromRecipes(new List<String> { "TestIngredient1", "TestIngredient2", "TestIngredient3" }));
        }

        [TestMethod]
        public void DrinkRecipeCanMakeRecipeFalseInvalidStepTest()
        {
            DrinkRecipe recipe = new DrinkRecipe("Test Recipe");
            recipe.FirstStep = new RecipeStepCall(null);
            recipe.Ingredients.Add(new Ingredient("TestIngredient1", 1, IngredientUnit.Dash, "Test Ingredient 1"));
            recipe.Ingredients.Add(new Ingredient("NonexistentIngredient", 1, IngredientUnit.Dash, "Test Ingredient 2"));
            Assert.IsFalse(recipe.CanMakeFromRecipes(new List<String> { "TestIngredient1", "TestIngredient2", "TestIngredient3" }));
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlNameTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"part\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"part\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                });
            Assert.AreEqual("Raspberry Champagne", recipe.Name);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassChampagneTest()
        {
            TestGlass("Champagne", 200);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassCocktailTest()
        {
            TestGlass("Cocktail", 250);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassOldFashionedTest()
        {
            TestGlass("Old Fashioned", 250);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassCollinsTest()
        {
            TestGlass("Collins", 400);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassHurricaneTest()
        {
            TestGlass("Hurricane", 300);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassSourTest()
        {
            TestGlass("Sour", 120);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassShooterTest()
        {
            TestGlass("Shooter", 25);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassIrishCoffeeGlassTest()
        {
            TestGlass("Irish Coffee Glass", 177);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmGlassSnifterTest()
        {
            TestGlass("Snifter", 350);
        }

        private static void TestGlass(String glassName, int glassVolume)
        {
            String xmlFile = String.Format("<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"{0}\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"part\" />></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>", glassName);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
            }, new Dictionary<String, double>
                {
                    {glassName, glassVolume},
                });
            Ingredient raspberryIngredient = recipe.Ingredients.SingleOrDefault();
            Assert.IsNotNull(raspberryIngredient);
            Assert.AreEqual(glassVolume, (int)raspberryIngredient.AmountQuantityInmL);

            recipe.FirstStep.Call(null, null);
            Assert.AreEqual(String.Format("Place a {0} glass underneath the spout of the bartender so I can make you a Raspberry Champagne.", glassName), recipe.FirstStep.MessageToUser);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlIngredientsOnlyPartsTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Collins\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"part\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"part\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Collins", 400},
                });
            Assert.AreEqual(2, recipe.Ingredients.Count);

            Ingredient raspberryIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Black Raspberry Liqueur"));
            Assert.IsNotNull(raspberryIngredient);
            Assert.AreEqual(400 / 6, (int)raspberryIngredient.AmountQuantityInmL);
            Assert.AreEqual(1, raspberryIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Part, raspberryIngredient.AmountUnit);

            Ingredient chammpaigneIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Champagne"));
            Assert.IsNotNull(chammpaigneIngredient);
            Assert.AreEqual(5 * 400 / 6, (int)chammpaigneIngredient.AmountQuantityInmL);
            Assert.AreEqual(5, chammpaigneIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Part, chammpaigneIngredient.AmountUnit);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlIngredientsOnlyExactTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                });
            Assert.AreEqual(2, recipe.Ingredients.Count);

            Ingredient raspberryIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Black Raspberry Liqueur"));
            Assert.IsNotNull(raspberryIngredient);
            Assert.AreEqual(1, (int)raspberryIngredient.AmountQuantityInmL);
            Assert.AreEqual(1, raspberryIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Dash, raspberryIngredient.AmountUnit);

            Ingredient chammpaigneIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Champagne"));
            Assert.IsNotNull(chammpaigneIngredient);
            Assert.AreEqual(5, (int)chammpaigneIngredient.AmountQuantityInmL);
            Assert.AreEqual(5, chammpaigneIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Dash, chammpaigneIngredient.AmountUnit);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlIngredientsMixedAmountsTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Collins\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"part\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Collins", 400},
                });
            Assert.AreEqual(2, recipe.Ingredients.Count);

            Ingredient raspberryIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Black Raspberry Liqueur"));
            Assert.IsNotNull(raspberryIngredient);
            Assert.AreEqual(395, (int)raspberryIngredient.AmountQuantityInmL);
            Assert.AreEqual(1, raspberryIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Part, raspberryIngredient.AmountUnit);

            Ingredient chammpaigneIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Champagne"));
            Assert.IsNotNull(chammpaigneIngredient);
            Assert.AreEqual(5, (int)chammpaigneIngredient.AmountQuantityInmL);
            Assert.AreEqual(5, chammpaigneIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Dash, chammpaigneIngredient.AmountUnit);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlIngredientsMixedAmountsLargeFixedTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Collins\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"part\" /><Ingredient Key=\"champagne\" Quantity=\"500\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Collins", 400},
                });
            // Test ingredients
            Assert.AreEqual(2, recipe.Ingredients.Count);

            Ingredient raspberryIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Black Raspberry Liqueur"));
            Assert.IsNotNull(raspberryIngredient);
            Assert.AreEqual(400, (int)raspberryIngredient.AmountQuantityInmL);
            Assert.AreEqual(1, raspberryIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Part, raspberryIngredient.AmountUnit);

            Ingredient chammpaigneIngredient = recipe.Ingredients.SingleOrDefault(i => i.Name.Equals("Champagne"));
            Assert.IsNotNull(chammpaigneIngredient);
            Assert.AreEqual(500, (int)chammpaigneIngredient.AmountQuantityInmL);
            Assert.AreEqual(500, chammpaigneIngredient.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Dash, chammpaigneIngredient.AmountUnit);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlIngredientsInvalidIngredientNameTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"part\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
            }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                });
            Assert.IsNull(recipe);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlStepsIngredientArgumentsTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Pour\" Arguments=\"champagne,black_raspberry_liqueur\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                });

            // Test Steps
            Assert.IsTrue(recipe.FirstStep._nextStep is RecipeStepCall<Ingredient>);
            Assert.IsTrue(recipe.FirstStep._nextStep._step is PourStep);
            RecipeStepCall<Ingredient> stepCall = recipe.FirstStep._nextStep as RecipeStepCall<Ingredient>;
            Assert.AreEqual(new Ingredient("champagne", 5, IngredientUnit.Dash, "Champagne"), stepCall._argument);
            Assert.IsTrue(recipe.FirstStep._nextStep._nextStep._step is PourStep);
            Assert.IsTrue(recipe.FirstStep._nextStep._nextStep is RecipeStepCall<Ingredient>);
            stepCall = recipe.FirstStep._nextStep._nextStep as RecipeStepCall<Ingredient>;
            Assert.AreEqual(new Ingredient("black_raspberry_liqueur", 1, IngredientUnit.Dash, "Black Raspberry Liqueur"), stepCall._argument);
        }

        [TestMethod]
        public void DrinkRecipeCreateFromXmlStepsNonIngredientArgumentsTest()
        {
            String xmlFile = "<Drink Key=\"testdrink\" Name=\"Raspberry Champagne\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Garnish\" Arguments=\"lemon_wheel\" /></Steps></Drink>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipe recipe = DrinkRecipe.CreateFromXml(doc.LastChild, new Dictionary<String, String>
            {
                {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                {"champagne", "Champagne"},
            }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                });

            // Test Steps
            Assert.IsTrue(recipe.FirstStep._nextStep is RecipeStepCall<String>);
            Assert.IsTrue(recipe.FirstStep._nextStep._step is GarnishStep);
            RecipeStepCall<String> stepCall = recipe.FirstStep._nextStep as RecipeStepCall<String>;
            Assert.AreEqual("lemon wheel", stepCall._argument);
        }
    }
}
