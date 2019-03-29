using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces;
using SAMI.Persistence;

namespace $safeprojectname$
{
    [ParseableElement("$projectname$", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class $safeprojectname$ : IIOInterface
	{
        public String Name
        {
            get
            {
                return "$safeprojectname$";
            }
        }

        public IEnumerable<IParseable> Children
        {
            get 
            {
                yield break;
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield break;
            }
        }

        public $safeprojectname$(IConfigurationManager configManager)
        {
        }

        public void Initialize(IConfigurationManager configurationManager)
        {
        }

        public void Dispose()
        {
        }

        public void AddChild(IParseable child)
        {
        }
	}
}
