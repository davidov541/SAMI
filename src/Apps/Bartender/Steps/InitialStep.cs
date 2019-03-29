using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class InitialStep : RecipeStep
    {
        private String _drinkName;
        private String _glassName;

        public InitialStep(String drinkName, String glassName)
            : base("Initial")
        {
            _drinkName = drinkName;
            _glassName = glassName;
        }

        public override void DoStep(Dialog userInput, IBartenderController controller)
        {
            if (userInput == null || String.IsNullOrEmpty(userInput.GetPropertyValue("answer")))
            {
                MessageToUser = String.Format("Place a {1} glass underneath the spout of the bartender so I can make you a {0}.", _drinkName, _glassName);
                NextGrammarNeeded = BartenderApp.ConfirmationRuleName;
                IsStepDone = false;
                ShouldCancel = false;
            }
            else
            {
                ShouldCancel = !Boolean.Parse(userInput.GetPropertyValue("answer"));
                IsStepDone = true;
                MessageToUser = null;
                NextGrammarNeeded = null;
            }
        }

        public override object Clone()
        {
            InitialStep step = new InitialStep(_drinkName, _glassName);
            return CopyValuesToClone(step);
        }

        public override bool Equals(object obj)
        {
            InitialStep step = obj as InitialStep;
            if(!base.Equals(obj))
            {
                return false;
            }
            else if(step == null)
            {
                return false;
            }
            else if(!step._drinkName.Equals(_drinkName))
            {
                return false;
            }
            return true;
        }
    }
}
