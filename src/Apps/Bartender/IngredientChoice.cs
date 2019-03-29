using System;

namespace SAMI.Apps.Bartender
{
    internal class IngredientChoice
    {
        public String Key
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public IngredientChoice(String key, String name)
        {
            Name = name;
            Key = key;
        }

        public override bool Equals(object obj)
        {
            IngredientChoice other = obj as IngredientChoice;
            if (other.Key == null && Key != null)
            {
                return false;
            }
            else if (other.Key != null && !other.Key.Equals(Key))
            {
                return false;
            }
            else if (other.Name == null && Name != null)
            {
                return false;
            }
            else if (other.Name != null && !other.Name.Equals(Name))
            {
                return false;
            }
            return true;
        }
    }
}
