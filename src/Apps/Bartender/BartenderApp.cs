using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Bartender
{
    [ParseableElement("Drinks", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class BartenderApp : VoiceActivatedApp<BartenderConversation>
    {
        public const String CommandName = "Bartender";
        public const String ConfirmationRuleName = CommandName + "_confirmation";

        private DrinkRecipeProvider _recipeProvider;

        private IEnumerable<DrinkRecipe> AvailableDrinks
        {
            get
            {
                if (ConfigManager != null && ConfigManager.FindAllComponentsOfType<IBartenderController>().Any())
                {
                    IEnumerable<String> availableDrinks = ConfigManager.FindAllComponentsOfType<IBartenderController>().SelectMany(s => s.AvailableLiquids).ToList();
                    return _recipeProvider.Drinks.Where(drink => drink.CanMakeFromRecipes(availableDrinks));
                }
                return new List<DrinkRecipe>();
            }
        }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && AvailableDrinks.Any();
            }
        }

        public override string InvalidMessage
        {
            get
            {
                return "No bartenders are available to use.";
            }
        }

        public BartenderApp()
            : this(new DrinkRecipeProvider("drinks.xml"))
        {
        }

        public BartenderApp(DrinkRecipeProvider provider)
            : base()
        {
            _recipeProvider = provider;
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (ConfigManager.FindAllComponentsOfType<IBartenderController>().Any() &&
                AvailableDrinks.Any())
            {
                List<String> drinkNames = AvailableDrinks.Select(d => d.Name).ToList();
                XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("BartenderGrammar.grxml", GetType()), "DrinkName", drinkNames);
                Provider = new GrammarProvider(grammar, GrammarUtility.GetGenericTrueFalseGrammar(ConfirmationRuleName, CommandName, new List<String> { "o k" }, new List<String> { "cancel" }));
            }
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new BartenderConversation(ConfigManager, _recipeProvider);
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }
    }
}
