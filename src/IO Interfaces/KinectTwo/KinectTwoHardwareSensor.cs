using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Speech.KinectTwo
{
    /// <summary>
    /// In charge of initializing Kinect and getting speech recognition data from it.
    /// </summary>
    [ParseableElement("KinectTwo", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class KinectTwoHardwareSensor : BaseSpeechRecognitionSensor
    {
        private KinectSensor _sensor;

        /// <summary>
        /// Stream for 32b-16b conversion.
        /// </summary>
        private KinectAudioStream _convertStream = null;
        private bool _kinectActive = false;
        private readonly TimeSpan _resetInterval = TimeSpan.FromMinutes(30);

        private bool _resetKinect = false;

        public KinectTwoHardwareSensor()
            : base()
        {
        }

        protected override void InitializeWithControllers()
        {
            _sensor = KinectSensor.GetDefault();
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
            _sensor.Open();

            // grab the audio stream
            IReadOnlyList<AudioBeam> audioBeamList = _sensor.AudioSource.AudioBeams;
            Stream audioStream = audioBeamList[0].OpenInputStream();

            // create the convert stream
            _convertStream = new KinectAudioStream(audioStream);
        }

        public override void Dispose()
        {
            if (IsValid)
            {
                _recoEngine.Dispose();
                if (_sensor != null)
                {
                    _sensor.Close();
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
                AudioSource source = _sensor.AudioSource;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            // Create a SpeechRecognitionEngine object and add the grammars to it.
            _recoEngine = new SpeechRecognitionEngine(ri.Id);
            _recoEngine.LoadGrammar(_grammars.Single(gram => gram.Name.Equals(GrammarName)));
            _recoEngine.BabbleTimeout = _resetInterval;
            _recoEngine.InitialSilenceTimeout = _resetInterval;
            _convertStream.SpeechActive = true;

            // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
            // This will prevent recognition accuracy from degrading over time.
            _recoEngine.UpdateRecognizerSetting("AdaptationOn", 0);
            _recoEngine.SetInputToAudioStream(_convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
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
