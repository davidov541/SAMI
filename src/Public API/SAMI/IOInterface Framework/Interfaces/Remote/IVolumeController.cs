using System;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Remote
{
    /// <summary>
    /// Interface for a component which adjusts the volume on a device.
    /// </summary>
    public interface IVolumeController : IIOInterface
    {
        /// <summary>
        /// The current volume that the hardware is set to.
        /// The volume is set on a scale from 0 - 1.
        /// </summary>
        double Volume
        {
            get;
            set;
        }
    }
}
