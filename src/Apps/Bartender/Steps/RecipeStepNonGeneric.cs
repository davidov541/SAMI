using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class RecipeStep : ICloneable
    {
        public String Name
        {
            get;
            protected set;
        }

        public bool ShouldCancel
        {
            get;
            protected set;
        }

        public String MessageToUser
        {
            get;
            protected set;
        }

        public String NextGrammarNeeded
        {
            get;
            protected set;
        }

        public bool IsStepDone
        {
            get;
            protected set;
        }

        public RecipeStep(String name)
        {
            Name = name;
            ShouldCancel = false;
            IsStepDone = false;
        }

        public virtual void DoStep(Dialog d, IBartenderController controller)
        {
            IsStepDone = true;
            ShouldCancel = false;
            MessageToUser = null;
            NextGrammarNeeded = null;
        }

        public virtual object Clone()
        {
            RecipeStep step = new RecipeStep(Name);
            return CopyValuesToClone(step);
        }

        protected RecipeStep CopyValuesToClone(RecipeStep step)
        {
            step.IsStepDone = IsStepDone;
            step.ShouldCancel = ShouldCancel;
            step.MessageToUser = MessageToUser;
            step.NextGrammarNeeded = NextGrammarNeeded;
            return step;
        }

        public override bool Equals(object obj)
        {
            RecipeStep step = obj as RecipeStep;
            if(step == null)
            {
                return false;
            }
            else if(!step.IsStepDone.Equals(IsStepDone))
            {
                return false;
            }
            else if(step.MessageToUser == null && MessageToUser != null)
            {
                return false;
            }
            else if(MessageToUser != null && !step.MessageToUser.Equals(MessageToUser))
            {
                return false;
            }
            else if(!step.Name.Equals(Name))
            {
                return false;
            }
            else if(step.NextGrammarNeeded == null && NextGrammarNeeded != null)
            {
                return false;
            }
            else if(NextGrammarNeeded != null && !step.NextGrammarNeeded.Equals(NextGrammarNeeded))
            {
                return false;
            }
            else if(!step.ShouldCancel.Equals(ShouldCancel))
            {
                return false;
            }
            return true;
        }
    }
}
