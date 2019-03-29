using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Apps.Remote
{
    [ParseableElement("VolumeRemote", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class VolumeRemoteReference : RemoteReference
    {
        public VolumeRemoteReference()
            : base()
        {
        }
    }
}
