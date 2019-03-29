using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Apps.Remote
{
    [ParseableElement("PowerRemote", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class PowerRemoteReference : RemoteReference
    {
        public PowerRemoteReference()
            : base()
        {
        }
    }
}
