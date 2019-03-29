using System;

namespace SAMI.Persistence
{
    public interface IParseableMetadata
    {
        /// <summary>
        /// Name of the attribute. This should match the name of the element in the XML file.
        /// </summary>
        String Name
        {
            get;
        }

        /// <summary>
        /// The type of parseable element.
        /// </summary>
        ParseableElementType Type
        {
            get;
        }
    }
}
