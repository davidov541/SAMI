using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SAMI.Apps.Bartender
{
    internal class Ingredient
    {
        private double _partAmount;
        private List<IngredientChoice> _choiceKeys;

        public String Key
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public double AmountQuantity
        {
            get;
            private set;
        }

        public IngredientUnit AmountUnit
        {
            get;
            private set;
        }

        public double AmountQuantityInmL
        {
            get
            {
                return ConvertToMilliliters(AmountQuantity, AmountUnit, _partAmount);
            }
        }

        internal Ingredient(String key, double amountQuantity, IngredientUnit amountUnit, String name)
            : this(key, new List<IngredientChoice> { new IngredientChoice(key, name) }, amountQuantity, amountUnit, name)
        {
        }

        internal Ingredient(String key, List<IngredientChoice> choiceKeys, double amountQuantity, IngredientUnit amountUnit, String name)
        {
            _choiceKeys = choiceKeys;
            Key = key;
            AmountQuantity = amountQuantity;
            AmountUnit = amountUnit;
            Name = name;
            _partAmount = 0;
        }

        public void Initialize()
        {
        }

        public void Dispose()
        {
        }

        public IEnumerable<String> GetAvailableKeys(IEnumerable<String> dispensers)
        {
            return dispensers.Where(dk => _choiceKeys.Select(d => d.Key).Contains(dk));
        }

        public void SetPartAmount(double partAmount)
        {
            _partAmount = partAmount;
        }

        public override bool Equals(object obj)
        {
            Ingredient other = obj as Ingredient;
            if (other == null)
            {
                return false;
            }
            else if (other.AmountQuantity != AmountQuantity)
            {
                return false;
            }
            else if (other.AmountUnit != AmountUnit)
            {
                return false;
            }
            else if (other.Key == null && Key != null)
            {
                return false;
            }
            else if (other.Key != null && !other.Key.Equals(Key))
            {
                return false;
            }
            else if (other.Name == null && Name != null)
            {
                return false;
            }
            else if (other.Name != null && !other.Name.Equals(Name))
            {
                return false;
            }
            else if (other._partAmount != _partAmount)
            {
                return false;
            }
            else if (other._choiceKeys.Count != _choiceKeys.Count)
            {
                return false;
            }
            foreach (IngredientChoice choice in _choiceKeys)
            {
                if (!other._choiceKeys.Contains(choice))
                {
                    return false;
                }
            }
            return true;
        }

        #region Static Functions
        public static Ingredient CreateFromXml(XmlNode ingredientNode, Dictionary<String, String> userNameLookup)
        {
            IngredientUnit unit = IngredientUnit.Part;
            switch (ingredientNode.Attributes["Unit"].Value)
            {
                case "(ice)":
                case "ice":
                    unit = IngredientUnit.Ice;
                    break;
                case "(piece)":
                case "piece":
                    unit = IngredientUnit.Piece;
                    break;
                case "part":
                    unit = IngredientUnit.Part;
                    break;
                case "dash":
                    unit = IngredientUnit.Dash;
                    break;
                case "teaspoon":
                    unit = IngredientUnit.Teaspoon;
                    break;
                case "splash":
                    unit = IngredientUnit.Splash;
                    break;
                case "(whipped_cream)":
                    unit = IngredientUnit.Whipped_Cream;
                    break;
                case "pint":
                    unit = IngredientUnit.Pint;
                    break;
                case "scoop":
                    unit = IngredientUnit.Scoop;
                    break;
                case "half":
                    unit = IngredientUnit.Half;
                    break;
                case "pinch":
                    unit = IngredientUnit.Pinch;
                    break;
                case "drop":
                    unit = IngredientUnit.Drop;
                    break;
                case "bottle":
                    unit = IngredientUnit.Bottle;
                    break;
                case "litre":
                    unit = IngredientUnit.Litre;
                    break;
                default:
                    break;
            }
            if (ingredientNode.LocalName.Equals("Ingredient"))
            {
                if (!userNameLookup.Keys.Contains(ingredientNode.Attributes["Key"].Value))
                {
                    return null;
                }
                return new Ingredient(ingredientNode.Attributes["Key"].Value, Double.Parse(ingredientNode.Attributes["Quantity"].Value), unit, userNameLookup[ingredientNode.Attributes["Key"].Value]);
            }
            else
            {
                List<IngredientChoice> choices = new List<IngredientChoice>();
                foreach (XmlNode choice in ingredientNode.ChildNodes)
                {
                    if (!userNameLookup.Keys.Contains(choice.Attributes["Key"].Value))
                    {
                        return null;
                    }
                    choices.Add(new IngredientChoice(choice.Attributes["Key"].Value, userNameLookup[choice.Attributes["Key"].Value]));
                }
                return new Ingredient(ingredientNode.Attributes["Key"].Value, choices, Double.Parse(ingredientNode.Attributes["Quantity"].Value), unit, ingredientNode.Attributes["Key"].Value);
            }
        }

        private static double ConvertToMilliliters(double amount, IngredientUnit unit, double partAmount)
        {
            switch (unit)
            {
                case IngredientUnit.Dash:
                    return amount * 1;
                case IngredientUnit.Teaspoon:
                    return amount * 5;
                case IngredientUnit.Splash:
                    return amount * 25;
                case IngredientUnit.Pint:
                    return amount * 473.18;
                case IngredientUnit.Drop:
                    return amount * 0.5;
                case IngredientUnit.Litre:
                    return amount * 1000;
                case IngredientUnit.Part:
                    return amount * partAmount;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
