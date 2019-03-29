using System;

namespace SAMI.Apps.Bartender
{
    internal class RecipeStepFactory
    {
        public RecipeStepCall GetRecipeStepCall<T>(String stepType, T argument)
        {
            RecipeStep<T> step = GetStep(stepType) as RecipeStep<T>;
            return new RecipeStepCall<T>(step, argument);
        }

        public RecipeStepCall GetRecipeStepCall(String stepType)
        {
            RecipeStep step = GetStep(stepType);
            return new RecipeStepCall(step);
        }

        public RecipeStep GetInitialStep(String drinkName, String glassName)
        {
            return new InitialStep(drinkName, glassName);
        }

        private RecipeStep GetStep(String stepType)
        {
            switch (stepType)
            {
                case "Add":
                    return new AddStep();
                case "Pour":
                    return new PourStep();
                case "Shake":
                    return new ShakeStep();
                case "Strain":
                    return new StrainStep();
                case "Crush":
                    return new CrushStep();
                case "Ignite":
                    return new IgniteStep();
                case "Garnish":
                    return new GarnishStep();
                case "Stir":
                    return new StirStep();
                case "Blend":
                    return new BlendStep();
                default:
                    break;
            }
            return null;
        }
    }
}
