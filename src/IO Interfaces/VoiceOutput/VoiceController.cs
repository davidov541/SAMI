using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Speech.Synthesis;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Speech
{
    [ParseableElement("VoiceOutput", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class VoiceController : IOutputController
    {
        private SpeechSynthesizer _synth;
        private bool _isSpeaking;

        private const String win8Voice = "Microsoft Zira Desktop";

        public String Name
        {
            get
            {
                return win8Voice;
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

        public VoiceController()
        {
            _synth = new SpeechSynthesizer();
            if (_synth.GetInstalledVoices().Any(voice => voice.VoiceInfo.Name.Equals(win8Voice)))
            {
                _synth.SelectVoice(win8Voice);
            }
            _synth.SetOutputToDefaultAudioDevice();
            _synth.SpeakCompleted += _synth_SpeakCompleted;
            _isSpeaking = false;
        }

        private void _synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            _isSpeaking = false;
        }

        public void OutputText(String speech)
        {
            _isSpeaking = true;
            _synth.SpeakAsync(speech);
        }

        public void Initialize(IConfigurationManager manager)
        {
        }

        public void Dispose()
        {
            _synth.Dispose();
        }

        public void AddChild(IParseable component)
        {
        }
    }
}
