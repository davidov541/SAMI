using System;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Football;
using SAMI.IOInterfaces.Interfaces.Sports;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Football
{
    internal class FootballConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "Football";
            }
        }

        public FootballConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            Dialog phrase = CurrentDialog;
            String teamName = phrase.GetPropertyValue("Team");
            GameParameter parameter = (GameParameter)Enum.Parse(typeof(GameParameter), phrase.GetPropertyValue("Parameter"));

            IFootballSensor sensor = ConfigManager.FindAllComponentsOfType<IFootballSensor>().FirstOrDefault(s => s.Teams.Any(t => t.Key.Equals(teamName)));

            base.Speak();
            ConversationIsOver = true;

            if (sensor == null)
            {
                return String.Empty;
            }

            switch (parameter)
            {
                case GameParameter.Score:
                    return RespondToGameScore(sensor, sensor.Teams.Single(t => t.Key.Equals(teamName)));
                default:
                    return String.Empty;
            }
        }

        #region Response Methods
        private String RespondToGameScore(IFootballSensor sensor, FootballTeam team)
        {
            FootballGame score = sensor.LoadLatestScoreForTeam(team.Key);

            if (score == null)
            {
                return String.Format("The {0} are not playing this week.", team.Name);
            }

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
                        startingPhrase = String.Format("The {0} are beating the {1} by a score of {2} to {3}. ", team.Name, score.LosingTeamScore.Team.Name, score.WinningTeamScore.Score, score.LosingTeamScore.Score);
                    }
                    else
                    {
                        startingPhrase = String.Format("The {0} are losing to the {1} by a score of {2} to {3}. ", team.Name, score.WinningTeamScore.Team.Name, score.WinningTeamScore.Score, score.LosingTeamScore.Score);
                    }

                    String quarterNum = SayOrdinal(score.Quarter);
                    if (score.TimeLeft.Equals(TimeSpan.Zero))
                    {
                        startingPhrase += String.Format("at the end of the {0} quarter.", quarterNum);
                    }
                    else if (score.TimeLeft.Equals(new TimeSpan(0, 15, 0)))
                    {
                        startingPhrase += String.Format("at the beginning of the {0} quarter.", quarterNum);
                    }
                    else
                    {
                        startingPhrase += String.Format("with {0} {1} remaining in the {2} quarter.", score.TimeLeft.Minutes, score.TimeLeft.Seconds, quarterNum);
                    }
                    break;
                case GameState.GameHasntStarted:
                    if (score.HomeTeamScore.Team.Equals(team))
                    {
                        startingPhrase = String.Format("The {0} will be playing the {1} at {2} on {3}.", team.Name, score.AwayTeamScore.Team.Name, score.StartingTime.ToShortTimeString(), score.StartingTime.DayOfWeek.ToString());
                    }
                    else
                    {
                        startingPhrase = String.Format("The {0} will be playing the {1} at {2} on {3}.", team.Name, score.HomeTeamScore.Team.Name, score.StartingTime.ToShortTimeString(), score.StartingTime.DayOfWeek.ToString());
                    }
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
