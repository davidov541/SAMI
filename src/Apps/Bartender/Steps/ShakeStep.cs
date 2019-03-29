using System;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class ShakeStep : RecipeStep
    {
        private enum ShakeStepState
        {
            Start,
            HasSwitchedGlasses,
            HasShaken,
            HasReturnedToGlass,
        }

        private ShakeStepState _state;

        public ShakeStep()
            : base("Shake")
        {
            _state = ShakeStepState.Start;
        }

        public override void DoStep(Dialog userInput, IBartenderController controller)
        {
            switch (_state)
            {
                case ShakeStepState.Start:
                    MessageToUser = String.Format("Pour the drink into a shaker.");
                    NextGrammarNeeded = BartenderApp.ConfirmationRuleName;
                    IsStepDone = false;
                    ShouldCancel = false;
                    _state = ShakeStepState.HasSwitchedGlasses;
                    break;
                case ShakeStepState.HasSwitchedGlasses:
                    ShouldCancel = !Boolean.Parse(userInput.GetPropertyValue("answer"));
                    IsStepDone = ShouldCancel;
                    MessageToUser = ShouldCancel ? null : String.Format("Shake the drink.");
                    NextGrammarNeeded = ShouldCancel ? null : BartenderApp.ConfirmationRuleName;
                    _state = ShakeStepState.HasShaken;
                    break;
                case ShakeStepState.HasShaken:
                    ShouldCancel = !Boolean.Parse(userInput.GetPropertyValue("answer"));
                    IsStepDone = ShouldCancel;
                    MessageToUser = ShouldCancel ? null : String.Format("Pour the drink back into the glass.");
                    NextGrammarNeeded = ShouldCancel ? null : BartenderApp.ConfirmationRuleName;
                    _state = ShakeStepState.HasReturnedToGlass;
                    break;
                case ShakeStepState.HasReturnedToGlass:
                    IsStepDone = true;
                    ShouldCancel = !Boolean.Parse(userInput.GetPropertyValue("answer"));
                    MessageToUser = null;
                    NextGrammarNeeded = null;
                    break;
                default:
                    break;
            }
        }

        public override object Clone()
        {
            ShakeStep step = new ShakeStep();
            return CopyValuesToClone(step);
        }
    }
}
