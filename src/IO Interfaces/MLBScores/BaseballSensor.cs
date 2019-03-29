using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Baseball;
using SAMI.IOInterfaces.Interfaces.Sports;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Baseball
{
    [ParseableElement("MLBScores", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class BaseballSensor : IBaseballSensor
    {
        private MLBStandings _dailyStandings;
        private DateTime _lastTimeStandingsUpdated;

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
                return "MLB";
            }
        }

        public bool IsValid
        {
            get
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://gd2.mlb.com/components/game/mlb");

                    using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                    {
                        if (rsp.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                    }
                }
                catch (WebException)
                {
                    // Eat it because all we want to do is return false
                }

                // Otherwise
                return false;
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

        public BaseballSensor()
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

        public IEnumerable<BaseballTeam> Teams
        {
            get
            {
                yield return new MLBTeam("Cincinnati", "Reds", "Reds", MLBDivision.NLCentral);
                yield return new MLBTeam("Chicago", "Cubs", "Cubs", MLBDivision.NLCentral);
                yield return new MLBTeam("Saint Louis", "Cardinals", "Cardinals", MLBDivision.NLCentral);
                yield return new MLBTeam("Milwaukee", "Brewers", "Brewers", MLBDivision.NLCentral);
                yield return new MLBTeam("Pittsburgh", "Pirates", "Pirates", MLBDivision.NLCentral);
                yield return new MLBTeam("New York", "Mets", "Mets", MLBDivision.NLEast);
                yield return new MLBTeam("Philidelphia", "Phillies", "Phillies", MLBDivision.NLEast);
                yield return new MLBTeam("Atlanta", "Braves", "Braves", MLBDivision.NLEast);
                yield return new MLBTeam("Washington", "Nationals", "Nationals", MLBDivision.NLEast);
                yield return new MLBTeam("Floridia", "Marlins", "Marlins", MLBDivision.NLEast);
                yield return new MLBTeam("Arizona", "Diamondbacks", "D-backs", MLBDivision.NLWest);
                yield return new MLBTeam("San Fransisco", "Giants", "Giants", MLBDivision.NLWest);
                yield return new MLBTeam("Los Angeles", "Dodgers", "Dodgers", MLBDivision.NLWest);
                yield return new MLBTeam("San Diego", "Padres", "Padres", MLBDivision.NLWest);
                yield return new MLBTeam("Colorado", "Rockies", "Rockies", MLBDivision.NLWest);
                yield return new MLBTeam("New York", "Yankees", "Yankees", MLBDivision.ALEast);
                yield return new MLBTeam("Boston", "Red Socks", "Red Sox", MLBDivision.ALEast);
                yield return new MLBTeam("Tampa Bay", "Rays", "Rays", MLBDivision.ALEast);
                yield return new MLBTeam("Toronto", "Blue Jays", "Blue Jays", MLBDivision.ALEast);
                yield return new MLBTeam("Baltimore", "Orioles", "Orioles", MLBDivision.ALEast);
                yield return new MLBTeam("Chicago", "White Socks", "White Sox", MLBDivision.ALCentral);
                yield return new MLBTeam("Detroit", "Tigers", "Tigers", MLBDivision.ALCentral);
                yield return new MLBTeam("Cleveland", "Indians", "Indians", MLBDivision.ALCentral);
                yield return new MLBTeam("Minnesota", "Twins", "Twins", MLBDivision.ALCentral);
                yield return new MLBTeam("Royals", "Royals", "Royals", MLBDivision.ALCentral);
                yield return new MLBTeam("Seattle", "Mariners", "Mariners", MLBDivision.ALWest);
                yield return new MLBTeam("Oakland", "Athletics", "Athletics", MLBDivision.ALWest);
                yield return new MLBTeam("Anaheim", "Angels", "Angels", MLBDivision.ALWest);
                yield return new MLBTeam("Texas", "Rangers", "Rangers", MLBDivision.ALWest);
                yield return new MLBTeam("Houston", "Astros", "Astros", MLBDivision.ALWest);
            }
        }

        public IEnumerable<BaseballGame> LoadAllScores(DateTime date)
        {
            XmlDocument scoreboardDoc = new XmlDocument();
            scoreboardDoc.Load(GetScoreboardFile(date));
            XmlDocument masterScoreboardDoc = new XmlDocument();
            masterScoreboardDoc.Load(GetMasterScoreboardFile(date));
            XmlDocument epgDoc = new XmlDocument();
            epgDoc.Load(GetEpgFile(date));
            foreach (XmlElement gameXml in scoreboardDoc.SelectSingleNode("scoreboard").ChildNodes)
            {
                String id = gameXml.SelectSingleNode("game").Attributes["id"].Value;
                String masterID = ConvertIdToMaster(id);
                XmlElement masterGameXml = masterScoreboardDoc.SelectSingleNode(String.Format("games/game[@id='{0}']", masterID)) as XmlElement;
                XmlElement epgGameXml = epgDoc.SelectSingleNode(String.Format("epg/game[@id='{0}']", masterID)) as XmlElement;

                BaseballGame game = new BaseballGame();
                // Postponed games will not have the start time labelled on it.
                if (epgGameXml.HasAttribute("start") && !String.IsNullOrEmpty(epgGameXml.Attributes["start"].Value))
                {
                    game.StartingTime = DateTime.Parse(epgGameXml.Attributes["start"].Value).ToLocalTime();
                }

                BaseballTeamScore score1 = new BaseballTeamScore();
                XmlElement teamScore = gameXml.SelectNodes("team")[0] as XmlElement;
                score1.Team = Teams.Single(t => t.Key.Equals(teamScore.Attributes["name"].Value));
                score1.Score = Int32.Parse(teamScore.SelectSingleNode("gameteam").Attributes["R"].Value);

                BaseballTeamScore score2 = new BaseballTeamScore();
                teamScore = gameXml.SelectNodes("team")[1] as XmlElement;
                score2.Team = Teams.Single(t => t.Key.Equals(teamScore.Attributes["name"].Value));
                score2.Score = Int32.Parse(teamScore.SelectSingleNode("gameteam").Attributes["R"].Value);

                if (score1.Score > score2.Score)
                {
                    game.WinningTeamScore = score1;
                    game.LosingTeamScore = score2;
                }
                else
                {
                    game.WinningTeamScore = score2;
                    game.LosingTeamScore = score1;
                }

                if (score1.Team.Equals(masterGameXml.Attributes["away_team_name"].Value))
                {
                    game.AwayTeamScore = score1;
                    game.HomeTeamScore = score2;
                }
                else
                {
                    game.HomeTeamScore = score1;
                    game.AwayTeamScore = score2;
                }

                List<String> channels = new List<String>();
                if (masterGameXml.SelectNodes("broadcast/home").Count > 0)
                {
                    channels.AddRange(GetChannels(masterGameXml.SelectSingleNode("broadcast/home") as XmlElement));
                }
                if (masterGameXml.SelectNodes("broadcast/away").Count > 0)
                {
                    channels.AddRange(GetChannels(masterGameXml.SelectSingleNode("broadcast/away") as XmlElement));
                }
                if (epgGameXml.SelectNodes("game_media/media").Count > 0 && epgGameXml.SelectSingleNode("game_media/media").Attributes["free"].Value.Equals("ALL"))
                {
                    channels.Add("MLB.tv");
                }
                game.Channels = channels;

                String state = masterGameXml.SelectSingleNode("status").Attributes["status"].Value;
                if (state.Equals("Final") ||
                    state.Equals("Game Over"))
                {
                    game.State = GameState.Completed;
                }
                else if (state.Equals("Preview"))
                {
                    game.State = GameState.GameHasntStarted;
                }
                else if (state.Equals("In Progress"))
                {
                    game.NumberOfOuts = Int32.Parse(gameXml.Attributes["outs"].Value);
                    game.NumberOnBase = gameXml.SelectNodes("on_base").Count;
                    game.InningNumber = Int32.Parse(gameXml.SelectSingleNode("inningnum").Attributes["inning"].Value);
                    game.TopOfInning = gameXml.SelectSingleNode("inningnum").Attributes["half"].Equals("T");
                    game.State = GameState.Started;
                }
                else if (state.Equals("Postponed"))
                {
                    game.State = GameState.Postponed;
                }
                else
                {
                    game.State = GameState.RainDelay;
                }
                yield return game;
            }
        }

        private static IEnumerable<String> GetChannels(XmlElement channelElement)
        {
            String tvList = channelElement.SelectSingleNode("tv").InnerText;
            foreach (String tvChannel in tvList.Split(','))
            {
                yield return tvChannel.Trim();
            }
        }

        private static String ConvertIdToMaster(String baseID)
        {
            StringBuilder convertedString = new StringBuilder(baseID);
            int firstIndex = baseID.IndexOf('_');
            int secondIndex = baseID.IndexOf('_', firstIndex + 1);
            int thirdIndex = baseID.IndexOf('_', secondIndex + 1);
            convertedString[firstIndex] = '/';
            convertedString[secondIndex] = '/';
            convertedString[thirdIndex] = '/';
            convertedString.Replace('_', '-');
            return convertedString.ToString();
        }

        public IEnumerable<BaseballGame> LoadScoresForTeam(DateTime date, BaseballTeam team)
        {
            foreach (BaseballGame game in LoadAllScores(date))
            {
                if (game.WinningTeamScore.Team.Key.Equals(team.Key) ||
                    game.LosingTeamScore.Team.Key.Equals(team.Key))
                {
                    yield return game;
                }
            }
        }

        public BaseballStandings LoadStandings()
        {
            if (_lastTimeStandingsUpdated == null || !_lastTimeStandingsUpdated.Date.Equals(DateTime.Now.Date))
            {
                UpdateDailyStandings();
            }
            return _dailyStandings;
        }

        private void UpdateDailyStandings()
        {
            XmlDocument doc = new XmlDocument();
            MLBStandings standings = new MLBStandings();
            DateTime currDate = DateTime.Now;

            while (DateTime.Now.Subtract(currDate).Days < 7)
            {
                currDate = currDate.Subtract(TimeSpan.FromDays(1));
                doc.Load(GetEpgFile(currDate));
                if (doc == null)
                {
                    break;
                }
                foreach (XmlElement game in doc.SelectNodes("/epg/game"))
                {
                    BaseballTeamStanding homeDivStanding = new BaseballTeamStanding();
                    BaseballTeamStanding homeWildcardStanding = new BaseballTeamStanding();
                    String homeTeamName = game.Attributes["home_team_name"].Value;
                    BaseballTeam homeTeam = Teams.SingleOrDefault(t => t.Key.Equals(homeTeamName));
                    if (!standings.TryGetStandingsForTeam(homeTeam, out homeDivStanding, out homeWildcardStanding) &&
                        !String.IsNullOrEmpty(game.Attributes["home_games_back"].Value) &&
                        !String.IsNullOrEmpty(game.Attributes["home_games_back_wildcard"].Value))
                    {
                        homeDivStanding = new BaseballTeamStanding();
                        homeWildcardStanding = new BaseballTeamStanding();
                        homeDivStanding.Team = homeTeam;
                        homeWildcardStanding.Team = homeTeam;

                        String gamesBack = game.Attributes["home_games_back"].Value;
                        if (gamesBack.Equals("-"))
                        {
                            homeDivStanding.GamesBack = 0;
                        }
                        else
                        {
                            homeDivStanding.GamesBack = -1 * Double.Parse(gamesBack);
                        }

                        gamesBack = game.Attributes["home_games_back_wildcard"].Value;
                        if (gamesBack.Equals("-"))
                        {
                            homeWildcardStanding.GamesBack = 0;
                        }
                        else if (gamesBack.StartsWith("+"))
                        {
                            homeWildcardStanding.GamesBack = Double.Parse(gamesBack);
                        }
                        else
                        {
                            homeWildcardStanding.GamesBack = -1 * Double.Parse(gamesBack);
                        }

                        standings.AddStandingsForTeam(homeDivStanding, homeWildcardStanding, (Teams.SingleOrDefault(t => t.Key.Equals(homeTeamName)) as MLBTeam).Division);
                    }

                    BaseballTeamStanding awayDivStanding = new BaseballTeamStanding();
                    BaseballTeamStanding awayWildcardStanding = new BaseballTeamStanding();
                    String awayTeamName = game.Attributes["away_team_name"].Value;
                    BaseballTeam awayTeam = Teams.SingleOrDefault(t => t.Key.Equals(awayTeamName));
                    if (!standings.TryGetStandingsForTeam(awayTeam, out awayDivStanding, out awayWildcardStanding) &&
                        !String.IsNullOrEmpty(game.Attributes["away_games_back"].Value))
                    {
                        awayDivStanding = new BaseballTeamStanding();
                        awayWildcardStanding = new BaseballTeamStanding();
                        awayDivStanding.Team = awayTeam;
                        awayWildcardStanding.Team = awayTeam;

                        String gamesBack = game.Attributes["away_games_back"].Value;
                        if (gamesBack.Equals("-"))
                        {
                            awayDivStanding.GamesBack = 0;
                        }
                        else
                        {
                            awayDivStanding.GamesBack = -1 * Double.Parse(gamesBack);
                        }

                        gamesBack = game.Attributes["away_games_back_wildcard"].Value;
                        if (gamesBack.Equals("-") || String.IsNullOrEmpty(gamesBack))
                        {
                            awayWildcardStanding.GamesBack = 0;
                        }
                        else if (gamesBack.StartsWith("+"))
                        {
                            awayWildcardStanding.GamesBack = Double.Parse(gamesBack);
                        }
                        else
                        {
                            awayWildcardStanding.GamesBack = -1 * Double.Parse(gamesBack);
                        }

                        standings.AddStandingsForTeam(awayDivStanding, awayWildcardStanding, (Teams.SingleOrDefault(t => t.Key.Equals(awayTeamName)) as MLBTeam).Division);
                    }
                }
            }
            _lastTimeStandingsUpdated = DateTime.Now;
            _dailyStandings = standings;
        }

        protected virtual String GetScoreboardFile(DateTime date)
        {
            return String.Format("http://gd2.mlb.com/components/game/mlb/year_{0:0000}/month_{1:00}/day_{2:00}/scoreboard.xml", date.Year, date.Month, date.Day);
        }

        protected virtual String GetMasterScoreboardFile(DateTime date)
        {
            return String.Format("http://gd2.mlb.com/components/game/mlb/year_{0:0000}/month_{1:00}/day_{2:00}/master_scoreboard.xml", date.Year, date.Month, date.Day);
        }

        protected virtual String GetEpgFile(DateTime date)
        {
            return String.Format("http://gd2.mlb.com/components/game/mlb/year_{0:0000}/month_{1:00}/day_{2:00}/epg.xml", date.Year, date.Month, date.Day);
        }
    }
}
