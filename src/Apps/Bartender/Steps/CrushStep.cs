﻿using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class CrushStep : RecipeStep<Ingredient>
    {
        public CrushStep()
            : base("Crush")
        {
        }

        public override void DoStep(Dialog userInput, IBartenderController controller, Ingredient ingredient)
        {
            if (userInput == null || String.IsNullOrEmpty(userInput.GetPropertyValue("answer")))
            {
                MessageToUser = String.Format("Crush {0} and add to the glass and tell me when you are done.", ingredient.Name);
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
            CrushStep step = new CrushStep();
            return CopyValuesToClone(step);
        }
    }
}
