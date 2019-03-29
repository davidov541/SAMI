using System;

namespace SAMI.Apps
{
    /// <summary>
    /// Struct describing a given location.
    /// </summary>
    public struct Location
    {
        /// <summary>
        /// The U.S. state for this location
        /// </summary>
        public String State
        {
            get;
            private set;
        }

        /// <summary>
        /// The city for this location. If this is used, 
        /// it is expected that state is also set.
        /// </summary>
        public String City
        {
            get;
            private set;
        }

        /// <summary>
        /// The U.S. zip code for the location.
        /// </summary>
        public int ZipCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Full constructor for Location.
        /// </summary>
        /// <param name="city">Value of <see cref="City"/></param>
        /// <param name="state">Value of <see cref="State"/></param>
        /// <param name="zipCode">Value of <see cref="ZipCode"/></param>
        public Location(String city, String state, int zipCode)
            : this()
        {
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }
}
