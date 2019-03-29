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
    public class IngredientTests : BaseSAMITests
    {
        [TestMethod]
        public void IngredientFullConstructorTest()
        {
            String ingredientKey = "IngredientKey";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            Assert.AreEqual(ingredientKey, ingredient.Key);
            Assert.AreEqual(amountQuantity, ingredient.AmountQuantity);
            Assert.AreEqual(unit, ingredient.AmountUnit);
            Assert.AreEqual(ingredientName, ingredient.Name);
        }

        [TestMethod]
        public void IngredientSingleIngredientChoiceConstructorTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            Ingredient ingredient = new Ingredient(ingredientKey, amountQuantity, unit, ingredientName);
            Assert.AreEqual(ingredientKey, ingredient.Key);
            Assert.AreEqual(amountQuantity, ingredient.AmountQuantity);
            Assert.AreEqual(unit, ingredient.AmountUnit);
            Assert.AreEqual(ingredientName, ingredient.Name);
        }

        [TestMethod]
        public void IngredientQuantityInmLDashTest()
        {
            TestQuantityInmL(IngredientUnit.Dash, 10, 20, 10);
        }

        [TestMethod]
        public void IngredientQuantityInmLTeaspoonTest()
        {
            TestQuantityInmL(IngredientUnit.Teaspoon, 10, 20, 50);
        }

        [TestMethod]
        public void IngredientQuantityInmLSplashTest()
        {
            TestQuantityInmL(IngredientUnit.Splash, 10, 20, 250);
        }

        [TestMethod]
        public void IngredientQuantityInmLPintTest()
        {
            TestQuantityInmL(IngredientUnit.Pint, 10, 20, 4731.8);
        }

        [TestMethod]
        public void IngredientQuantityInmLDropTest()
        {
            TestQuantityInmL(IngredientUnit.Drop, 10, 20, 5);
        }

        [TestMethod]
        public void IngredientQuantityInmLLitreTest()
        {
            TestQuantityInmL(IngredientUnit.Litre, 10, 20, 10000);
        }

        [TestMethod]
        public void IngredientQuantityInmLPartTest()
        {
            TestQuantityInmL(IngredientUnit.Part, 10, 20, 200);
        }

        [TestMethod]
        public void IngredientQuantityInmLBottleTest()
        {
            TestQuantityInmL(IngredientUnit.Bottle, 10, 0, 0);
        }

        private void TestQuantityInmL(IngredientUnit unit, int amountQuantity, int partAmount, double expectedAmountQuantity)
        {
            String ingredientKey = "IngredientKey";
            String ingredientName = "Ingredient Name";
            Ingredient ingredient = new Ingredient(ingredientKey, amountQuantity, unit, ingredientName);
            ingredient.SetPartAmount(partAmount);
            Assert.AreEqual(expectedAmountQuantity, ingredient.AmountQuantityInmL);
        }

        [TestMethod]
        public void IngredientGetAvailableKeysNoMatchesTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String> {
                "NonChoice1",
                "NonChoice2",
                "NonChoice3",
            });
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void IngredientGetAvailableKeysOneMatchTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String> {
                "IngredientChoice1",
                "NonChoice2",
                "NonChoice3",
            });
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(d => d.Equals(choices[0].Key)));
        }

        [TestMethod]
        public void IngredientGetAvailableKeysManyMatchesTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String> {
                "IngredientChoice1",
                "IngredientChoice2",
                "NonChoice3",
            });
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(d => d.Equals(choices[0].Key)));
            Assert.IsTrue(result.Any(d => d.Equals(choices[1].Key)));
        }

        [TestMethod]
        public void IngredientGetAvailableKeysAllMatchTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String> {
                "IngredientChoice1",
                "IngredientChoice2",
                "IngredientChoice3",
            });
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(d => d.Equals(choices[0].Key)));
            Assert.IsTrue(result.Any(d => d.Equals(choices[1].Key)));
            Assert.IsTrue(result.Any(d => d.Equals(choices[2].Key)));
        }

        [TestMethod]
        public void IngredientGetAvailableKeysOneChoiceTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String> {
                "IngredientChoice1",
                "IngredientChoice2",
                "IngredientChoice3",
            });
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(d => d.Equals(choices[0].Key)));
        }

        [TestMethod]
        public void IngredientGetAvailableKeysNoChoicesTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>();
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String> {
                "IngredientChoice1",
                "IngredientChoice2",
                "IngredientChoice3",
            });
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void IngredientGetAvailableKeysNoDispensersTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String>());
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void IngredientGetAvailableKeysOneDispenserTest()
        {
            String ingredientKey = "IngredientKey";
            double amountQuantity = 10;
            IngredientUnit unit = IngredientUnit.Bottle;
            String ingredientName = "Ingredient Name";
            List<IngredientChoice> choices = new List<IngredientChoice>
            {
                new IngredientChoice("IngredientChoice1", "Ingredient Choice One"),
                new IngredientChoice("IngredientChoice2", "Ingredient Choice Two"),
                new IngredientChoice("IngredientChoice3", "Ingredient Choice Three"),
            };
            Ingredient ingredient = new Ingredient(ingredientKey, choices, amountQuantity, unit, ingredientName);
            IEnumerable<String> result = ingredient.GetAvailableKeys(new List<String>
                {
                    "IngredientChoice1",
                });
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(d => d.Equals(choices[0].Key)));
        }

        [TestMethod]
        public void IngredientParseBottleUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"bottle\" />", "testIngredient", 1.5, IngredientUnit.Bottle);
        }

        [TestMethod]
        public void IngredientParseDashUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"dash\" />", "testIngredient", 1.5, IngredientUnit.Dash);
        }

        [TestMethod]
        public void IngredientParseDropUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"drop\" />", "testIngredient", 1.5, IngredientUnit.Drop);
        }

        [TestMethod]
        public void IngredientParseHalfUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"half\" />", "testIngredient", 1.5, IngredientUnit.Half);
        }

        [TestMethod]
        public void IngredientParseIceUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"ice\" />", "testIngredient", 1.5, IngredientUnit.Ice);
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"(ice)\" />", "testIngredient", 1.5, IngredientUnit.Ice);
        }

        [TestMethod]
        public void IngredientParseLitreUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"litre\" />", "testIngredient", 1.5, IngredientUnit.Litre);
        }

        [TestMethod]
        public void IngredientParsePartUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"part\" />", "testIngredient", 1.5, IngredientUnit.Part);
        }

        [TestMethod]
        public void IngredientParsePieceUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"piece\" />", "testIngredient", 1.5, IngredientUnit.Piece);
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"(piece)\" />", "testIngredient", 1.5, IngredientUnit.Piece);
        }

        [TestMethod]
        public void IngredientParsePinchUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"pinch\" />", "testIngredient", 1.5, IngredientUnit.Pinch);
        }

        [TestMethod]
        public void IngredientParsePintUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"pint\" />", "testIngredient", 1.5, IngredientUnit.Pint);
        }

        [TestMethod]
        public void IngredientParseScoopUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"scoop\" />", "testIngredient", 1.5, IngredientUnit.Scoop);
        }

        [TestMethod]
        public void IngredientParseSplashUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"splash\" />", "testIngredient", 1.5, IngredientUnit.Splash);
        }

        [TestMethod]
        public void IngredientParseTeaspoonUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"teaspoon\" />", "testIngredient", 1.5, IngredientUnit.Teaspoon);
        }

        [TestMethod]
        public void IngredientParseWhippedCreamUnitTest()
        {
            TestParsing("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"(whipped_cream)\" />", "testIngredient", 1.5, IngredientUnit.Whipped_Cream);
        }

        private void TestParsing(String xmlString, String key, double quantity, IngredientUnit unit)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            Ingredient result = Ingredient.CreateFromXml(doc.LastChild, new Dictionary<String, String>
                {
                    {key, "test name"},
                });
            Assert.AreEqual(key, result.Key);
            Assert.AreEqual(quantity, result.AmountQuantity);
            Assert.AreEqual(unit, result.AmountUnit);
            Assert.AreEqual("test name", result.Name);
        }

        [TestMethod]
        public void IngredientParseInvalidKeyTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Ingredient Key=\"testIngredient\" Quantity=\"1.5\" Unit=\"(whipped_cream)\" />");
            Ingredient result = Ingredient.CreateFromXml(doc.LastChild, new Dictionary<String, String>());
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IngredientParseInvalidChoiceTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<ChoiceOf Key=\"test_choice\" Quantity=\"1\" Unit=\"dash\"><Ingredient Key=\"triple_sec\" /><Ingredient Key=\"cointreau\" /></ChoiceOf>");
            Ingredient result = Ingredient.CreateFromXml(doc.LastChild, new Dictionary<String, String>
                {
                    {"test_choice", "test name"},
                    {"triple_sec", "triple sec"},
                });
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IngredientParseChoiceOfUnitTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<ChoiceOf Key=\"test_choice\" Quantity=\"1\" Unit=\"dash\"><Ingredient Key=\"triple_sec\" /><Ingredient Key=\"cointreau\" /></ChoiceOf>");
            Ingredient result = Ingredient.CreateFromXml(doc.LastChild, new Dictionary<String, String>
                {
                    {"test_choice", "test name"},
                    {"triple_sec", "triple sec"},
                    {"cointreau", "cointreau"},
                });
            Assert.AreEqual("test_choice", result.Key);
            Assert.AreEqual(1, result.AmountQuantity);
            Assert.AreEqual(IngredientUnit.Dash, result.AmountUnit);
            Assert.AreEqual("test_choice", result.Name);
            IEnumerable<String> possibleDrinks = result.GetAvailableKeys(new List<String>
                {
                    "triple_sec",
                    "cointreau",
                });
            Assert.AreEqual(2, possibleDrinks.Count());
            Assert.IsTrue(possibleDrinks.Any(d => d.Equals("triple_sec")));
            Assert.IsTrue(possibleDrinks.Any(d => d.Equals("cointreau")));
        }
    }
}
