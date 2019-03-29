using System;
using System.Collections.Generic;
using SAMI.IOInterfaces.Interfaces.Power;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.LightSwitch
{
    /// <summary>
    /// Interface for components that control simple light switches.
    /// </summary>
    public interface ILightSwitchController : IPowerController
    {
        /// <summary>
        /// Type of switch this represents.
        /// </summary>
        SwitchType Type
        {
            get;
        }
    }
}
