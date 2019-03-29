using System;
using SAMI.IOInterfaces.Interfaces.Sports;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// Represents a single baseball team at any level.
    /// </summary>
    public class BaseballTeam : SportsTeam
    {
        /// <summary>
        /// Name of the division in which this baseball team is located in.
        /// </summary>
        public String Division
        {
            get;
            private set;
        }

        /// <summary>
        /// Full constructor of BaseballTeam.
        /// </summary>
        /// <param name="locationName">City or location of the team.</param>
        /// <param name="team">Team name or mascot name.</param>
        /// <param name="key">The key by which this is represented in places where spaces are not allowed.</param>
        /// <param name="division">Name of the division that this team is in.</param>
        public BaseballTeam(String locationName, String team, String key, String division)
            : base(locationName, team, key)
        {
            Division = division;
        }

        /// <summary>
        /// Indicates whether the given object is equal or not.
        /// In this case, this == obj iff the divisions match up and both are BaseballTeams.
        /// </summary>
        /// <param name="obj">Other object to check.</param>
        /// <returns>True if they are considered equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
            {
                return false;
            }

            BaseballTeam other = obj as BaseballTeam;
            if(other == null)
            {
                return false;
            }

            return other.Division.Equals(Division);
        }
    }
}
