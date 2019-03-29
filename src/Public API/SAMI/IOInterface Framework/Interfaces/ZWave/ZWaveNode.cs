using System;
using System.Collections.Generic;
using System.Linq;
using OpenZWaveDotNet;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal abstract class ZWaveNode : IZWaveNode
    {
        public abstract string Name
        {
            get;
        }

        public virtual bool IsValid
        {
            get
            {
                return IsConnected || !_controller.IsInitialized;
            }
        }

        internal bool IsConnected
        {
            get;
            set;
        }

        private int _nodeId;
        public int NodeId
        {
            get
            {
                return _nodeId;
            }
            set
            {
                _nodeId = value;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("NodeId", () => _nodeId.ToString(), s => NodeId = Int32.Parse(s));
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

        private List<ZWValueID> _valueIds;
        protected IEnumerable<ZWValueID> ValueIds
        {
            get
            {
                return _valueIds;
            }
        }

        private static IZWaveController _controller;
        internal static void SetController(IZWaveController controller)
        {
            _controller = controller;
        }

        public ZWaveNode()
        {
            _children = new List<IParseable>();
            _valueIds = new List<ZWValueID>();
        }

        public void Initialize(IConfigurationManager configManager)
        {
            if (_controller == null)
            {
                _controller = new ZWaveController(configManager as IInternalConfigurationManager);
            }
        }

        public void Dispose()
        {
        }

        public void AddChild(IParseable child)
        {
            _children.Add(child);
        }

        public void AddValueId(ZWValueID valueId)
        {
            if (!_valueIds.Any(v => v.GetCommandClassId() == valueId.GetCommandClassId() && v.GetId() == valueId.GetId()))
            {
                _valueIds.Add(valueId);
            }
        }

        public void RemoveValueId(ZWValueID valueId)
        {
            ZWValueID valueIdForRemoval = _valueIds.SingleOrDefault(v => v.GetCommandClassId() == valueId.GetCommandClassId() && v.GetIndex() == valueId.GetIndex());
            if (valueIdForRemoval != null)
            {
                _valueIds.Remove(valueIdForRemoval);
            }
        }

        protected void SetValue(byte index, byte commandClassId, bool value)
        {
            ZWValueID valueId = GetValueId(index, commandClassId);
            _controller.SetValue(valueId, value);
        }

        protected void SetValue(byte index, byte commandClassId, byte value)
        {
            ZWValueID valueId = GetValueId(index, commandClassId);
            _controller.SetValue(valueId, value);
        }

        private ZWValueID GetValueId(byte index, byte commandClassId)
        {
            ZWValueID valueId = _valueIds.SingleOrDefault(v => v.GetCommandClassId() == commandClassId && v.GetIndex() == index);
            if (valueId == null)
            {
                throw new ArgumentException("The index and CommandClassID do not match any available values!");
            }
            return valueId;
        }

        public bool TryStartPairing(IZWavePairingMonitor monitor)
        {
            bool successful = _controller.TryStartPairing(this, monitor);
            return successful;
        }

        public void ResetController()
        {
            _controller.ResetController();
        }
    }
}
