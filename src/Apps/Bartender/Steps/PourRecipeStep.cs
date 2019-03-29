using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class PourStep : RecipeStep<Ingredient>
    {
        public PourStep()
            : base("Pour")
        {
        }

        public override void DoStep(Dialog userInput, IBartenderController controller, Ingredient ingredient)
        {
            controller.DispenseLiquid(ingredient.GetAvailableKeys(controller.AvailableLiquids).First(), ingredient.AmountQuantityInmL);
            IsStepDone = true;
            ShouldCancel = false;
            MessageToUser = null;
            NextGrammarNeeded = null;
        }

        public override object Clone()
        {
            PourStep step = new PourStep();
            return CopyValuesToClone(step);
        }
    }
}
