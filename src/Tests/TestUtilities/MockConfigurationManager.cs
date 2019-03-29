using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Test.Utilities
{
    internal class MockConfigurationManager : IInternalConfigurationManager
    {
        public event EventHandler InitializationComplete;

        private List<IParseable> _components = new List<IParseable>();

        public Location LocalLocation
        {
            get 
            {
                return new Location("Austin", "Texas", 78759);
            }
        }

        public string XBeeCOM
        {
            get
            {
                return "XBeeCOM";
            }
        }

        public string ZWaveCOM
        {
            get
            {
                return "ZWaveCOM";
            }
        }

        public IEnumerable<T> FindAllComponentsOfType<T>() where T : IParseable
        {
            return _components.OfType<T>();
        }

        public string GetPathForFile(string fileName, Type owningType)
        {
            String assemblyLocation = Assembly.GetAssembly(owningType).Location;
            String assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(assemblyDirectory, fileName);
        }

        public void AddComponent(IParseable component)
        {
            _components.Add(component);
        }

        public IEnumerable<T> FindAllComponentsOfTypeEvenInvalid<T>() where T : IParseable
        {
            return _components.OfType<T>();
        }

        public void SaveConfiguration()
        {
        }

        public void SaveConfiguration(string fileName)
        {
        }
    }
}
