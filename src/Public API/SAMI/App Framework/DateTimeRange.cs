namespace SAMI.Apps
{
    /// <summary>
    /// Indicates what kind of date this date time is representing.
    /// </summary>
    public enum DateTimeRange
    {
        /// <summary>
        /// This date time represents a specific time.
        /// </summary>
        SpecificTime,
        /// <summary>
        /// This date time represents now
        /// </summary>
        Now, 
        /// <summary>
        /// This date time represents a day
        /// </summary>
        Day, 
        /// <summary>
        /// This date time represents the early morning (12 AM - 6 AM) of a day
        /// </summary>
        EarlyMorning,
        /// <summary>
        /// This date time represents the morning (6 AM - 12 PM) of a day
        /// </summary>
        Morning, 
        /// <summary>
        /// This date time represents the afternoon (12 PM - 5 PM) of a day
        /// </summary>
        Afternoon, 
        /// <summary>
        /// This date time represents the evening (5 PM - 8 PM) of a day
        /// </summary>
        Evening, 
        /// <summary>
        /// This date time represents the night (8 PM - 12 AM) of a day
        /// </summary>
        Night, 
        /// <summary>
        /// This date time is indeterminate, and means any time.
        /// </summary>
        AnyTime
    }
}
