using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Speech.Recognition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Speech
{
    /// <summary>
    /// In charge of initializing Kinect and getting speech recognition data from it.
    /// </summary>
    [ParseableElement("SimulatedKinect", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class SimulatedKinectSensor : BaseKinectSensor
    {
        public SimulatedKinectSensor()
            : base()
        {
        }

        public override void Dispose()
        {
        }

        public override void CheckForUpdates()
        {
            if (_updateGrammars)
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
                RecognizerInfo ri = GetKinectRecognizer();

                // Create a SpeechRecognitionEngine object and add the grammars to it.
                _recoEngine = new SpeechRecognitionEngine(ri.Id);
                _recoEngine.LoadGrammar(_grammars.Single(gram => gram.Name.Equals(GrammarName)));

                _recoEngine.SpeechRecognized += recoEngine_SpeechRecognized;
            }
        }

        protected override void Stop()
        {
            if(_recoEngine != null)
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
            RecognitionResult result = e.Result;
            RecognizeSpeech(result);
        }
    }
}
