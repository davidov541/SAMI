using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class StirStep : RecipeStep
    {
        public StirStep()
            : base("Stir")
        {
        }

        public override void DoStep(Dialog userInput, IBartenderController controller)
        {
            if (userInput == null || String.IsNullOrEmpty(userInput.GetPropertyValue("answer")))
            {
                MessageToUser = String.Format("Stir the drink.");
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
            StirStep step = new StirStep();
            return CopyValuesToClone(step);
        }
    }
}
