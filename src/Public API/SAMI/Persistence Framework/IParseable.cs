using System;
using System.Collections.Generic;
using SAMI.Configuration;

namespace SAMI.Persistence
{
    /// <summary>
    /// Interface which enables a class to be persisted in the configuration file for SAMI.
    /// Any leaf class which implements this interface should have a ParseableElement attribute
    /// defined for the class, in order for it to be recognized by the framework.
    /// </summary>
    public interface IParseable : IDisposable
    {
        /// <summary>
        /// Indicates if this IParseable is valid to be used.
        /// </summary>
        bool IsValid
        {
            get;
        }

        /// <summary>
        /// Contains PersitentProperty instances for each property that should be persisted by this class.
        /// </summary>
        IEnumerable<PersistentProperty> Properties
        {
            get;
        }

        /// <summary>
        /// Contains all of the IParseable instances that should be persisted as
        /// children of this class.
        /// </summary>
        IEnumerable<IParseable> Children
        {
            get;
        }

        /// <summary>
        /// Called once all IParseable instances have been created from the configuration file.
        /// This is called on the parent element before being called on the children elements.
        /// </summary>
        /// <param name="configurationManager">ConfigurationManager which owns this component.</param>
        void Initialize(IConfigurationManager configurationManager);

        /// <summary>
        /// Adds a child IParseable element. 
        /// This should add the element to the backing for <see cref="Children"/>,
        /// and perform any other necessary work on the child.
        /// </summary>
        /// <param name="child">Child to add to this IParseable instance.</param>
        void AddChild(IParseable child);
    }
}
