using SAMI.Apps;
using SAMI.Configuration;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Base class for SAMI apps which are activated through voice.
    /// This class adds functionality that is needed for speech recognition.
    /// This is the preferred class to inherit from for all apps, if you do not
    /// have a custom conversation class to use.
    /// </summary>
    public abstract class VoiceActivatedApp : BaseSAMIApp
    {
        private bool _hasSyncedProvider = false;
        /// <summary>
        /// Only internal for testing purposes. Should not be directly used.
        /// </summary>
        internal GrammarProvider _provider;
        /// <summary>
        /// The grammar provider that this app uses in order to indicate
        /// what grammars the app recognizes.
        /// This provider should be set in the constructor of the app.
        /// </summary>
        protected GrammarProvider Provider
        {
            get
            {
                return _provider;
            }
            set
            {
                if (ConfigManager != null && _hasSyncedProvider)
                {
                    if (Provider != null)
                    {
                        foreach (IVoiceSensor sensor in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
                        {
                            sensor.RemoveGrammarProvider(Provider);
                        }
                    }
                    _provider = value;
                    SyncGrammarProvider();
                }
                else
                {
                    _provider = value;
                }
            }
        }

        private void SyncGrammarProvider()
        {
            foreach (IVoiceSensor sensor in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
            {
                sensor.AddGrammarProvider(Provider);
            }
            _hasSyncedProvider = true;
        }

        public VoiceActivatedApp()
            : base()
        {
        }

        /// <summary>
        /// Called once all IParseable instances have been created from the configuration file.
        /// This is called on the parent element before being called on the children elements.
        /// </summary>
        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (Provider != null && ConfigManager != null)
            {
                SyncGrammarProvider();
            }
            _hasSyncedProvider = true;
        }
    }
}
