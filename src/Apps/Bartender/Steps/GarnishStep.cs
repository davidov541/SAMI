using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class GarnishStep : RecipeStep<String>
    {
        public GarnishStep()
            : base("Garnish")
        {
        }

        public override void DoStep(Dialog userInput, IBartenderController controller, String garnish)
        {
            MessageToUser = String.Format("Garnish the drink with {0}.", garnish);
            NextGrammarNeeded = BartenderApp.ConfirmationRuleName;
            IsStepDone = true;
            ShouldCancel = false;
        }

        public override object Clone()
        {
            GarnishStep step = new GarnishStep();
            return CopyValuesToClone(step);
        }
    }
}
