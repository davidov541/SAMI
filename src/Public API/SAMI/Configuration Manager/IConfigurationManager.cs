using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SAMI.Apps;
using SAMI.Persistence;

namespace SAMI.Configuration
{
    /// <summary>
    /// Manager of configuration information about SAMI.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Fires after Initialize has been called on all components.
        /// This should not change any settings on a component, but may
        /// start processes that depend on all components being initialized.
        /// </summary>
        event EventHandler InitializationComplete;

        /// <summary>
        /// The location which SAMI resides in.
        /// </summary>
        Location LocalLocation
        {
            get;
        }

        /// <summary>
        /// Returns a list of all of the components of the given type that
        /// were specified in the configuration file, and available from plug-ins.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <returns>All of the components that match that given type.</returns>
        IEnumerable<T> FindAllComponentsOfType<T>() where T : IParseable;

        /// <summary>
        /// Returns the full absolute path of the file given. 
        /// This should be used whenever requesting a file by a relative path,
        /// since the actual location of that file may not be within the working directory.
        /// </summary>
        /// <param name="fileName">Relative path of the file.</param>
        /// <param name="owningType">A type inside of the library which owns the file. This will be where fileName should be relative to.</param>
        /// <returns>A string representing the absolute path to the file requested.</returns>
        String GetPathForFile(String fileName, Type owningType);
    }
}
