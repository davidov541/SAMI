using System;
using System.Linq;
using SAMI.IOInterfaces.Interfaces.Voice;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;

namespace SAMI.Apps.Bartender
{
    internal class BartenderConversation : Conversation
    {
        protected override string CommandName
        {
            get
            {
                return BartenderApp.CommandName;
            }
        }

        public override string GrammarRuleName
        {
            get
            {
                if (_nextGrammarName == null)
                {
                    return base.GrammarRuleName;
                }
                else
                {
                    return _nextGrammarName;
                }
            }
        }

        private RecipeStepCall _call = null;
        private String _drinkBeingMade = null;
        private String _nextGrammarName = null;
        private bool _isCleaningPumps = false;
        private IBartenderController _controllerInUse = null;
        private DrinkRecipeProvider _recipeProvider = null;

        public BartenderConversation(IConfigurationManager configManager, DrinkRecipeProvider provider)
            : base(configManager)
        {
            _recipeProvider = provider;
        }

        public override string Speak()
        {
            base.Speak();
            Dialog phrase = CurrentDialog;
            String subCommand = phrase.GetPropertyValue("Subcommand");
            IEnumerable<IBartenderController> controllers = ConfigManager.FindAllComponentsOfType<IBartenderController>();
            if (!controllers.Any())
            {
                ConversationIsOver = true;
                return String.Empty;
            }
            else if (String.IsNullOrEmpty(subCommand) && _isCleaningPumps)
            {
                ConversationIsOver = true;
                if (Boolean.Parse(phrase.GetPropertyValue("answer")))
                {
                    foreach (IBartenderController controller in controllers)
                    {
                        foreach (String pump in controller.AvailableLiquids)
                        {
                            controller.DispenseLiquid(pump, 25);
                        }
                    }
                    return "Cleaning the pumps now.";
                }
                else
                {
                    return "Never mind.";
                }
            }
            else if (subCommand.Equals("cleanPumps"))
            {
                ConversationIsOver = false;
                _isCleaningPumps = true;
                _nextGrammarName = BartenderApp.ConfirmationRuleName;
                return "Make sure that the pumps are all no longer in the drinks. Pumps will run one at a time until all have been cleaned. Tell me when you are ready.";
            }
            else if (String.IsNullOrEmpty(subCommand))
            {
                ConversationIsOver = false;
                if (_call == null)
                {
                    _drinkBeingMade = phrase.GetPropertyValue("DrinkName");
                    DrinkRecipe recipe = _recipeProvider.Drinks.Single(d => d.Name.Equals(_drinkBeingMade));
                    _controllerInUse = controllers.FirstOrDefault(s => recipe.CanMakeFromRecipes(s.AvailableLiquids));
                    if (_controllerInUse == null)
                    {
                        ConversationIsOver = true;
                        return String.Empty;
                    }
                    _call = recipe.Start();
                }
                _call = _call.Call(phrase, _controllerInUse);
                if (_call.IsDone)
                {
                    ConversationIsOver = true;
                    if (String.IsNullOrEmpty(_call.MessageToUser))
                    {
                        return String.Format("Your {0} is now being made.", _drinkBeingMade);
                    }
                    else
                    {
                        return _call.MessageToUser;
                    }
                }
                else if (_call.ShouldCancel)
                {
                    ConversationIsOver = true;
                    return "Never mind";
                }
                else
                {
                    _nextGrammarName = _call.NextGrammarNeeded;
                    return _call.MessageToUser;
                }
            }
            else if (subCommand.Equals("drinkList"))
            {
                ConversationIsOver = true;
                IEnumerable<String> ingredients = controllers.SelectMany(s => s.AvailableLiquids).ToList();
                IEnumerable<DrinkRecipe> recipes = _recipeProvider.Drinks.Where(drink => drink.CanMakeFromRecipes(ingredients)).ToList();
                return String.Format("I can make {0}.", SayList(recipes.Select(d => d.Name).ToList()));
            }
            else if (subCommand.Equals("ingredientList"))
            {
                ConversationIsOver = true;
                String drinkBeingMade = phrase.GetPropertyValue("DrinkName");
                DrinkRecipe recipe = _recipeProvider.Drinks.Single(d => d.Name.Equals(drinkBeingMade));
                return String.Format("A {0} contains {1}.", recipe.Name, SayList(recipe.Ingredients.Select(d => d.Name).ToList()));
            }
            return String.Empty;
        }
    }
}
