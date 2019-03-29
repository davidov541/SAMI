using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Configuration;
using SAMI.IOInterfaces;
using SAMI.Persistence;

namespace SAMI.Test.Infrastructure
{
    /// <summary>
    /// WARNING: Should not be used directly in any capacity. Only here to test XML Parsing.
    /// </summary>
    [ParseableElement("MockIOInterface", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockIOInterface : IDisposable, IIOInterface
    {
        public String Name
        {
            get
            {
                return "Mock IO Interface";
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
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
        public MockIOInterface()
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

        public void Dispose()
        {
            Assert.IsTrue(_hasBeenInitialized);
            Assert.IsTrue(_hasGottenEvent);
        }
    }
}
