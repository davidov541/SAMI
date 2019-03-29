using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps
{
    /// <summary>
    /// Base class for all SAMI apps.
    /// This is the non-templated version, for use in 
    /// iterating through all apps available.
    /// This should not be directly inherited from.
    /// Instead, inherit from VoiceActivatedApps, or, if no
    /// voice activation is required, inherit from SAMIApp.
    /// </summary>
    public abstract class BaseSAMIApp : IApp
    {
        /// <summary>
        /// Instance of the configuration manager to use in order to access
        /// information about SAMI's current configuration.
        /// </summary>
        protected IConfigurationManager ConfigManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether this app is valid.
        /// If it is not valid, it should be ignored by the system,
        /// although this property should be checked periodically for changes.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Message which indicates why the app is invalid.
        /// If the app is valid, this may be null or String.Empty.
        /// The message should be user visible, and able to be spoken.
        /// </summary>
        public virtual String InvalidMessage
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Full constructor for any SAMI app.
        /// </summary>
        /// <param name="configManager">Configuration manager instance to use.</param>
        public BaseSAMIApp()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Indicates whether the app can create a conversation from the given initial dialog.
        /// A conversation should be created, if one may be created.
        /// </summary>
        /// <param name="phrase">Initial dialog that the user is trying to create a conversation from.</param>
        /// <param name="createdConversation">If a conversation could be created, this is a pointer to the conversation. Otherwise, it is assumed to be null.</param>
        /// <returns>True if a conversation could be created. False otherwise.</returns>
        public virtual bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = null;
            return false;
        }

        /// <summary>
        /// Indicates that this app is raising an asynchronous alert to start a new conversation.
        /// </summary>
        internal event EventHandler<AsyncAlertEventArgs> AsyncAlertRaised;

        /// <summary>
        /// Raises an asynchronous alert, starting the conversation passed.
        /// </summary>
        /// <param name="convo">Conversation that should be started during the asynchronous alert.</param>
        protected void RaiseAsyncAlert(Conversation convo)
        {
            if (AsyncAlertRaised != null)
            {
                AsyncAlertRaised(this, new AsyncAlertEventArgs(convo));
            }
        }

        /// <summary>
        /// Contains PersistentProperty instances for each property that should be persisted by this class.
        /// </summary>
        public virtual IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield break;
            }
        }

        private List<IParseable> _children = new List<IParseable>();
        /// <summary>
        /// Contains all of the IParseable instances that should be persisted as
        /// children of this class.
        /// </summary>
        public IEnumerable<IParseable> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Called once all IParseable instances have been created from the configuration file.
        /// This is called on the parent element before being called on the children elements.
        /// </summary>
        public virtual void Initialize(IConfigurationManager configManager)
        {
            ConfigManager = configManager;
        }

        /// <summary>
        /// Adds a child IParseable element. 
        /// The base implementation of this function adds component
        /// to the Children enumeration. 
        /// Any override of this function should call the base call, and
        /// not add component to Children.
        /// </summary>
        /// <param name="child">Child to add to this IParseable instance.</param>
        public virtual void AddChild(IParseable component)
        {
            _children.Add(component);
        }
    }
}