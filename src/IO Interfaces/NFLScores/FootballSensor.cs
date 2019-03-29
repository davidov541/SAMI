using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Net;
using Codeplex.Data;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Football;
using SAMI.IOInterfaces.Interfaces.Sports;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Football
{
    [ParseableElement("NFLScores", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class FootballSensor : IFootballSensor
    {
        private const String ScoreUrl = "http://www.nfl.com/liveupdate/scorestrip/scorestrip.json";

        public String Name
        {
            get
            {
                return League;
            }
        }

        public String League
        {
            get
            {
                return "NFL";
            }
        }

        public bool IsValid
        {
            get
            {
                try
                {
                    String jsonResults = GetJsonScores();

                    if (!String.IsNullOrEmpty(jsonResults))
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // Eat it because all we want to do is return false
                }

                // Otherwise
                return false;
            }
        }

        public IEnumerable<FootballTeam> Teams
        {
            get
            {
                yield return new FootballTeam("Arizona", "Cardinals", "ARI");
                yield return new FootballTeam("Atlanta", "Falcons", "ATL");
                yield return new FootballTeam("Baltimore", "Ravens", "BAL");
                yield return new FootballTeam("Buffalo", "Bills", "BUF");
                yield return new FootballTeam("Carolina", "Panthers", "CAR");
                yield return new FootballTeam("Chicago", "Bears", "CHI");
                yield return new FootballTeam("Cincinnati", "Bengals", "CIN");
                yield return new FootballTeam("Cleveland", "Browns", "CLE");
                yield return new FootballTeam("Dallas", "Cowboys", "DAL");
                yield return new FootballTeam("Denver", "Broncos", "DEN");
                yield return new FootballTeam("Detroit", "Lions", "DET");
                yield return new FootballTeam("Green Bay", "Packers", "GB");
                yield return new FootballTeam("Houston", "Texans", "HOU");
                yield return new FootballTeam("Indianapolis", "Colts", "IND");
                yield return new FootballTeam("Jacksonville", "Jaguars", "JAC");
                yield return new FootballTeam("Kansas City", "Chiefs", "KC");
                yield return new FootballTeam("Miami", "Dolphins", "MIA");
                yield return new FootballTeam("Minnesota", "Vikings", "MIN");
                yield return new FootballTeam("New England", "Patriots", "NE");
                yield return new FootballTeam("New Orleans", "Saints", "NO");
                yield return new FootballTeam("New York", "Giants", "NYG");
                yield return new FootballTeam("New York", "Jets", "NYJ");
                yield return new FootballTeam("Oakland", "Raiders", "OAK");
                yield return new FootballTeam("Philadelphia", "Eagles", "PHI");
                yield return new FootballTeam("Pittsburgh", "Steelers", "PIT");
                yield return new FootballTeam("San Diego", "Chargers", "SD");
                yield return new FootballTeam("San Francisco", "fourty niners", "SF");
                yield return new FootballTeam("Seattle", "Seahawks", "SEA");
                yield return new FootballTeam("Saint Louis", "Rams", "STL");
                yield return new FootballTeam("Tampa Bay", "Buccaneers", "TB");
                yield return new FootballTeam("Tennessee", "Titans", "TEN");
                yield return new FootballTeam("Washington", "Redskins", "WAS");
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public FootballSensor()
        {
        }

        public void Initialize(IConfigurationManager manager)
        {
        }

        public void Dispose()
        {
        }

        public void AddChild(IParseable component)
        {
        }

        protected virtual String GetJsonScores()
        {
            return new WebClient().DownloadString(ScoreUrl);
        }

        public IEnumerable<FootballGame> LoadAllScores()
        {
            String jsonResults = GetJsonScores();
            while (jsonResults.Contains(",,"))
            {
                jsonResults = jsonResults.Replace(",,", ",\"\",");
            }
            var movieJson = DynamicJson.Parse(jsonResults);

            foreach (var game in movieJson.ss)
            {
                FootballTeamScore awayTeam = new FootballTeamScore()
                {
                    Team = Teams.Single(t => t.Key.Equals(game[4])),
                };
                awayTeam.Score = String.IsNullOrEmpty(game[5]) ? 0 : Int32.Parse(game[5]);
                FootballTeamScore homeTeam = new FootballTeamScore()
                {
                    Team = Teams.Single(t => t.Key.Equals(game[6])),
                };
                homeTeam.Score = String.IsNullOrEmpty(game[7]) ? 0 : Int32.Parse(game[7]);
                FootballGame parsedGame = new FootballGame()
                        {
                            State = GameState.GameHasntStarted,
                            HomeTeamScore = awayTeam,
                            AwayTeamScore = homeTeam,
                        };

                DateTime day = DateTime.Now;
                DayOfWeek desiredDayOfWeek = GetDayOfWeek(game[0]);
                switch (game[2] as String)
                {
                    case "Pregame":
                        parsedGame.State = GameState.GameHasntStarted;
                        while (day.DayOfWeek != desiredDayOfWeek)
                        {
                            day = day.AddDays(1);
                        }
                        parsedGame.StartingTime = GetStartingTime(game[1], day);
                        break;
                    case "Final":
                        parsedGame.State = GameState.Completed;
                        while (day.DayOfWeek != desiredDayOfWeek)
                        {
                            day = day.AddDays(-1);
                        }
                        parsedGame.StartingTime = GetStartingTime(game[1], day);
                        break;
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                        parsedGame.State = GameState.Started;
                        parsedGame.Quarter = Int32.Parse(game[2]);
                        parsedGame.StartingTime = GetStartingTime(game[1], DateTime.Today);
                        String[] timeLeft = (game[3] as String).Split(':');
                        parsedGame.TimeLeft = new TimeSpan(0, Int32.Parse(timeLeft[0]), Int32.Parse(timeLeft[1]));
                        break;
                    default:
                        break;
                }
                yield return parsedGame;
            }
        }

        private static DayOfWeek GetDayOfWeek(String parsedName)
        {
            switch (parsedName)
            {
                case "Mon":
                    return DayOfWeek.Monday;
                case "Tue":
                    return DayOfWeek.Tuesday;
                case "Wed":
                    return DayOfWeek.Wednesday;
                case "Thu":
                    return DayOfWeek.Thursday;
                case "Fri":
                    return DayOfWeek.Friday;
                case "Sat":
                    return DayOfWeek.Saturday;
                case "Sun":
                    return DayOfWeek.Sunday;
                default:
                    break;
            }
            return default(DayOfWeek);
        }

        private static DateTime GetStartingTime(String time, DateTime date)
        {
            TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            DateTimeOffset parsedDateLocal = DateTimeOffset.Parse(String.Format("{0}-{1}-{2} {3}", date.Year, date.Month, date.Day, time), CultureInfo.InvariantCulture).AddHours(12);
            TimeSpan tzOffset = tzInfo.GetUtcOffset(parsedDateLocal.DateTime);
            DateTimeOffset parsedDateTimeZone = new DateTimeOffset(parsedDateLocal.DateTime, tzOffset);
            return parsedDateTimeZone.DateTime;
        }

        public FootballGame LoadLatestScoreForTeam(String teamKey)
        {
            return LoadAllScores().SingleOrDefault(s => s.HomeTeamScore.Team.Key.Equals(teamKey) || s.AwayTeamScore.Team.Key.Equals(teamKey));
        }
    }
}
