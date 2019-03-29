using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    /// <summary>
    /// An IOInterface which utilizes the XBee interface to connect to a piece of hardware.
    /// </summary>
    public abstract class XBeeInterface : IIOInterface
    {
        private static IXBee _xbee;

        #region Properties
        /// <summary>
        /// Name of this particular instance of the IO resource.
        /// This allows for referencing from apps when determining which
        /// IO resources an app should interact with in a certain context.
        /// </summary>
        public abstract String Name
        {
            get;
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
        /// Indicates if this IParseable is valid to be used.
        /// If overriden, the base value of IsValid should be anded with the new value.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return _xbee.IsInitialized;
            }
        }

        private UInt64 _xbeeId;
        /// <summary>
        /// Contains PersitentProperty instances for each property that should be persisted by this class.
        /// </summary>
        public virtual IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("XBeeId", () => _xbeeId.ToString(), id => _xbeeId = Convert.ToUInt64(id, 16));
            }
        }
        #endregion

        #region Methods
        public XBeeInterface()
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
        /// Called once all IParseable instances have been created from the configuration file.
        /// This is called on the parent element before being called on the children elements.
        /// </summary>
        public virtual void Initialize(IConfigurationManager configManager)
        {
            if (_xbee == null)
            {
                _xbee = new XBee(configManager as IInternalConfigurationManager, true);
            }
        }

        /// <summary>
        /// Adds a child IParseable element. 
        /// This should add the element to the backing for <see cref="Children"/>,
        /// and perform any other necessary work on the child.
        /// </summary>
        /// <param name="child">Child to add to this IParseable instance.</param>
        public virtual void AddChild(IParseable component)
        {
            _children.Add(component);
        }

        /// <summary>
        /// Sends the data indicated to the correct XBee device.
        /// </summary>
        /// <param name="data">Data to send to the xbee device.</param>
        protected void SendXbeeData(List<byte> data)
        {
            _xbee.Send(_xbeeId, data);
        }


        /// <summary>
        /// Reads an RX packet from the correct XBee device.
        /// </summary>
        protected List<byte> ReadXbeeData()
        {
            return _xbee.Receive();
        }

        /// <summary>
        /// Starts a session to the xbee device. This should be called before calling SendXbeeData.
        /// </summary>
        protected void StartXbeeSession()
        {
            _xbee.StartXbeeSession();
        }

        /// <summary>
        /// Ends a session to the xbee device. This should be called after all calls to SendXbeeData.
        /// </summary>
        protected void EndXbeeSession()
        {
            _xbee.EndXbeeSession();
        }

        protected byte[] IntToByteArray(int value)
        {
            byte[] byteArray = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                byteArray[i] = (byte)(value & 0xFF);
                value = value >> 8;
            }
            return byteArray;
        }
        #endregion
    }
}
