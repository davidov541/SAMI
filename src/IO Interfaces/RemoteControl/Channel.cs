using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Remote
{
    [ParseableElement("Channel", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class Channel : IParseable
    {
        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public String Name
        {
            get;
            private set;
        }

        public int Number
        {
            get;
            private set;
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get 
            {
                yield return new PersistentProperty("Name", () => Name, n => Name = n);
                yield return new PersistentProperty("Number", () => Number.ToString(), n => Number = Int32.Parse(n));
            }
        }

        public IEnumerable<IParseable> Children
        {
            get 
            {
                yield break;
            }
        }

        public Channel()
        {
        }

        public void Initialize(IConfigurationManager manager)
        {
        }

        public void Dispose()
        {
        }

        public void AddChild(IParseable component)
        {
        }
    }
}
