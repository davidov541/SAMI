using System;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class StrainStep : RecipeStep
    {
        public StrainStep()
            : base("Strain")
        {
        }

        public override void DoStep(Dialog userInput, IBartenderController controller)
        {
            if (userInput == null || String.IsNullOrEmpty(userInput.GetPropertyValue("answer")))
            {
                MessageToUser = String.Format("Strain the drink.");
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
            StrainStep step = new StrainStep();
            return CopyValuesToClone(step);
        }
    }
}
