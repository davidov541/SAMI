using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.Remote
{
    /// <summary>
    /// Handles interfacing with a TV, whether through a remote or directly.
    /// </summary>
    public interface ITVRemote : IIOInterface
    {
        /// <summary>
        /// Sends a channel (in the form of a number) to the TV.
        /// The TV should then change to the indicated channel.
        /// There is no guarantee that the number is a valid channel.
        /// </summary>
        /// <param name="number">The number of the channel to switch to.</param>
        void SendChannel(string number);

        /// <summary>
        /// Returns a list of the names of channels which are available to switch to.
        /// These names should be user readable, but also work with <see cref="TrySendChannel"/>.
        /// </summary>
        /// <returns>A list of the names of the available channels.</returns>
        IEnumerable<String> GetChannels();

        /// <summary>
        /// Tries to have the TV switch to the channel represented by the given name.
        /// </summary>
        /// <param name="channelName">Name of the channel to try to switch to.</param>
        /// <returns>True if that channel is recognized and the switch was successful, false otherwise.</returns>
        bool TrySendChannel(String channelName);
    }
}
