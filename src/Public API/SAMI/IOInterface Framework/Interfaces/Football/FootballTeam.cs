using System;
using SAMI.IOInterfaces.Interfaces.Sports;

namespace SAMI.IOInterfaces.Interfaces.Football
{
    /// <summary>
    /// Represents an American football team at any level.
    /// </summary>
    public class FootballTeam : SportsTeam
    {
        /// <summary>
        /// Main constructor for a football team.
        /// </summary>
        /// <param name="locationName">The name of the location for this team, whether it is a city name, state name, or university name.</param>
        /// <param name="team">Name of the mascot for the team.</param>
        /// <param name="key">The key which refers to the team in grammars and to interfaces.</param>
        public FootballTeam(String locationName, String team, String key)
            : base(locationName, team, key)
        {
        }
    }
}
