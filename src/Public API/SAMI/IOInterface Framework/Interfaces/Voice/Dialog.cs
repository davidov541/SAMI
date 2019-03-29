using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Represents a single recognized phrase, along with the data
    /// the phrase contains.
    /// </summary>
    public class Dialog
    {
        /// <summary>
        /// Contains the data which this individual phrase contains.
        /// The key of the dictionary is the name of the piece of data,
        /// and the value is the value of that property name.
        /// </summary>
        internal Dictionary<String, String> PropVals
        {
            get;
            set;
        }

        /// <summary>
        /// The full string of the phrase that was actually detected.
        /// </summary>
        public String PhraseRecognized
        {
            get;
            private set;
        }

        /// <summary>
        /// Dialog constructor.
        /// </summary>
        /// <param name="propVals">The data that the phrase represents.</param>
        /// <param name="phraseRecognized">The actual phrase recognized.</param>
        public Dialog(Dictionary<String, String> propVals, String phraseRecognized)
        {
            PropVals = propVals;
            PhraseRecognized = phraseRecognized;
        }

        /// <summary>
        /// Gets the data for the given property. If the property was not set,
        /// String.Empty is returned.
        /// </summary>
        /// <param name="propertyName">Name of the property to query.</param>
        /// <returns>The value of the property, or String.Empty if not available.</returns>
        public String GetPropertyValue(String propertyName)
        {
            if(PropVals.ContainsKey(propertyName))
            {
                return PropVals[propertyName];
            }
            return String.Empty;
        }

        /// <summary>
        /// Checks to see if this dialog is for the given app, 
        /// by checking the Command property on this dialog.
        /// </summary>
        /// <param name="appName">Name of the app that is being checked for.</param>
        /// <returns>True if the Command property matches the given app name.</returns>
        public bool CheckForApp(String appName)
        {
            return PropVals.ContainsKey("Command") && PropVals["Command"].Equals(appName);
        }
    }
}
