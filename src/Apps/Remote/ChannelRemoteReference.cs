using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.Apps.Remote
{
    [ParseableElement("ChannelRemote", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ChannelRemoteReference : RemoteReference
    {
        public ChannelRemoteReference()
            : base()
        {
        }
    }
}
