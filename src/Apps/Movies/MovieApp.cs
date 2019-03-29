using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Movies;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Movie
{
    [ParseableElement("Movies", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class MovieApp : VoiceActivatedApp<MovieConversation>
    {
        private bool _moviesFound = true;

        public override bool IsValid
        {
            get
            {
                return base.IsValid && ConfigManager != null && ConfigManager.FindAllComponentsOfType<IMovieSensor>().Any() && _moviesFound;
            }
        }

        public override string InvalidMessage
        {
            get
            {
                if (IsValid)
                {
                    return String.Empty;
                }
                else
                {
                    return "No movie databases are available currently.";
                }
            }
        }

        public MovieApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            List<Showtime> showTimes = ConfigManager.FindAllComponentsOfType<IMovieSensor>().SelectMany(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime))).ToList();
            _moviesFound = showTimes.Any();
            if (IsValid)
            {
                Provider = new GrammarProvider(GetMainGrammar, new TimeSpan(1, 0, 0, 0));
            }
        }

        internal XmlGrammar GetMainGrammar()
        {
            if (ConfigManager != null && ConfigManager.FindAllComponentsOfType<IMovieSensor>().Any())
            {
                List<Showtime> showTimes = ConfigManager.FindAllComponentsOfType<IMovieSensor>().SelectMany(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime))).ToList();
                List<String> movieNames = showTimes.Select(showtime => showtime.MovieTitle).Distinct().ToList();
                if (movieNames.Count == 0)
                {
                    // If the movie list is empty, the grammar file becomes invalid.
                    // Therefore, I will add a nonsensical string that will never be detect
                    // to prevent the grammar from erroring.
                    // A better solution should be found for this, but it's not too important
                    // right now.
                    movieNames.Add("XYZ123");
                }
                List<String> theaterNames = showTimes.Select(showtime => showtime.Theater).Distinct().ToList();
                if (theaterNames.Count == 0)
                {
                    // If the movie list is empty, the grammar file becomes invalid.
                    // Therefore, I will add a nonsensical string that will never be detect
                    // to prevent the grammar from erroring.
                    // A better solution should be found for this, but it's not too important
                    // right now.
                    theaterNames.Add("XYZ123");
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigManager.GetPathForFile("MovieGrammar.grxml", GetType()));
                XmlElement oneof = GrammarUtility.CreateListOfPossibleStrings(doc, movieNames);
                XmlElement rule = GrammarUtility.CreateElement(doc, "rule", new Dictionary<string, string>
                {
                    {"id", "MovieName"},
                    {"scope", "public"},
                });
                rule.AppendChild(oneof);
                doc.LastChild.AppendChild(rule);

                oneof = GrammarUtility.CreateListOfPossibleStrings(doc, theaterNames);
                rule = GrammarUtility.CreateElement(doc, "rule", new Dictionary<string, string>
                {
                    {"id", "TheaterName"},
                    {"scope", "public"},
                });
                rule.AppendChild(oneof);
                doc.LastChild.AppendChild(rule);

                return new XmlGrammar(doc);
            }
            return null;
        }
    }
}
