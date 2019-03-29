using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.XBee;
using SAMI.Persistence;

namespace $safeprojectname$
{
    [ParseableElement("$projectname$", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class $safeprojectname$ : XBeeInterface
	{
        public override String Name
        {
            get
            {
                return "$safeprojectname$";
            }
        }
		
		public byte SomeProperty
        {
            get;
            private set;
        }
		
		public override IEnumerable<PersistentProperty> Properties
        {
            get
            {
				// Get the properties from XBeeInterface
                foreach (PersistentProperty prop in base.Properties)
                {
                    yield return prop;
                }
				// Add any extra properties needed here
                yield return new PersistentProperty("SomeProperty", () => String.Format("0x{0:2x}", SomeProperty), p => SomeProperty = Byte.Parse(p));
            }
        }

        public $safeprojectname$(IConfigurationManager configManager)
        {
        }

        public void DoSomething()
        {
            // Before sending data, StartXbeeSession must be called. This
            // reserves the XBee for use only by this controller.
            StartXbeeSession();
			// Send some data to the 
            SendXbeeData(new List<byte> { 0x01, SomeProperty });
            // EndXbeeSession must be called at the end. If it is not
            // called, other IO Interfaces will be prevented from using
            // the XBee.
            EndXbeeSession();
        }
	}
}
