using System;

namespace SAMI.Persistence
{
    /// <summary>
    /// Represents a property that will be persisted by a class in the configuration file.
    /// </summary>
    public class PersistentProperty
    {
        /// <summary>
        /// Name of the property that should be persisted.
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// A function which gets a string representing the value of this property.
        /// </summary>
        public Func<String> Getter
        {
            get;
            private set;
        }

        /// <summary>
        /// A function which sets the string to a given value.
        /// The value is passed in as a string representation.
        /// </summary>
        public Action<String> Setter
        {
            get;
            private set;
        }

        /// <summary>
        /// Basic constructor for PersistentProperty.
        /// </summary>
        /// <param name="name">Value of <see cref="Name"/></param>
        /// <param name="getter">Value of <see cref="Getter"/></param>
        /// <param name="setter">Value of <see cref="Setter"/></param>
        public PersistentProperty(String name, Func<String> getter, Action<String> setter)
        {
            Name = name;
            Getter = getter;
            Setter = setter;
        }
    }
}
