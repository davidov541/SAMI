using System;
using SAMI.Configuration;
using SAMI.Logging;

namespace SAMI.Application
{
    internal class KinectSAMI : SAMIBase
    {
        public bool UpdateWaiting
        {
            get;
            set;
        }

        public KinectSAMI(IInternalConfigurationManager configManager)
            : base(configManager)
        {
            UpdateWaiting = false;
            _lastTimeInteracted = DateTime.Now;
        }

        public override void CheckForInputs()
        {
        }

        public override void CheckForUpdates()
        {
            base.CheckForUpdates();
#if DEBUG
            if(UpdateWaiting)
#else
            if(UpdateWaiting && DateTime.Now.Subtract(_lastTimeInteracted).CompareTo(new TimeSpan(1, 0, 0)) > 0)
#endif
            {
                Logger.LogString(ConfigManager, "SAMI is ready to update!", LogCategory.Debug);
                CanBeUpdated = true;
                _shouldStop = true;
            }
        }
    }
}
