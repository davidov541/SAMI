using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Apps.Remote
{
    internal abstract class RemoteReference : IParseable
    {
        public String RemoteName
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get 
            {
                return true;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get 
            {
                yield return new PersistentProperty("RemoteName", () => RemoteName, n => RemoteName = n);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get 
            {
                yield break;
            }
        }

        public RemoteReference()
        {
        }

        public void Initialize(IConfigurationManager configManager)
        {
        }

        public void AddChild(IParseable child)
        {
        }

        public void Dispose()
        {
        }
    }
}
