using System;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Interface describing a component which outputs text to the user in some way.
    /// </summary>
    public interface IOutputController : IIOInterface
    {
        /// <summary>
        /// Outputs the given text.
        /// </summary>
        /// <param name="output">Text to output.</param>
        void OutputText(String output);
    }
}
