using System;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Bartender
{
    internal class RecipeStepCall : ICloneable
    {
        internal RecipeStep _step;
        internal RecipeStepCall _nextStep;

        public bool ShouldCancel
        {
            get
            {
                return _step == null ? false : _step.ShouldCancel;
            }
        }

        public String NextGrammarNeeded
        {
            get
            {
                return _step == null ? String.Empty : _step.NextGrammarNeeded;
            }
        }

        public String MessageToUser
        {
            get
            {
                return _step == null ? String.Empty : _step.MessageToUser;
            }
        }

        public bool IsDone
        {
            get
            {
                return _step != null && _step.IsStepDone && _nextStep == null;
            }
        }

        public bool IsValid
        {
            get
            {
                return _step != null && (_nextStep == null || _nextStep.IsValid);
            }
        }

        protected RecipeStepCall()
            : this(null)
        {
        }

        public RecipeStepCall(RecipeStep step)
            : this(step, null)
        {
        }

        public RecipeStepCall(RecipeStep step, RecipeStepCall nextStep)
        {
            _step = step;
            _nextStep = nextStep;
        }

        public RecipeStepCall Call(Dialog userResponse, IBartenderController controller)
        {
            CallInternal(userResponse, controller);
            if (!_step.IsStepDone || ShouldCancel || _nextStep == null)
            {
                return this;
            }
            return _nextStep.Call(null, controller);
        }

        protected virtual void CallInternal(Dialog userResponse, IBartenderController controller)
        {
            _step.DoStep(userResponse, controller);
        }

        public void AddNextStep(RecipeStepCall step)
        {
            _nextStep = step;
        }

        public virtual object Clone()
        {
            RecipeStepCall clone = new RecipeStepCall();
            return CopyValuesToClone(clone);
        }

        protected RecipeStepCall CopyValuesToClone(RecipeStepCall clone)
        {
            clone._step = _step.Clone() as RecipeStep;
            if(_nextStep != null)
            {
                clone._nextStep = _nextStep.Clone() as RecipeStepCall;
            }
            return clone;
        }

        public override bool Equals(object obj)
        {
            RecipeStepCall call = obj as RecipeStepCall;
            if(call == null)
            {
                return false;
            }
            else if(call._nextStep == null && _nextStep != null)
            {
                return false;
            }
            else if(call._nextStep != null && !call._nextStep.Equals(_nextStep))
            {
                return false;
            }
            else if(call._step == null && _step != null)
            {
                return false;
            }
            else if(call._step != null && !call._step.Equals(_step))
            {
                return false;
            }
            return true;
        }
    }
}
