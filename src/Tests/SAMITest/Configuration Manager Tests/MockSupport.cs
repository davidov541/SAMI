using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Test.Infrastructure
{
    [ParseableElement("MockSupport", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockSupport : IDisposable, IParseable
    {
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
        public MockSupport()
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
            _children.Add(child);
        }

        public void Dispose()
        {
            Assert.IsTrue(_hasBeenInitialized);
            Assert.IsTrue(_hasGottenEvent);
        }
    }
}
