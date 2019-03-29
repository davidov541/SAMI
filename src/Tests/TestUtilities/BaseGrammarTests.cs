using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Speech.Recognition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Test.Utilities
{
    [DeploymentItem("UtilityGrammar.grxml")]
    [DeploymentItem("DateTimeGrammar.grxml")]
    public class BaseGrammarTests : BaseSAMITests
    {
        private List<String> _utilityFiles = new List<String>
        {
            "UtilityGrammar.grxml", 
            "DateTimeGrammar.grxml"
        };

        public void TestGrammar<T>(String input, Dictionary<String, String> expectedParams, String grammarName = null)
            where T : BaseSAMIApp
        {
            // Setup fake configuration manager
            MockConfigurationManager configManager = GetConfigurationManager() as MockConfigurationManager;
            MockVoiceControl control = new MockVoiceControl();
            configManager.AddComponent(control);

            // Make app object to get grammar for.
            T app = typeof(T).GetConstructor(new Type[] { }).Invoke(new Object[] { }) as T;
            TestGrammar(input, expectedParams, grammarName, control, app);
        }

        public void TestGrammar(BaseSAMIApp app, IConfigurationManager configManager, String input, Dictionary<String, String> expectedParams, String grammarName = null)
        {
            // Setup fake configuration manager
            MockConfigurationManager mockConfigManager = configManager as MockConfigurationManager;
            MockVoiceControl control = new MockVoiceControl();
            mockConfigManager.AddComponent(control);

            // Make app object to get grammar for.
            TestGrammar(input, expectedParams, grammarName, control, app);
        }

        private void TestGrammar(String input, Dictionary<String, String> expectedParams, String grammarName, MockVoiceControl control, BaseSAMIApp app)
        {
            RecognitionResult result = RunGrammarOnInput(input, grammarName, control, app);
            Assert.IsNotNull(result);
            Dictionary<String, String> actualParams = ConvertResultToDictionary(result);

            // Check parameters
            Assert.AreEqual(expectedParams.Count, actualParams.Count);
            foreach (KeyValuePair<String, String> kvp in actualParams)
            {
                Assert.IsTrue(expectedParams.ContainsKey(kvp.Key), String.Format("The unexpected key {0} was found in the parameters.", kvp.Key));
                Assert.AreEqual(expectedParams[kvp.Key], kvp.Value);
            }

            Assert.IsTrue(app.IsValid);
        }

        private RecognitionResult RunGrammarOnInput(String input, String grammarName, MockVoiceControl control, BaseSAMIApp app)
        {
            app.Initialize(GetConfigurationManager());
            foreach (IParseable child in app.Children)
            {
                child.Initialize(GetConfigurationManager());
            }

            // Create a SpeechRecognitionEngine object and add the grammars to it.
            RecognizerInfo ri = GetKinectRecognizer();
            SpeechRecognitionEngine recoEngine = new SpeechRecognitionEngine(ri.Id);
            XmlGrammar grammar = control.GrammarProvider.MainGrammar;
            if (grammarName != null)
            {
                grammar = control.GrammarProvider.ExtraGrammars.Single(g => g.RuleName.Equals(grammarName));
            }

            foreach (String fileName in _utilityFiles)
            {
                XmlDocument fileDoc = new XmlDocument();
                fileDoc.Load(fileName);
                foreach (XmlElement element in fileDoc.LastChild.ChildNodes)
                {
                    XmlNode copiedNode = grammar.RootDocument.ImportNode(element, true);
                    grammar.RootDocument.LastChild.AppendChild(copiedNode);
                }
            }

            grammar.RootDocument.Save("grammar.xml");
            recoEngine.LoadGrammar(new Grammar("grammar.xml"));

            // Simulate recognition and get result parameters
            RecognitionResult result = recoEngine.SimulateRecognize(input);
            return result;
        }

        public void TestNoRecognitionGrammar<T>(String input, String grammarName = null)
            where T : BaseSAMIApp
        {
            // Setup fake configuration manager
            MockConfigurationManager configManager = GetConfigurationManager() as MockConfigurationManager;
            MockVoiceControl control = new MockVoiceControl();
            configManager.AddComponent(control);

            // Make app object to get grammar for.
            T app = typeof(T).GetConstructor(new Type[] { typeof(IConfigurationManager) }).Invoke(new Object[] { configManager }) as T;
            TestNoRecognitionGrammar(input, grammarName, control, app);
        }

        public void TestNoRecognitionGrammar(BaseSAMIApp app, IConfigurationManager configManager, String input, String grammarName = null)
        {
            // Setup fake configuration manager
            MockConfigurationManager mockConfigManager = configManager as MockConfigurationManager;
            MockVoiceControl control = new MockVoiceControl();
            mockConfigManager.AddComponent(control);

            // Make app object to get grammar for.
            TestNoRecognitionGrammar(input, grammarName, control, app);
        }

        private void TestNoRecognitionGrammar(String input, String grammarName, MockVoiceControl control, BaseSAMIApp app)
        {
            RecognitionResult result = RunGrammarOnInput(input, grammarName, control, app);
            Assert.IsTrue(result == null || result.Confidence < 0.7);
        }

        private RecognizerInfo GetKinectRecognizer()
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

        private Dictionary<String, String> ConvertResultToDictionary(RecognitionResult result)
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
    }
}
