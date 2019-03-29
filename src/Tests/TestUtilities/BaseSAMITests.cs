using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Test.Utilities
{
    public class BaseSAMITests
    {
        private MockConfigurationManager _configMan;

        protected IConfigurationManager GetConfigurationManager()
        {
            if(_configMan == null)
            {
                _configMan = new MockConfigurationManager();
            }
            return _configMan;
        }

        protected void AddComponentToConfigurationManager(IParseable component)
        {
            (GetConfigurationManager() as MockConfigurationManager).AddComponent(component);
        }
    }
}
