using System;
using System.Collections.Generic;
using System.Xml;

namespace SAMI.Apps.Bartender
{
    internal class DrinkRecipeProvider
    {
        private List<DrinkRecipe> _drinks;
        public List<DrinkRecipe> Drinks
        {
            get
            {
                return _drinks;
            }
        }

        public DrinkRecipeProvider(String file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            ParseXmlDocument(doc);
        }

        public DrinkRecipeProvider(XmlDocument doc)
        {
            ParseXmlDocument(doc);
        }

        public DrinkRecipeProvider(List<DrinkRecipe> drinks)
        {
            _drinks = drinks;
        }

        private void ParseXmlDocument(XmlDocument doc)
        {
            Dictionary<String, String> userStringLookup = new Dictionary<string, string>();
            foreach (XmlNode ingredient in doc.SelectNodes("DrinkRecipes/Ingredients/Ingredient"))
            {
                userStringLookup[ingredient.Attributes["Key"].Value] = ingredient.Attributes["Name"].Value;
            }

            Dictionary<String, double> drinkVolumes = new Dictionary<String, double>();
            foreach (XmlNode ingredient in doc.SelectNodes("DrinkRecipes/Glasses/Glass"))
            {
                drinkVolumes[ingredient.Attributes["Name"].Value] = Double.Parse(ingredient.Attributes["Volume"].Value);
            }

            _drinks = new List<DrinkRecipe>();
            foreach (XmlNode node in doc.SelectNodes("DrinkRecipes/Cocktails/Drink"))
            {
                _drinks.Add(DrinkRecipe.CreateFromXml(node, userStringLookup, drinkVolumes));
            }

            _drinks.RemoveAll(r => r == null);
        }
    }
}
