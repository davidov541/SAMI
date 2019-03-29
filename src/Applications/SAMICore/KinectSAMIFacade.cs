using System;
using SAMI.Configuration;

namespace SAMI.Application
{
    internal class KinectSAMIFacade : IDisposable
    {
        private KinectSAMI _sami;

        public bool CanBeUpdated
        {
            get
            {
                return _sami.CanBeUpdated;
            }
        }

        public bool UpdateWaiting
        {
            get
            {
                return _sami.UpdateWaiting;
            }
            set
            {
                _sami.UpdateWaiting = value;
            }
        }

        public KinectSAMIFacade(IInternalConfigurationManager configManager)
        {
            _sami = new KinectSAMI(configManager);
        }

        public void Run(bool giveIntro)
        {
            _sami.Run(giveIntro);
        }

        public void Dispose()
        {
            _sami.Dispose();
        }
    }
}
