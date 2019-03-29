using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Baseball;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Interfaces.Sports;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Baseball
{
    internal class BaseballConversation : Conversation
    {
        private IEnumerable<IOInterfaceReference> _remotes;

        protected override String CommandName
        {
            get
            {
                return "baseball";
            }
        }

        public BaseballConversation(IConfigurationManager configManager, IEnumerable<IOInterfaceReference> remotes)
            : base(configManager)
        {
            _remotes = remotes;
        }

        public override string Speak()
        {
            Dialog phrase = CurrentDialog;
            SamiDateTime time = ParseTime(phrase.GetPropertyValue("Time"));
            String teamName = phrase.GetPropertyValue("Team");
            GameParameter parameter = (GameParameter)Enum.Parse(typeof(GameParameter), phrase.GetPropertyValue("Parameter"));
            ConversationIsOver = true;

            base.Speak();

            IBaseballSensor sensor = ConfigManager.FindAllComponentsOfType<IBaseballSensor>().FirstOrDefault(s => s.Teams.Any(t => t.Key.Equals(teamName)));
            if (sensor != null)
            {
                BaseballTeam team = sensor.Teams.Single(t => t.Key.Equals(teamName));

                switch (parameter)
                {
                    case GameParameter.Score:
                        return RespondForAllGames(sensor, time.Time, team);
                    case GameParameter.Standings:
                        return RespondToStandings(sensor, team);
                    case GameParameter.MLBTVFreeGame:
                        return RespondToMLBTVGame(sensor);
                    case GameParameter.TVChannel:
                        return RespondToTVChannel(sensor, team, time.Time);
                    case GameParameter.TurnToGame:
                        return RespondToTurnToGame(sensor, team, time.Time);
                    default:
                        return String.Empty;
                }
            }
            else if (parameter == GameParameter.MLBTVFreeGame)
            {
                sensor = ConfigManager.FindAllComponentsOfType<IBaseballSensor>().FirstOrDefault(s => s.League.Equals("MLB"));
                if (sensor != null)
                {
                    return RespondToMLBTVGame(sensor);
                }
            }
            return String.Empty;
        }

        #region Response Methods
        private String RespondToTurnToGame(IBaseballSensor sensor, BaseballTeam team, DateTime time)
        {
            BaseballGame lastGame = null;
            foreach (BaseballGame game in sensor.LoadScoresForTeam(time, team))
            {
                List<String> possibleChannels = GetChannelsGameIsOn(sensor, game);
                lastGame = game;
                switch (game.State)
                {
                    case GameState.NoGame:
                        return String.Format("The {0} are not playing today.", team.Name);
                    case GameState.GameHasntStarted:
                        if (possibleChannels.Any())
                        {
                            return String.Format("The {0} will be playing at {1}.", team.Name, game.StartingTime.ToShortTimeString());
                        }
                        else
                        {
                            return String.Format("The {0} game will not be available on national TV.", team.Name);
                        }
                    case GameState.RainDelay:
                        if (possibleChannels.Any())
                        {
                            return String.Format("The {0} game is currently in a rain delay, but is available on ESPN.", team.Name);
                        }
                        else
                        {
                            return String.Format("The {0} game is not available on national TV.", team.Name);
                        }
                    case GameState.Started:
                        if (possibleChannels.Any() && _remotes.Any())
                        {
                            IEnumerable<String> possibleRemoteNames = _remotes.Select(r => r.Name);
                            IEnumerable<ITVRemote> remotes = ConfigManager.FindAllComponentsOfType<ITVRemote>();
                            foreach (ITVRemote remote in remotes)
                            {
                                if (possibleRemoteNames.Contains(remote.Name) &&
                                    possibleChannels.Any(c => remote.GetChannels().Contains(c)))
                                {
                                    Task.Run(() => remote.SendChannel(possibleChannels.First(c => remote.GetChannels().Contains(c))));
                                }
                            }
                            return "OK";
                        }
                        else if (possibleChannels.Any())
                        {
                            return String.Format("There is no T V remote connected to me for me to use. The {0} game is on {1}.", team.Name, SayList(possibleChannels));
                        }
                        else
                        {
                            return String.Format("The {0} game is not on a national broadcast!", team.Name);
                        }
                    case GameState.Postponed:
                    case GameState.Completed:
                        break;
                }
            }
            if (lastGame != null)
            {
                if (lastGame.State == GameState.Postponed)
                {
                    return RespondForOneGame(lastGame, team);
                }
                return String.Format("The game has already completed. {0}", RespondForOneGame(lastGame, team));
            }
            return String.Format("The {0} are not playing today.", team.Name);
        }

        private String RespondToTVChannel(IBaseballSensor sensor, BaseballTeam team, DateTime time)
        {
            BaseballGame lastGame = null;
            foreach (BaseballGame game in sensor.LoadScoresForTeam(time, team))
            {
                List<String> possibleChannels = GetChannelsGameIsOn(sensor, game);
                lastGame = game;
                switch (game.State)
                {
                    case GameState.NoGame:
                        return String.Format("The {0} are not playing today.", team.Name);
                    case GameState.GameHasntStarted:
                        if (!possibleChannels.Any())
                        {
                            return String.Format("The {0} game will not be available on national TV.", team.Name);
                        }
                        else
                        {
                            return String.Format("The {0} game will be available on {1} at {2}", team.Name, SayList(possibleChannels), game.StartingTime.ToShortTimeString());
                        }
                    case GameState.RainDelay:
                        if (!possibleChannels.Any())
                        {
                            return String.Format("The {0} game is not available on national TV.", team.Name);
                        }
                        else
                        {
                            return String.Format("The {0} game is currently in a rain delay, but is available on {1}", team.Name, SayList(possibleChannels));
                        }
                    case GameState.Started:
                        if (!possibleChannels.Any())
                        {
                            return String.Format("The {0} game is not available on national TV.", team.Name);
                        }
                        else
                        {
                            return String.Format("The {0} game is available on {1}.", team.Name, SayList(possibleChannels));
                        }
                }
            }
            if (lastGame != null)
            {
                if (lastGame.State == GameState.Postponed)
                {
                    return RespondForOneGame(lastGame, team);
                }
                return String.Format("The game has already completed. {0}", RespondForOneGame(lastGame, team));
            }
            return String.Format("The {0} are not playing today.", team.Name);
        }

        private String RespondToMLBTVGame(IBaseballSensor sensor)
        {
            BaseballGame game = sensor.LoadAllScores(DateTime.Now).SingleOrDefault(g => g.Channels.Any(c => c.Equals("MLB.tv")));
            if (game != null)
            {
                switch (game.State)
                {
                    case GameState.NoGame:
                        return String.Format("Error, something went wrong!");
                    case GameState.GameHasntStarted:
                        return String.Format("The {0} at {1} game at {2} today is free on MLB.TV.",
                            game.AwayTeamScore.Team.Name,
                            game.HomeTeamScore.Team.Name,
                            game.StartingTime.ToShortTimeString());
                    case GameState.RainDelay:
                        return String.Format("The {0} at {1} game, which is currently under a rain delay, is free on MLB.TV.",
                            game.AwayTeamScore.Team.Name,
                            game.HomeTeamScore.Team.Name);
                    case GameState.Started:
                        return String.Format("The {0} at {1} game playing currently is free on MLB.TV.",
                            game.AwayTeamScore.Team.Name,
                            game.HomeTeamScore.Team.Name);
                    case GameState.Completed:
                        return "The free game today has already completed.";
                }
            }
            return "There are no free games on MLB.TV available today!";
        }

        private String RespondToStandings(IBaseballSensor sensor, BaseballTeam team)
        {
            BaseballStandings standings = sensor.LoadStandings();
            BaseballTeamStanding divisionStandings, wildcardStandings = null;
            BaseballStandingsWithWildcard fullStandings = standings as BaseballStandingsWithWildcard;

            bool gotStandingsSuccessful = false;
            if (fullStandings != null)
            {
                gotStandingsSuccessful = fullStandings.TryGetStandingsForTeam(team, out divisionStandings, out wildcardStandings);
            }
            else
            {
                gotStandingsSuccessful = standings.TryGetStandingsForTeam(team, out divisionStandings);
            }

            if (!gotStandingsSuccessful)
            {
                return String.Format("I couldn't find any information about the {0} this year.", team.Name);
            }
            else if (divisionStandings.GamesBack == 0)
            {
                return String.Format("The {0} lead the {1}.", team.Name, team.Division);
            }
            else if (wildcardStandings == null)
            {
                return String.Format("The {0} are {1} games behind the leaders in the {2}.",
                    team.Name, divisionStandings.GamesBack * -1, team.Division);
            }
            else if (wildcardStandings.GamesBack > 0)
            {
                return String.Format("The {0} are {1} games behind the leaders in the {2}, and lead the wildcard standings by {3} games.",
                    team.Name, divisionStandings.GamesBack * -1, team.Division, wildcardStandings.GamesBack);
            }
            else if (wildcardStandings.GamesBack == 0)
            {
                return String.Format("The {0} are {1} games behind the leaders in the {2}, and just barely have a wildcard spot.",
                    team.Name, divisionStandings.GamesBack * -1, team.Division);
            }
            else
            {
                return String.Format("The {0} are {1} games behind the leaders in the {2}, and are {3} games behind in the wildcard standings.",
                    team.Name, divisionStandings.GamesBack * -1, team.Division, wildcardStandings.GamesBack * -1);
            }
        }

        private String RespondForAllGames(IBaseballSensor sensor, DateTime time, BaseballTeam team)
        {
            IEnumerable<BaseballGame> scores = sensor.LoadScoresForTeam(time, team);

            if (!scores.Any())
            {
                return String.Format("The {0} are not playing today.", team.Name);
            }
            if (scores.Count() == 1)
            {
                return RespondForOneGame(scores.First(), team);
            }
            else if (scores.Count() == 2)
            {
                return "In the first game, " + RespondForOneGame(scores.First(), team) +
                    ". In the second game, " + RespondForOneGame(scores.Last(), team) + ".";
            }
            else
            {
                // We shouldn't have more than a doubleheader...
                return String.Empty;
            }
        }
        #endregion

        #region Utility Methods
        private List<String> GetChannelsGameIsOn(IBaseballSensor sensor, BaseballGame game)
        {
            List<String> possibleChannels = new List<string>();
            foreach (String channel in game.Channels)
            {
                switch (channel)
                {
                    case "ESPN":
                        possibleChannels.Add("ESPN");
                        break;
                    case "ESPN2":
                        possibleChannels.Add("ESPN2");
                        break;
                    case "FOX":
                        possibleChannels.Add("Fox");
                        break;
                    case "WGN":
                        possibleChannels.Add("WGN");
                        break;
                    case "MLBN":
                        possibleChannels.Add("The MLB Network");
                        break;
                    case "TBS":
                        possibleChannels.Add("TBS");
                        break;
                    default:
                        break;
                }
            }

            return possibleChannels.Distinct().ToList();
        }

        private String RespondForOneGame(BaseballGame score, BaseballTeam team)
        {
            String startingPhrase = String.Empty;
            switch (score.State)
            {
                case GameState.Started:
                    if (score.WinningTeamScore.Score == score.LosingTeamScore.Score)
                    {
                        if (score.WinningTeamScore.Team.Equals(team))
                        {
                            startingPhrase = String.Format("The {0} and the {1} are tied at {2} a piece. ", team.Name, score.LosingTeamScore.Team.Name, score.WinningTeamScore.Score);
                        }
                        else
                        {
                            startingPhrase = String.Format("The {0} and the {1} are tied at {2} a piece. ", team.Name, score.WinningTeamScore.Team.Name, score.WinningTeamScore.Score);
                        }
                    }
                    else if (score.WinningTeamScore.Team.Equals(team))
                    {
                        startingPhrase = String.Format("The {0} are beating the {1} {2} to {3}. ", team.Name, score.LosingTeamScore.Team.Name, score.WinningTeamScore.Score, score.LosingTeamScore.Score);
                    }
                    else
                    {
                        startingPhrase = String.Format("The {0} are losing to the {1} by a score of {2} to {3}. ", team.Name, score.WinningTeamScore.Team.Name, score.WinningTeamScore.Score, score.LosingTeamScore.Score);
                    }

                    String inningNum = SayOrdinal(score.InningNumber);
                    if (score.TopOfInning)
                    {
                        startingPhrase += String.Format("with {2} on and {0} outs in the top of the {1}.", score.NumberOfOuts, inningNum, score.NumberOnBase);
                    }
                    else
                    {
                        startingPhrase += String.Format("with {2} on and {0} outs in the bottom of the {1}.", score.NumberOfOuts, inningNum, score.NumberOnBase);
                    }
                    break;
                case GameState.RainDelay:
                    startingPhrase = String.Format("The {0} game is currently in a rain delay.", team.Name);
                    break;
                case GameState.Postponed:
                    startingPhrase = String.Format("The {0} game has been postponed.", team.Name);
                    break;
                case GameState.NoGame:
                    startingPhrase = String.Format("The {0} did not have a game that day.", team.Name);
                    break;
                case GameState.GameHasntStarted:
                    startingPhrase = String.Format("The {0} will be playing at {1}.", team.Name, score.StartingTime.ToShortTimeString());
                    break;
                case GameState.Completed:
                    if (score.WinningTeamScore.Team.Equals(team))
                    {
                        startingPhrase = String.Format("The {0} beat the {1} by a score of {2} to {3}.", team.Name, score.LosingTeamScore.Team.Name, score.WinningTeamScore.Score, score.LosingTeamScore.Score);
                    }
                    else
                    {
                        startingPhrase = String.Format("The {0} lost to the {1} by a score of {2} to {3}.", team.Name, score.WinningTeamScore.Team.Name, score.WinningTeamScore.Score, score.LosingTeamScore.Score);
                    }
                    break;
            }
            return startingPhrase;
        }
        #endregion
    }
}
