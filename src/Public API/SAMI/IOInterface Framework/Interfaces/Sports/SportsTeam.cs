using System;

namespace SAMI.IOInterfaces.Interfaces.Sports
{
    /// <summary>
    /// A representation of a generic sports team.
    /// Override this for various sports.
    /// </summary>
    public abstract class SportsTeam
    {
        /// <summary>
        /// Name of the area which this sports team represents. 
        /// This could be city, state, university, country, or other locations.
        /// </summary>
        public String LocationName
        {
            get;
            private set;
        }

        /// <summary>
        /// Mascot name of the team.
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Key which is used to look up this team in grammars and in APIs.
        /// </summary>
        public String Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Full constructor for SportsTeam.
        /// </summary>
        /// <param name="locationName">Value for <see cref="LocationName"/></param>
        /// <param name="team">Value for <see cref="Name"/></param>
        /// <param name="key">Value for <see cref="Key"/></param>
        public SportsTeam(String locationName, String team, String key)
        {
            LocationName = locationName;
            Name = team;
            Key = key;
        }

        /// <summary>
        ///  Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SportsTeam))
            {
                return false;
            }
            SportsTeam other = obj as SportsTeam;
            if(!other.Name.Equals(Name))
            {
                return false;
            }
            else if(!other.Key.Equals(Key))
            {
                return false;
            }
            else if(!other.LocationName.Equals(LocationName))
            {
                return false;
            }
            return true;
        }
    }
}
