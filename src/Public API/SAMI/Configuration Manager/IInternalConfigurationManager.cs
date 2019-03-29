using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAMI.Persistence;

namespace SAMI.Configuration
{
    internal interface IInternalConfigurationManager : IConfigurationManager
    {
        /// <summary>
        /// The COM port that the xbee hub is on.
        /// </summary>
        String XBeeCOM
        {
            get;
        }

        /// <summary>
        /// The COM port that the ZWave controller is on.
        /// </summary>
        String ZWaveCOM
        {
            get;
        }

        /// <summary>
        /// Loads all components of the given type, even those that have an IsValid value of false.
        /// </summary>
        /// <typeparam name="T">Type to filter components by.</typeparam>
        /// <returns>An enumerable of all components of the given type.</returns>
        IEnumerable<T> FindAllComponentsOfTypeEvenInvalid<T>()
            where T : IParseable;

        /// <summary>
        /// Saves the configuration, as it is currently, back to the file from where it was loaded from.
        /// </summary>
        void SaveConfiguration();
    }
}
