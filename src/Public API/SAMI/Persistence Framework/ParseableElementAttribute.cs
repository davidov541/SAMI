using System;
using System.ComponentModel.Composition;

namespace SAMI.Persistence
{
    /// <summary>
    /// Attribute which indicates when a class can be parsed by the persistence framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [MetadataAttribute]
    public class ParseableElementAttribute : ExportAttribute, IParseableMetadata
    {
        /// <summary>
        /// Name of the attribute. This should match the name of the element in the XML file.
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of parseable element.
        /// </summary>
        public ParseableElementType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor for the attribute.
        /// </summary>
        /// <param name="name">Value for <see cref="Name"/></param>
        /// <param name="type">Value for <see cref="Type"/></param>
        public ParseableElementAttribute(String name, ParseableElementType type)
            : base(name, typeof(IParseable))
        {
            Name = name;
            Type = type;
        }
    }
}
