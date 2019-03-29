using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class RecipeStepCall<T> : RecipeStepCall
    {
        internal T _argument;

        protected RecipeStepCall()
            : base()
        {
        }

        public RecipeStepCall(RecipeStep<T> step, T argument)
            : base(step)
        {
            _argument = argument;
        }

        protected override void CallInternal(Dialog userResponse, IBartenderController controller)
        {
            (_step as RecipeStep<T>).DoStep(userResponse, controller, _argument);
        }

        public override object Clone()
        {
            RecipeStepCall<T> clone = new RecipeStepCall<T>();
            clone._argument = _argument;
            return CopyValuesToClone(clone);
        }
    }
}
