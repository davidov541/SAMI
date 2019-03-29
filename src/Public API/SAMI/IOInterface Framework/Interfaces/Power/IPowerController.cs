using System;

namespace SAMI.IOInterfaces.Interfaces.Power
{
    public interface IPowerController : IIOInterface
    {
        /// <summary>
        /// Turns on this switch.
        /// </summary>
        void TurnOn();

        /// <summary>
        /// Turns off this switch.
        /// </summary>     
        void TurnOff();
    }
}
