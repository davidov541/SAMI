using System;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenZWaveDotNet;
using SAMI.Configuration;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal class ZWaveController : IZWaveController
    {
        private IInternalConfigurationManager _configManager;
        private IZWaveManager _zWaveManager;
        private IZWaveOptions _zWaveOptions;
        private UInt32 _homeId;
        private IZWavePairingMonitor _monitor;
        private bool _hasSeenButtonPress;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public ZWaveController(IInternalConfigurationManager manager)
        {
            _configManager = manager;
            _configManager.InitializationComplete += ConfigurationInitializationComplete;
            IsInitialized = false;
        }

        public ZWaveController(IInternalConfigurationManager configManager, IZWaveManager zWaveManager, IZWaveOptions zWaveOptions)
            : this(configManager)
        {
            _zWaveManager = zWaveManager;
            _zWaveOptions = zWaveOptions;
        }

        private void ConfigurationInitializationComplete(object sender, EventArgs e)
        {
            _configManager.InitializationComplete -= ConfigurationInitializationComplete;

            if (_zWaveManager == null)
            {
                _zWaveManager = new ZWaveManager();
            }
            if (_zWaveOptions == null)
            {
                _zWaveOptions = new ZWaveOptions();
            }
            String optionsDirectory;
            if (Assembly.GetEntryAssembly() == null)
            {
                optionsDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ZWave");
            }
            else
            {
                optionsDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ZWave");
            }
            _zWaveOptions.Create(optionsDirectory, optionsDirectory, "");
            _zWaveOptions.Lock();
            _zWaveManager.Create();
            _zWaveManager.AddDriver(_configManager.ZWaveCOM, ZWControllerInterface.Serial);
            _zWaveManager.OnNotification += NotificationRecieved;
            _zWaveManager.OnControllerStateChanged += ControllerStateChanged;
        }

        private void ControllerStateChanged(Object sender, ControllerStateChangedEventArgs args)
        {
        }

        private ZWaveNode _nodeBeingPaired;
        private void NotificationRecieved(Object sender, NotificationEventArgs args)
        {
            ZWaveNode node = _configManager.FindAllComponentsOfTypeEvenInvalid<ZWaveNode>().SingleOrDefault(n => n.NodeId == args.NodeId);
            switch (args.NotificationType)
            {
                case ZWNotification.Type.NodeAdded:
                    if (node != null)
                    {
                        node.IsConnected = true;
                    }
                    else if (_nodeBeingPaired != null)
                    {
                        _nodeBeingPaired.NodeId = args.NodeId;
                        _nodeBeingPaired.IsConnected = true;
                        _nodeBeingPaired = null;
                    }
                    break;
                case ZWNotification.Type.NodeRemoved:
                    if (node != null)
                    {
                        node.IsConnected = false;
                    }
                    break;
                case ZWNotification.Type.AllNodesQueried:
                case ZWNotification.Type.AllNodesQueriedSomeDead:
                    IsInitialized = true;
                    break;
                case ZWNotification.Type.NodeQueriesComplete:
                    if (_monitor != null)
                    {
                        _monitor.PairingCompleted();
                        _monitor = null;
                    }
                    break;
                case ZWNotification.Type.ValueAdded:
                    if (node != null)
                    {
                        node.AddValueId(args.ValueId);
                    }
                    break;
                case ZWNotification.Type.ValueRemoved:
                    if (node != null)
                    {
                        node.RemoveValueId(args.ValueId);
                    }
                    if (_monitor != null && !_hasSeenButtonPress)
                    {
                        _monitor.DeviceFound();
                        _hasSeenButtonPress = true;
                    }
                    break;
                case ZWNotification.Type.DriverReady:
                    _homeId = args.HomeId;
                    break;
                case ZWNotification.Type.ValueRefreshed:
                case ZWNotification.Type.ValueChanged:
                case ZWNotification.Type.AwakeNodesQueried:
                case ZWNotification.Type.ButtonOff:
                case ZWNotification.Type.ButtonOn:
                case ZWNotification.Type.CreateButton:
                case ZWNotification.Type.DeleteButton:
                case ZWNotification.Type.DriverFailed:
                case ZWNotification.Type.DriverReset:
                case ZWNotification.Type.EssentialNodeQueriesComplete:
                case ZWNotification.Type.Group:
                case ZWNotification.Type.Notification:
                case ZWNotification.Type.PollingDisabled:
                case ZWNotification.Type.PollingEnabled:
                case ZWNotification.Type.SceneEvent:
                case ZWNotification.Type.NodeEvent:
                case ZWNotification.Type.NodeNaming:
                case ZWNotification.Type.NodeNew:
                case ZWNotification.Type.NodeProtocolInfo:
                default:
                    break;
            }
        }

        public void SetValue(ZWValueID valueId, bool value)
        {
            _zWaveManager.SetValue(valueId, value);
        }

        public void SetValue(ZWValueID valueId, byte value)
        {
            _zWaveManager.SetValue(valueId, value);
        }

        public bool TryStartPairing(ZWaveNode nodeBeingPaired, IZWavePairingMonitor monitor)
        {
            if (_nodeBeingPaired != null ||
                _monitor != null)
            {
                return false;
            }
            bool result = _zWaveManager.BeginControllerCommand(_homeId, ZWControllerCommand.AddDevice, false, 0);
            if (result)
            {
                _nodeBeingPaired = nodeBeingPaired;
                _hasSeenButtonPress = false;
                _monitor = monitor;
                IsInitialized = false;
                _monitor.PairingStarted();
            }
            return result;
        }


        public void ResetController()
        {
            _zWaveManager.ResetController(_homeId);
        }
    }
}
