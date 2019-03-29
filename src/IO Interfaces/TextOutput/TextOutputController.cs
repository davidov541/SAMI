using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.TextOutput
{
    [ParseableElement("TextOutput", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class TextOutputController : ITextOutputController
    {
        public event EventHandler<SpeechOutputAddedEventArgs> SpeechOutputAdded;

        public String Name
        {
            get
            {
                return "Console";
            }
        }

        /// <inheritdoc />
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

        public TextOutputController()
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

        public void OutputText(string output)
        {
            if (SpeechOutputAdded != null)
            {
                SpeechOutputAdded(this, new SpeechOutputAddedEventArgs(output));
            }
        }
    }
}
