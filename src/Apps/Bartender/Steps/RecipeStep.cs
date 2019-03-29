using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class RecipeStep<T> : RecipeStep
    {
        public RecipeStep(String name)
            : base(name)
        {
        }

        public virtual void DoStep(Dialog d, IBartenderController controller, T argument)
        {
            base.DoStep(d, controller);
        }

        public override object Clone()
        {
            RecipeStep<T> step = new RecipeStep<T>(Name);
            return CopyValuesToClone(step);
        }
    }
}
