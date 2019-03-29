using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Speech.Recognition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Speech
{
    internal abstract class BaseKinectSensor : IVoiceSensor
    {
        protected bool _updateGrammars = false;
        protected List<Grammar> _grammars;
        protected String _grammarFileName = "MainGrammar.xml";
        protected SpeechRecognitionEngine _recoEngine;
        protected GrammarProviderCollection _grammarCollection;
        private IConfigurationManager _configManager;

        private bool _speechDriverInstalled = true;
        private bool _kinectDriverInstalled = true;

        public String Name
        {
            get;
            private set;
        }

        private String _grammarName = GrammarUtility.MainGrammarName;
        /// <summary>
        /// Sets the current grammar that should be used when listening for input.
        /// This may be accessed anywhere, but should only be set from the UI thread,
        /// since setting the grammar requires restarting the recognition engine.
        /// </summary>
        public String GrammarName
        {
            get
            {
                return _grammarName;
            }
            set
            {
                if (IsValid)
                {
                    if (_grammarName != value)
                    {
                        Stop();
                        _grammarName = value;
                        Start();
                    }
                    CheckForUpdates();
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler<RecognizedNewPhraseEventArgs> RecognizedNewPhrase;

        /// <inheritdoc />
        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("Name", () => Name, n => Name = n);
            }
        }

        /// <inheritdoc />
        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public abstract void CheckForUpdates();

        public BaseKinectSensor()
        {
        }

        /// <inheritdoc />
        public void Initialize(IConfigurationManager configManager)
        {
            _configManager = configManager;
            if (IsValid)
            {
                configManager.InitializationComplete += Configuration_InitializationComplete;
                try
                {
                    _grammarCollection = new GrammarProviderCollection();
                    _grammarCollection.GrammarChanged += GrammarChanged;
                    InitializeWithControllers();
                }
                catch (FileNotFoundException)
                {
                    _speechDriverInstalled = false;
                    _configManager.InitializationComplete -= Configuration_InitializationComplete;
                    Console.WriteLine("It appears you do not have the Microsoft Speech API installed. This means any Kinect-based IO Interface will not work, until it is installed. It is suggested you install the driver in order to use SAMI.");
                }
                catch (BadImageFormatException)
                {
                    _kinectDriverInstalled = false;
                    _configManager.InitializationComplete -= Configuration_InitializationComplete;
                    Console.WriteLine("It appears you do not have the Kinect 1.0 SDK installed. This means SAMI will not be able to work with the Kinect. It is suggested you install the driver in order to use SAMI with a Kinect.");
                }
            }
        }

        private void Configuration_InitializationComplete(object sender, EventArgs e)
        {
            try
            {
                (sender as IConfigurationManager).InitializationComplete -= Configuration_InitializationComplete;
                UpdateGrammars();
                Start();
            }
            catch (COMException)
            {
                _speechDriverInstalled = false;
                _configManager.InitializationComplete -= Configuration_InitializationComplete;
                Console.WriteLine("It appears you do not have the Microsoft Speech API installed. This means any Kinect-based IO Interface will not work, until it is installed. It is suggested you install the driver in order to use SAMI.");
            }
        }

        protected virtual void InitializeWithControllers()
        {
        }

        protected abstract void Start();

        protected abstract void Stop();

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return _speechDriverInstalled && _kinectDriverInstalled;
            }
        }

        /// <inheritdoc />
        public void AddChild(IParseable component)
        {
        }

        protected void OnRecognizedNewPhrase(Dialog dialog)
        {
            if (RecognizedNewPhrase != null)
            {
                RecognizedNewPhrase(this, new RecognizedNewPhraseEventArgs(dialog));
            }
        }

        protected void GrammarChanged(object sender, EventArgs e)
        {
            _updateGrammars = true;
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        protected static RecognizerInfo GetKinectRecognizer()
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

            return SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault();
        }

        protected void UpdateGrammars()
        {
            if (IsValid)
            {
                _grammars = new List<Grammar>();

                _grammarCollection.UpdateMainGrammarFile(_grammarFileName);
                Grammar baseGrammar = new Grammar(_grammarFileName);
                baseGrammar.Enabled = true;
                baseGrammar.Name = GrammarUtility.MainGrammarName;
                _grammars.Add(baseGrammar);

                foreach (XmlGrammar unparsed in _grammarCollection.ExtraGrammars)
                {
                    String fileName = unparsed.RuleName + ".xml";
                    unparsed.RootDocument.Save(fileName);
                    Grammar gram = new Grammar(fileName);
                    gram.Name = unparsed.RuleName;
                    _grammars.Add(gram);
                }
                _updateGrammars = false;
            }
        }

        /// <inheritdoc />
        public abstract void Dispose();

        protected Dictionary<String, String> ConvertResultToDictionary(RecognitionResult result)
        {
            Dictionary<String, String> vals = new Dictionary<string, string>();
            foreach (KeyValuePair<String, SemanticValue> val in result.Semantics)
            {
                if (val.Value.Value == null)
                {
                    vals[val.Key] = "";
                }
                else
                {
                    vals[val.Key] = val.Value.Value.ToString();
                }
            }
            return vals;
        }

        protected void RecognizeSpeech(RecognitionResult result)
        {
            // If confidence is 1.0, we probably just simulated it, so no need to report what we found.
            if (result != null && result.Confidence != 1.0)
            {
                Console.WriteLine(result.Text + " - " + result.Confidence.ToString() + "%");
            }
            if (result != null && result.Confidence > 0.7)
            {
                Dictionary<String, String> vals = ConvertResultToDictionary(result);
                OnRecognizedNewPhrase(new Dialog(vals, result.Text));
            }
        }

        /// <inheritdoc />
        public void AddGrammarProvider(GrammarProvider provider)
        {
            _grammarCollection.AddGrammarProvider(provider);
        }

        /// <inheritdoc />
        public void RemoveGrammarProvider(GrammarProvider provider)
        {
            _grammarCollection.RemoveGrammarProvider(provider);
        }

        /// <inheritdoc />
        public Dialog GetDialogForInput(String input)
        {
            if (IsValid)
            {
                RecognitionResult result = _recoEngine.EmulateRecognize(input);
                return new Dialog(ConvertResultToDictionary(result), result.Text);
            }
            return null;
        }

        /// <inheritdoc />
        public void SimulateInput(String input)
        {
            if (IsValid)
            {
                _recoEngine.EmulateRecognize(input);
            }
        }
    }
}
