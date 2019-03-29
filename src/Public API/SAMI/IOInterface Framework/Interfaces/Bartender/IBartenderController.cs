using System;
using System.Collections.Generic;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Bartender
{
    /// <summary>
    /// Interface for a component which communicates with an automated
    /// bartender system that mixes drinks.
    /// </summary>
    public interface IBartenderController : IIOInterface
    {
        /// <summary>
        /// A list of all of the keys of liquids that are avaialable to be dispensed.
        /// </summary>
        IEnumerable<String> AvailableLiquids
        {
            get;
        }

        /// <summary>
        /// Dispenses the given amount of liquid.
        /// </summary>
        /// <param name="liquidKey">Key for which liquid to dispense.</param>
        /// <param name="milliLiters">How much liquid to dispense, in mL.</param>
        void DispenseLiquid(String liquidKey, double milliLiters);
    }
}
