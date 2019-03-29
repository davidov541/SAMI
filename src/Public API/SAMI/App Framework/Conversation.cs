using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps
{
    /// <summary>
    /// Represents a cohesive set of Dialogs between SAMI and the user.
    /// Conversations are created when a matching phrase is recognized from
    /// the user, and stays in memory until ConversationIsOver is set.
    /// This class should be overriden by anyone making an App.
    /// </summary>
    public abstract class Conversation : IDisposable
    {
        /// <summary>
        /// Instance of the configuration manager to use in order to access
        /// information about SAMI's current configuration.
        /// </summary>
        protected IConfigurationManager ConfigManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates when this conversation should be disposed of and removed from being active.
        /// Starts out as false, and is set to true by the Conversation class itself.
        /// </summary>
        public bool ConversationIsOver
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicates the name of the grammar rule that should be used for the next user input.
        /// By default, this is the name of the rule for all apps, but this value should
        /// be changed if in the middle of a multi-dialog conversation.
        /// </summary>
        public virtual String GrammarRuleName
        {
            get
            {
                return GrammarUtility.MainGrammarName;
            }
        }

        /// <summary>
        /// Indicates the last Dialog that this conversation has seen.
        /// Use this when getting information about the phrase that should
        /// be processed currently.
        /// </summary>
        protected Dialog CurrentDialog
        {
            get
            {
                return CurrentDialogStack.Peek();
            }
        }

        /// <summary>
        /// Value of Command, supplied by the grammar, for which
        /// this conversation should be created.
        /// </summary>
        protected abstract String CommandName
        {
            get;
        }

        internal bool ReadyToSpeak
        {
            get;
            set;
        }

        private DateTime _expirationDate;
        internal bool HasExpired
        {
            get
            {
                return _expirationDate.CompareTo(DateTime.Now) < 0;
            }
        }

        private Stack<Dialog> CurrentDialogStack
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor for Conversation.
        /// </summary>
        public Conversation(IConfigurationManager configManager)
            : this(configManager, DateTime.Now.AddMinutes(15), false)
        {
        }

        /// <summary>
        /// Constructor for Conversation which indicates the time at which
        /// this conversation will automatically dispose itself.
        /// </summary>
        /// <param name="expirationDate">Time at which this Conversation will automatically dispose of itself.</param>
        public Conversation(IConfigurationManager configManager, DateTime expirationDate)
            : this(configManager, expirationDate, false)
        {
        }

        /// <summary>
        /// Constructor for Conversation which indicates whether the Conversation
        /// can start working immediately, even if no user interaction has occurred.
        /// </summary>
        /// <param name="canStartSpeakingImmediately">If true, Speak() will be called on this conversation as soon as the conversation is processed.</param>
        public Conversation(IConfigurationManager configManager, bool canStartSpeakingImmediately)
            : this(configManager, DateTime.Now.AddMinutes(15), canStartSpeakingImmediately)
        {
        }

        /// <summary>
        /// Full constructor for Conversation.
        /// </summary>
        /// <param name="expirationDate">Time at which this Conversation will automatically dispose of itself.</param>
        /// <param name="canStartSpeakingImmediately">If true, Speak() will be called on this conversation as soon as the conversation is processed.</param>
        public Conversation(IConfigurationManager configManager, DateTime expirationDate, bool canStartSpeakingImmediately)
        {
            _expirationDate = expirationDate;
            ReadyToSpeak = canStartSpeakingImmediately;
            CurrentDialogStack = new Stack<Dialog>();
            ConfigManager = configManager;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Tries to add the given dialog to this conversation.
        /// </summary>
        /// <param name="phrase">Dialog to add to the conversation.</param>
        /// <returns>True if the dialog could be added, false otherwise.</returns>
        public bool TryAddDialog(Dialog phrase)
        {
            if (phrase.CheckForApp(CommandName))
            {
                ReadyToSpeak = true;
                CurrentDialogStack.Push(phrase);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Processes the last input, returning what should be said back to the user.
        /// </summary>
        /// <returns>Phrase which should be said back to the user.</returns>
        public virtual String Speak()
        {
            ReadyToSpeak = false;
            return String.Empty;
        }

        #region Parsing Utilities
        /// <summary>
        /// Parses the time from the given string.
        /// </summary>
        /// <param name="timeString">The time, represented as a string.</param>
        /// <returns>The datetime passed in, represented as a SamiDateTime.</returns>
        protected SamiDateTime ParseTime(String timeString)
        {
            return ParseTime(GenDictionaryFromString(timeString));
        }

        /// <summary>
        /// Parses a location from the given string.
        /// </summary>
        /// <param name="location">The location represented as a String.</param>
        /// <returns>The location passed in, representd as a Location.</returns>
        protected Location ParseLocation(String location)
        {
            switch (location.ToLower())
            {
                case "here":
                    return ConfigManager.LocalLocation;
                case "austin":
                    return new Location("Austin", "TX", 0);
                case "cincinnati":
                    return new Location("Cincinnati", "OH", 0);
                case "dayton":
                    return new Location("Dayton", "OH", 0);
                default:
                    return new Location("Cincinnati", "OH", Int32.Parse(location));
            }
        }

        /// <summary>
        /// Returns a string to represent the number passed in.
        /// The string will be formatted so that SAMI will properly say
        /// the number to the user.
        /// </summary>
        /// <param name="num">Number to say to the user.</param>
        /// <returns>A string which represents the given number.</returns>
        protected String SayOrdinal(int num)
        {
            String response = String.Empty;
            switch ((num / 10))
            {
                case 0:
                case 1:
                    break;
                case 2:
                    response += "twenty ";
                    num = num % 10;
                    break;
                case 3:
                    response += "thirty ";
                    num = num % 10;
                    break;
                case 4:
                    response += "fourty ";
                    num = num % 10;
                    break;
                case 5:
                    response += "fifty ";
                    num = num % 10;
                    break;
                case 6:
                    response += "sixty ";
                    num = num % 10;
                    break;
                case 7:
                    response += "seventy ";
                    num = num % 10;
                    break;
                case 8:
                    response += "eighty ";
                    num = num % 10;
                    break;
                case 9:
                    response += "ninety ";
                    num = num % 10;
                    break;
            }
            switch (num)
            {
                case 0:
                    response += "th";
                    break;
                case 1:
                    response += "first";
                    break;
                case 2:
                    response += "second";
                    break;
                case 3:
                    response += "third";
                    break;
                case 4:
                    response += "fourth";
                    break;
                case 5:
                    response += "fifth";
                    break;
                case 6:
                    response += "sixth";
                    break;
                case 7:
                    response += "seventh";
                    break;
                case 8:
                    response += "eighth";
                    break;
                case 9:
                    response += "ninth";
                    break;
                case 10:
                    response += "tenth";
                    break;
                case 11:
                    response += "eleventh";
                    break;
                case 12:
                    response += "twelth";
                    break;
                case 13:
                    response += "thirteenth";
                    break;
                case 14:
                    response += "fourteenth";
                    break;
                case 15:
                    response += "fifteenth";
                    break;
                case 16:
                    response += "sixteenth";
                    break;
                case 17:
                    response += "seventeenth";
                    break;
                case 18:
                    response += "eighteenth";
                    break;
                case 19:
                    response += "ninteenth";
                    break;
                default:
                    break;
            }
            return response;
        }

        /// <summary>
        /// Converts a TimeSpan into a string which is meant to be said.
        /// The calling function can decide if the number needs to be pluralized.
        /// </summary>
        /// <param name="timeInMs">The time to be said, as a TimeSpan.</param>
        /// <param name="usePlural">If true, the number units are pluralized</param>
        /// <returns>A string which represents the given time.</returns>
        protected String SayTime(TimeSpan timeSpan, bool usePlural)
        {

            String val = String.Empty;
            if (timeSpan.Hours > 0)
            {
                if (timeSpan.Hours == 1 || !usePlural)
                {
                    val += String.Format("{0} hour", timeSpan.Hours);
                }
                else
                {
                    val += String.Format("{0} hours", timeSpan.Hours);

                }
            }

            if (timeSpan.Minutes > 0)
            {
                if (val.Length > 0)
                {
                    val += " and ";
                }
                if (timeSpan.Minutes == 1 || !usePlural)
                {
                    val += String.Format("{0} minute", timeSpan.Minutes);
                }
                else
                {
                    val += String.Format("{0} minutes", timeSpan.Minutes);
                }
            }

            if (timeSpan.Seconds > 0)
            {
                if (val.Length > 0)
                {
                    val += " and ";
                }
                if (timeSpan.Seconds == 1 || !usePlural)
                {
                    val += String.Format("{0} second", timeSpan.Seconds);
                }
                else
                {
                    val += String.Format("{0} seconds", timeSpan.Seconds);
                }
            }

            if (val.Equals(String.Empty))
            {
                val = "0 second";
            }

            return val;
        }

        /// <summary>
        /// Returns a string to represent the list of items passed in.
        /// The String will be formatted so that SAMI will properly say
        /// the list to the user.
        /// </summary>
        /// <param name="list">A list of items to be said. Each item should be properly formatted to say to the user.</param>
        /// <returns>A single string which represents the list passed in.</returns>
        protected String SayList(List<String> list)
        {
            String returnString = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                returnString += list[i];
                if (i < list.Count - 1)
                {
                    returnString += ". ";
                }
                if (i == list.Count - 2)
                {
                    returnString += "and ";
                }
            }
            return returnString;
        }

        private SamiDateTime ParseTime(Dictionary<string, string> dict)
        {
            DateTime time = DateTime.Today;
            DateTimeRange range = DateTimeRange.Day;


            // See if we want to return now
            if (dict.ContainsKey("TimeOfDay"))
            {
                if (dict["TimeOfDay"].Equals("now"))
                {
                    return new SamiDateTime(DateTime.Now, DateTimeRange.Now);
                }
            }

            // Get a relative date or day of week
            int deltaDay = 0;
            if (dict.ContainsKey("DayOfWeek"))
            {
                switch (dict["DayOfWeek"])
                {
                    case "today":
                        // time is already DateTime.Today
                        break;
                    case "tomorrow":
                        time = time.AddDays(1);
                        break;
                    case "yesterday":
                        time = time.AddDays(-1);
                        break;
                    default:
                        deltaDay = Int32.Parse(dict["DayOfWeek"]) - (int)(time.DayOfWeek);
                        if (deltaDay < 0)
                        {
                            deltaDay += 7;
                        }
                        time = time.AddDays(deltaDay);
                        break;
                }
            }
            else if (dict.ContainsKey("Month") && dict.ContainsKey("Day"))
            {
                // Specifying both a month and a day
                time = time.AddMonths(Int32.Parse(dict["Month"]) - time.Month);
                time = time.AddDays(Int32.Parse(dict["Day"]) - time.Day);
                if (time < DateTime.Today)
                {
                    time = time.AddYears(1);
                }
            }
            else if (!dict.ContainsKey("Month") && dict.ContainsKey("Day"))
            {
                // Only contains a day
                int day = Int32.Parse(dict["Day"]);
                if (day > 31)
                {
                    day = 31;
                }
                time = time.AddDays(-time.Day);
                while (day <= DateTime.DaysInMonth(time.Year, time.Month) && time.AddDays(day) >= DateTime.Today)
                {
                    time = time.AddMonths(1);
                }
                time = time.AddDays(day);
            }

            // Time
            if (dict.ContainsKey("Hours") && dict.ContainsKey("Minutes"))
            {
                range = DateTimeRange.SpecificTime;
                int hours = Int32.Parse(dict["Hours"]);
                if (hours == 12)
                {
                    // Loop back since 12 is the start of the day
                    hours = 0;
                }
                int minutes = Int32.Parse(dict["Minutes"]);
                if (dict["TimeOfDay"].Equals(""))
                {
                    time = time.AddHours(hours).AddMinutes(minutes);
                    while (time < DateTime.Now)
                    {
                        time = time.AddHours(12);
                    }
                }
                else if (dict["TimeOfDay"].Equals("am"))
                {
                    time = time.AddHours(hours).AddMinutes(minutes);
                }
                else if (dict["TimeOfDay"].Equals("pm"))
                {
                    time = time.AddHours(hours + 12).AddMinutes(minutes);
                }
                else if (dict["TimeOfDay"].Equals("midnight"))
                {
                    // Assuming midnight is the end of the day, not the start of the day.
                    time = time.AddHours(24);
                }
            }
            else if (dict.ContainsKey("TimeOfDay"))
            {
                switch (dict["TimeOfDay"])
                {
                    case "earlymorning":
                        range = DateTimeRange.EarlyMorning;
                        break;
                    case "morning":
                        range = DateTimeRange.Morning;
                        break;
                    case "afternoon":
                        range = DateTimeRange.Afternoon;
                        break;
                    case "evening":
                        range = DateTimeRange.Evening;
                        break;
                    case "night":
                        range = DateTimeRange.Night;
                        break;
                }
            }

            return new SamiDateTime(time, range);
        }

        private static Dictionary<String, String> GenDictionaryFromString(String s)
        {
            Dictionary<String, String> dict = new Dictionary<String, String>();

            foreach (String param in s.Split(';'))
            {
                int seperatorIndex = param.IndexOf('=');
                if (seperatorIndex > 0)
                {
                    String key = param.Substring(0, seperatorIndex);
                    String value;
                    if (seperatorIndex == param.Length - 1)
                    {
                        value = "";
                    }
                    else
                    {
                        value = param.Substring(seperatorIndex + 1, param.Length - seperatorIndex - 1);
                    }
                    dict.Add(key, value);
                }
            }
            return dict;
        }
        #endregion
    }
}
