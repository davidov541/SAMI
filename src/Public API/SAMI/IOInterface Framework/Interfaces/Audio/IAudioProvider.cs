namespace SAMI.IOInterfaces.Interfaces.Audio
{
    /// <summary>
    /// Stores data that should be output to an audio device, supplying the data
    /// to an <see cref="IAudioStreamManager"/> instance when it is ready for the data.
    /// Should not be created directly by clients, and is instead bade by a <see cref="IAudioStreamManager"/>.
    /// </summary>
    public interface IAudioProvider
    {
        /// <summary>
        /// Indicates how many samples are currently available in the buffer.
        /// </summary>
        int NumberOfSamplesInBuffer
        {
            get;
        }

        /// <summary>
        /// Adds the given data to the buffer of data to be written to the audio device.
        /// </summary>
        /// <param name="buffer">Data that needs to be written to the audio device.</param>
        /// <returns>Number of values that were actually recorded.</returns>
        int AddData(byte[] buffer, int offset, int size);

        /// <summary>
        /// Erases all data in the buffer, starting clean.
        /// </summary>
        void Reset();
    }
}
