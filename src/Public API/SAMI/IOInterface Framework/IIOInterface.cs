using System;
using SAMI.Persistence;

namespace SAMI.IOInterfaces
{
    /// <summary>
    /// Interface representing all IO resources.
    /// Any IO resources must inherit from this interface.
    /// </summary>
    public interface IIOInterface : IParseable
    {
        /// <summary>
        /// Name of this particular instance of the IO resource.
        /// This allows for referencing from apps when determining which
        /// IO resources an app should interact with in a certain context.
        /// </summary>
        String Name
        {
            get;
        }
    }
}
