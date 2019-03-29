using SAMI.IOInterfaces.Interfaces.LightSwitch;

namespace SAMI.IOInterfaces.Interfaces.LightSwitch
{
    /// <summary>
    /// A controller for a light switch that may be dimmed.
    /// </summary>
    public interface IDimmableLightSwitchController : ILightSwitchController
    {
        /// <summary>
        /// Sets the light level to the specified level on this light.
        /// </summary>
        /// <param name="level">Double between 0 and 1, where 0 is all the way off, and 1 is all the way on.</param>
        void SetLightLevel(double level);
    }
}
