using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Speech.Recognition;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Speech.KinectTwo
{
    /// <summary>
    /// In charge of initializing Kinect and getting speech recognition data from it.
    /// </summary>
    [ParseableElement("Microphone", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class BuiltInVoiceSensor : BaseSpeechRecognitionSensor
    {
        public BuiltInVoiceSensor()
            : base()
        {
        }

        public override void Dispose()
        {
        }

        public override void CheckForUpdates()
        {
            if (_updateGrammars && IsValid)
            {
                Stop();
                UpdateGrammars();
                Start();
            }
        }

        protected override void Start()
        {
            if (_grammarCollection.IsValidGrammar)
            {
                RecognizerInfo ri = GetVoiceRecognizer();

                // Create a SpeechRecognitionEngine object and add the grammars to it.
                _recoEngine = new SpeechRecognitionEngine(ri.Id);
                _recoEngine.LoadGrammar(_grammars.Single(gram => gram.Name.Equals(GrammarName)));

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                _recoEngine.UpdateRecognizerSetting("AdaptationOn", 0);
                _recoEngine.SetInputToDefaultAudioDevice();

                _recoEngine.SpeechRecognized += recoEngine_SpeechRecognized;

                _recoEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        protected override void Stop()
        {
            if (_recoEngine != null)
            {
                try
                {
                    _recoEngine.RecognizeAsyncCancel();
                }
                catch (InvalidOperationException)
                {
                    // This means we were already cancelled due to timeout.
                    // We need to figure out in what cases this happens, but
                    // until then, we can just ignore.
                }
                _recoEngine.Dispose();
                _recoEngine = null;
            }
        }

        public void recoEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (IsValid)
            {
                RecognitionResult result = e.Result;
                RecognizeSpeech(result);
            }
        }
    }
}
