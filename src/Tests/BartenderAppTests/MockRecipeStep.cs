using System;
using SAMI.Apps.Bartender;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace BartenderAppTests
{
    internal class MockRecipeStep : RecipeStep
    {
        public bool DoStepIsCalled
        {
            get;
            private set;
        }

        private bool _endAfterCall;
        public MockRecipeStep(String messageToUser, String nextGrammarNeeded, bool isStepDone, bool shouldCancel, bool endAfterCall)
            : base("Mock Recipe Step")
        {
            IsStepDone = isStepDone;
            MessageToUser = messageToUser;
            NextGrammarNeeded = nextGrammarNeeded;
            ShouldCancel = shouldCancel;
            DoStepIsCalled = false;
            _endAfterCall = endAfterCall;
        }

        public override void DoStep(Dialog d, IBartenderController controller)
        {
            if(_endAfterCall || DoStepIsCalled)
            {
                IsStepDone = true;
            }
            else
            {
                IsStepDone = false;
            }
            DoStepIsCalled = true;
        }

        public override object Clone()
        {
            MockRecipeStep step = new MockRecipeStep(MessageToUser, NextGrammarNeeded, IsStepDone, ShouldCancel, _endAfterCall);
            return CopyValuesToClone(step);
        }
    }
}
