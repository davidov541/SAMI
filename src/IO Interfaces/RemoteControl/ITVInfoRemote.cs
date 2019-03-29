using SAMI.IOInterfaces.Interfaces.Remote;

namespace SAMI.IOInterfaces.Remote
{
    /// <summary>
    /// A TV remote which knows how to display information on the screen about
    /// what show is currently playing.
    /// </summary>
    public interface ITVInfoRemote : ITVRemote
    {
        /// <summary>
        /// Displays information on the screen about what show is currently playing on TV.
        /// Calling this function again will remove that information.
        /// </summary>
        void ToggleInfo();
    }
}
