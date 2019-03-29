using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Test.Utilities
{
    public class MockVoiceControl : IVoiceSensor
    {
        public GrammarProvider GrammarProvider
        {
            get;
            private set;
        }

        public string GrammarName
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return "Test Voice Control";
            }
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
                yield break;
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public event EventHandler<RecognizedNewPhraseEventArgs> RecognizedNewPhrase;

        public Dialog GetDialogForInput(string input)
        {
            return null;
        }

        public void SimulateInput(string input)
        {
        }

        public void AddGrammarProvider(GrammarProvider provider)
        {
            GrammarProvider = provider;
        }

        public void RemoveGrammarProvider(GrammarProvider provider)
        {
            GrammarProvider = null;
        }

        public void CheckForUpdates()
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
