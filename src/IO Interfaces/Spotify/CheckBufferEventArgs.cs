using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMI.IOInterfaces.Spotify
{
    internal class CheckBufferEventArgs : EventArgs
    {
        public int NumberOfSamples
        {
            get;
            set;
        }
    }
}
