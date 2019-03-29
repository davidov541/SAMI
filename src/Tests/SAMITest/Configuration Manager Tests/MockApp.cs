using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Test.Infrastructure
{
    /// <summary>
    /// WARNING: Should not be used directly in any capacity. Only here to test XML Parsing.
    /// </summary>
    [ParseableElement("MockApp", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockApp : IApp, IDisposable
    {
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public string InvalidMessage
        {
            get
            {
                return String.Empty;
            }
        }

        public String TestProp
        {
            get;
            set;
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("TestProp", () => TestProp, t => TestProp = t);
            }
        }

        private List<IParseable> _children;
        public IEnumerable<IParseable> Children
        {
            get
            {
                return _children;
            }
        }

        private bool _hasBeenInitialized;
        private bool _hasGottenEvent;
        public MockApp()
        {
            _children = new List<IParseable>();
            _hasBeenInitialized = false;
            _hasGottenEvent = false;
        }

        private void configManager_InitializationComplete(object sender, System.EventArgs e)
        {
            Assert.IsFalse(_hasGottenEvent);
            _hasGottenEvent = true;
        }

        public void Initialize(IConfigurationManager configManager)
        {
            configManager.InitializationComplete += configManager_InitializationComplete;
            Assert.IsFalse(_hasBeenInitialized);
            _hasBeenInitialized = true;
        }

        public void AddChild(IParseable child)
        {
            Assert.IsInstanceOfType(child, typeof(MockSupport));
            _children.Add(child);
        }

        public void RemoveChild(String testAttributeValue)
        {
            Assert.IsTrue(_children.OfType<MockSupport>().Any(c => c.TestProp.Equals(testAttributeValue)));
            _children.Remove(_children.OfType<MockSupport>().Single(c => c.TestProp.Equals(testAttributeValue)));
        }

        public void Dispose()
        {
            Assert.IsTrue(_hasBeenInitialized);
            Assert.IsTrue(_hasGottenEvent);
        }

        public bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = null;
            Assert.Fail("TryCreateConversationFromPhrase was called when it was not expected!");
            return false;
        }
    }
}
