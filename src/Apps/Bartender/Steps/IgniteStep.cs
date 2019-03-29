using System;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class IgniteStep : RecipeStep
    {
        public IgniteStep()
            : base("Ignite")
        {
        }

        public override void DoStep(Dialog userInput, IBartenderController controller)
        {
            if (userInput == null || String.IsNullOrEmpty(userInput.GetPropertyValue("answer")))
            {
                MessageToUser = String.Format("Ignite the drink.");
                NextGrammarNeeded = BartenderApp.ConfirmationRuleName;
                IsStepDone = false;
                ShouldCancel = false;
            }
            else
            {
                IsStepDone = true;
                ShouldCancel = !Boolean.Parse(userInput.GetPropertyValue("answer"));
                MessageToUser = null;
                NextGrammarNeeded = null;
            }
        }

        public override object Clone()
        {
            IgniteStep step = new IgniteStep();
            return CopyValuesToClone(step);
        }
    }
}
