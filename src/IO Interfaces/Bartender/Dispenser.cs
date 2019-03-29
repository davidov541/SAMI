using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Bartender
{
    [ParseableElement("Dispenser", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class Dispenser : IParseable
    {
        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public String Key
        {
            get;
            private set;
        }

        public int DispenserIndex
        {
            get;
            private set;
        }

        public String MillilitersPerSecond
        {
            get;
            private set;
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("Key", () => Key, k => Key = k);
                yield return new PersistentProperty("DispenserIndex", () => DispenserIndex.ToString(), k => DispenserIndex = Int32.Parse(k));
                yield return new PersistentProperty("MillilitersPerSecond", () => MillilitersPerSecond, k => MillilitersPerSecond = k);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public Dispenser()
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
