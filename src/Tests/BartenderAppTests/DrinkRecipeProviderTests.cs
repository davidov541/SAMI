using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Bartender;

namespace BartenderAppTests
{
    [TestClass]
    public class DrinkRecipeProviderTests
    {
        [TestMethod]
        public void DrinkRecipeProviderConstructorInvalidDrinksTest()
        {
            String recipe1XmlFile = "<Drink Key=\"testdrink\" Name=\"Test Drink\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Pour\" Arguments=\"champagne,black_raspberry_liqueur\" /></Steps></Drink>";
            String recipe2XmlFile = "<Drink Key=\"testdrink2\" Name=\"Test Drink 2\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            String xmlFile = String.Format("<DrinkRecipes><Cocktails>{0}{1}</Cocktails><Ingredients></Ingredients><Glasses><Glass Name=\"Champagne\" Volume=\"200\" /></Glasses></DrinkRecipes>", recipe1XmlFile, recipe2XmlFile);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipeProvider provider = new DrinkRecipeProvider(doc);
            Assert.AreEqual(0, provider.Drinks.Count);
        }

        [TestMethod]
        public void DrinkRecipeProviderConstructorInvalidGlassTest()
        {
            String recipe1XmlFile = "<Drink Key=\"testdrink\" Name=\"Test Drink\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Pour\" Arguments=\"champagne,black_raspberry_liqueur\" /></Steps></Drink>";
            String recipe2XmlFile = "<Drink Key=\"testdrink2\" Name=\"Test Drink 2\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            String xmlFile = String.Format("<DrinkRecipes><Cocktails>{0}{1}</Cocktails><Ingredients><Ingredient Key=\"champagne\" CabinetShelf=\"None\" Name=\"Champagne\"/><Ingredient Key=\"black_raspberry_liqueur\" CabinetShelf=\"None\" Name=\"Black Raspberry Liqueur\"/></Ingredients><Glasses></Glasses></DrinkRecipes>", recipe1XmlFile, recipe2XmlFile);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipeProvider provider = new DrinkRecipeProvider(doc);
            Assert.AreEqual(0, provider.Drinks.Count);
        }

        [TestMethod]
        public void DrinkRecipeProviderConstructorTest()
        {
            String recipe1XmlFile = "<Drink Key=\"testdrink\" Name=\"Test Drink\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Pour\" Arguments=\"champagne,black_raspberry_liqueur\" /></Steps></Drink>";
            String recipe2XmlFile = "<Drink Key=\"testdrink2\" Name=\"Test Drink 2\" Glass=\"Champagne\"><Ingredients><Ingredient Key=\"black_raspberry_liqueur\" Quantity=\"1\" Unit=\"dash\" /><Ingredient Key=\"champagne\" Quantity=\"5\" Unit=\"dash\" /></Ingredients><Steps><Step Type=\"Stir\" /></Steps></Drink>";
            String xmlFile = String.Format("<DrinkRecipes><Cocktails>{0}{1}</Cocktails><Ingredients><Ingredient Key=\"champagne\" CabinetShelf=\"None\" Name=\"Champagne\"/><Ingredient Key=\"black_raspberry_liqueur\" CabinetShelf=\"None\" Name=\"Black Raspberry Liqueur\"/></Ingredients><Glasses><Glass Name=\"Champagne\" Volume=\"200\" /></Glasses></DrinkRecipes>", recipe1XmlFile, recipe2XmlFile);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            DrinkRecipeProvider provider = new DrinkRecipeProvider(doc);
            Assert.AreEqual(2, provider.Drinks.Count);

            DrinkRecipe testDrinkRecipe = provider.Drinks.SingleOrDefault(d => d.Name.Equals("Test Drink"));
            XmlDocument recipe1Doc = new XmlDocument();
            recipe1Doc.LoadXml(recipe1XmlFile);
            Assert.AreEqual(DrinkRecipe.CreateFromXml(recipe1Doc.LastChild, new Dictionary<String, String>
                {
                    {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                    {"champagne", "Champagne"},
                }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                }), testDrinkRecipe);

            DrinkRecipe testDrink2Recipe = provider.Drinks.SingleOrDefault(d => d.Name.Equals("Test Drink 2"));
            XmlDocument recipe2Doc = new XmlDocument();
            recipe2Doc.LoadXml(recipe2XmlFile);
            Assert.AreEqual(DrinkRecipe.CreateFromXml(recipe2Doc.LastChild, new Dictionary<String, String>
                {
                    {"black_raspberry_liqueur", "Black Raspberry Liqueur"},
                    {"champagne", "Champagne"},
                }, new Dictionary<String, double>
                {
                    {"Champagne", 200},
                }), testDrink2Recipe);
        }
    }
}
