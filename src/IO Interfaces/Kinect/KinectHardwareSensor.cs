using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using SAMI.Configuration;
using SAMI.Logging;
using SAMI.Persistence;
using Timer = System.Timers.Timer;

namespace SAMI.IOInterfaces.Speech
{
    /// <summary>
    /// In charge of initializing Kinect and getting speech recognition data from it.
    /// </summary>
    [ParseableElement("Kinect", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class KinectHardwareSensor : BaseKinectSensor
    {
        private KinectSensor _sensor;
        private Stream _inputstream;
        private bool _kinectActive = false;
        private Timer _resetTimer;
        private readonly TimeSpan _resetInterval = TimeSpan.FromMinutes(30);

        private bool _resetKinect = false;
        private bool _kinectDriverInstalled = true;

        public KinectHardwareSensor()
            : base()
        {
        }

        protected override void InitializeWithControllers()
        {
            _resetTimer = new Timer(_resetInterval.TotalMilliseconds);
            _resetTimer.Elapsed += ResetTimer_Elapsed;
            _resetTimer.AutoReset = true;
            _resetTimer.Start();

            _sensor = (from sensorToCheck in KinectSensor.KinectSensors where sensorToCheck.Status == KinectStatus.Connected select sensorToCheck).FirstOrDefault();
            if (_sensor == null)
            {
                Console.WriteLine(
                        "No Kinect sensors are attached to this computer or none of the ones that are\n" +
                        "attached are \"Connected\".\n" +
                        "Attach the KinectSensor and restart this application.\n" +
                        "If that doesn't work run SkeletonViewer-WPF to better understand the Status of\n" +
                        "the Kinect sensors.\n\n" +
                        "Press any key to continue.\n");
                return;
            }
            _sensor.Start();
        }

        public override void Dispose()
        {
            if (IsValid)
            {
                if (_inputstream != null)
                {
                    _inputstream.Dispose();
                }
                _recoEngine.Dispose();
                if (_sensor != null)
                {
                    _sensor.Stop();
                }
            }
        }

        protected override void Start()
        {
            if (_grammarCollection.IsValidGrammar)
            {
                SetupRecognitionEngine();

                _recoEngine.SpeechRecognized += recoEngine_SpeechRecognized;
                _recoEngine.RecognizeCompleted += recoEngine_RecognizeCompleted;

                _kinectActive = true;

                if (_sensor != null)
                {
                    _recoEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }

        private void SetupRecognitionEngine()
        {
            if (_sensor != null)
            {
                // Obtain the KinectAudioSource to do audio capture
                KinectAudioSource source = _sensor.AudioSource;
                source.EchoCancellationMode = EchoCancellationMode.CancellationAndSuppression; // No AEC for this sample
                source.NoiseSuppression = true;
                source.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition
                _inputstream = source.Start();
            }

            RecognizerInfo ri = GetKinectRecognizer();

            // Create a SpeechRecognitionEngine object and add the grammars to it.
            _recoEngine = new SpeechRecognitionEngine(ri.Id);
            if (_sensor != null)
            {
                _recoEngine.SetInputToAudioStream(_inputstream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            }
            _recoEngine.LoadGrammar(_grammars.Single(gram => gram.Name.Equals(GrammarName)));
            _recoEngine.BabbleTimeout = _resetInterval;
            _recoEngine.InitialSilenceTimeout = _resetInterval;
        }

        protected override void Stop()
        {
            if (_recoEngine != null)
            {
                try
                {
                    _recoEngine.RecognizeAsyncCancel();
                    while (_kinectActive && _sensor != null)
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (InvalidOperationException exp)
                {
                    // This means we were already cancelled due to timeout.
                    // We need to figure out in what cases this happens, but
                    // until then, we can just ignore.
                }
                _recoEngine.Dispose();
                _recoEngine = null;
            }
        }

        public override void CheckForUpdates()
        {
            if (IsValid)
            {
                if (_resetKinect)
                {
                    Stop();
                    if (_updateGrammars)
                    {
                        UpdateGrammars();
                        _updateGrammars = false;
                    }
                    Start();
                    _resetKinect = false;
                }
                else if (_updateGrammars)
                {
                    Stop();
                    UpdateGrammars();
                    Start();
                }
            }
        }

        protected void ResetTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            _resetKinect = true;
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        public void recoEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;
            RecognizeSpeech(result);
        }

        private void recoEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            _kinectActive = false;
        }
    }
}
