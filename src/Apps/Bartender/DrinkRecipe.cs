using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class DrinkRecipe
    {
        public String Name
        {
            get;
            private set;
        }

        public List<Ingredient> Ingredients
        {
            get;
            private set;
        }

        public RecipeStepCall FirstStep
        {
            get;
            set;
        }

        public static DrinkRecipe CreateFromXml(XmlNode recipeNode, Dictionary<String, String> userNameLookup, Dictionary<String, double> glassVolumes)
        {
            RecipeStepFactory factory = new RecipeStepFactory();
            DrinkRecipe recipe = new DrinkRecipe(recipeNode.Attributes["Name"].Value);
            // Determine total number of parts, static volume
            double totalParts = 0;
            double staticVolume = 0;

            if (!glassVolumes.Keys.Contains(recipeNode.Attributes["Glass"].Value))
            {
                return null;
            }
            double drinkVolume = glassVolumes[recipeNode.Attributes["Glass"].Value];

            foreach (XmlNode ingredient in recipeNode.SelectNodes("Ingredients/*"))
            {
                Ingredient ingred = Ingredient.CreateFromXml(ingredient, userNameLookup);
                if (ingred != null)
                {
                    recipe.Ingredients.Add(ingred);

                    // Get information so we can determine how much a part really is for this recipe.
                    if (ingred.AmountUnit == IngredientUnit.Part)
                    {
                        totalParts += ingred.AmountQuantity;
                    }
                    else
                    {
                        staticVolume += ingred.AmountQuantityInmL;
                    }
                }
                else
                {
                    return null;
                }
            }
            double partVolume = (drinkVolume - staticVolume) / totalParts;
            if (partVolume < 0)
            {
                partVolume = drinkVolume / totalParts;
            }
            else if (partVolume == Double.PositiveInfinity)
            {
                partVolume = 0;
            }

            recipe.Ingredients.ForEach(i => i.SetPartAmount(partVolume));

            List<RecipeStepCall> calls = new List<RecipeStepCall>();
            calls.Add(new RecipeStepCall(factory.GetInitialStep(recipe.Name, recipeNode.Attributes["Glass"].Value)));
            foreach (XmlElement step in recipeNode.SelectNodes("Steps/Step").OfType<XmlElement>())
            {
                if (step.HasAttribute("Arguments"))
                {
                    foreach (String arg in step.Attributes["Arguments"].Value.Split(','))
                    {
                        if (recipe.Ingredients.Any(i => i.Key.Equals(arg)))
                        {
                            calls.Add(factory.GetRecipeStepCall(step.Attributes["Type"].Value, recipe.Ingredients.SingleOrDefault(i => i.Key.Equals(arg))));
                        }
                        else
                        {
                            calls.Add(factory.GetRecipeStepCall(step.Attributes["Type"].Value, arg.Replace('_', ' ')));
                        }
                    }
                }
                else
                {
                    calls.Add(factory.GetRecipeStepCall(step.Attributes["Type"].Value));
                }
            }

            calls.Reverse();
            foreach (RecipeStepCall call in calls)
            {
                call.AddNextStep(recipe.FirstStep);
                recipe.FirstStep = call;
            }
            return recipe;
        }

        private static double GetDrinkVolume(String glassName)
        {
            switch (glassName)
            {
                case "Champagne":
                    return 200;
                case "Cocktail":
                    return 250;
                case "Old Fashioned":
                    return 250;
                case "Collins":
                    return 400;
                case "Hurricane":
                    return 300;
                case "Sour":
                    return 120;
                case "Shooter":
                    return 25;
                case "Irish Coffee Glass":
                    return 177;
                case "Snifter":
                    return 350;
                default:
                    return 400;
            }
        }

        public DrinkRecipe(String name)
        {
            Name = name;
            Ingredients = new List<Ingredient>();
        }

        public bool CanMakeFromRecipes(IEnumerable<String> dispensers)
        {
            return Ingredients.TrueForAll(ingred => ingred.GetAvailableKeys(dispensers).Any() || ingred.AmountUnit == IngredientUnit.Ice || ingred.AmountUnit == IngredientUnit.Whipped_Cream) &&
                FirstStep.IsValid;
        }

        public RecipeStepCall Start()
        {
            return FirstStep.Clone() as RecipeStepCall;
        }

        public override bool Equals(object obj)
        {
            DrinkRecipe other = obj as DrinkRecipe;
            if (other == null)
            {
                return false;
            }
            else if (other.FirstStep == null && FirstStep != null)
            {
                return false;
            }
            else if (other.FirstStep != null && !other.FirstStep.Equals(FirstStep))
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
            else if (other.Ingredients.Count != Ingredients.Count)
            {
                return false;
            }

            foreach (Ingredient ingredient in Ingredients)
            {
                if (!other.Ingredients.Contains(ingredient))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
