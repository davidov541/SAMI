using System.Collections.Generic;
using SAMI.IOInterfaces.Interfaces.Baseball;

namespace SAMI.IOInterfaces.Baseball
{
    internal class MLBStandings : BaseballStandingsWithWildcard
    {
        private List<BaseballTeamStanding> _nlEastStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _nlCentralStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _nlWestStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _nlWildcardStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _alEastStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _alCentralStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _alWestStandings = new List<BaseballTeamStanding>();
        private List<BaseballTeamStanding> _alWildcardStandings = new List<BaseballTeamStanding>();

        /// <summary>
        /// Adds the given divisional and wildcard standings for a team.
        /// </summary>
        /// <param name="divisionStandings">The divisional standings for the team.</param>
        /// <param name="wildcardStandings">The wildcard standings for the team.</param>
        /// <param name="division">The division that the standings should be added.
        /// </param>
        public void AddStandingsForTeam(BaseballTeamStanding divisionStandings, BaseballTeamStanding wildcardStandings, MLBDivision division)
        {
            AddStandingsForTeam(divisionStandings, wildcardStandings);
            switch (division)
            {
                case MLBDivision.NLEast:
                    _nlEastStandings.Add(divisionStandings);
                    _nlWildcardStandings.Add(wildcardStandings);
                    break;
                case MLBDivision.NLCentral:
                    _nlCentralStandings.Add(divisionStandings);
                    _nlWildcardStandings.Add(wildcardStandings);
                    break;
                case MLBDivision.NLWest:
                    _nlWestStandings.Add(divisionStandings);
                    _nlWildcardStandings.Add(wildcardStandings);
                    break;
                case MLBDivision.ALEast:
                    _alEastStandings.Add(divisionStandings);
                    _alWildcardStandings.Add(wildcardStandings);
                    break;
                case MLBDivision.ALCentral:
                    _alCentralStandings.Add(divisionStandings);
                    _alWildcardStandings.Add(wildcardStandings);
                    break;
                case MLBDivision.ALWest:
                    _alWestStandings.Add(divisionStandings);
                    _alWildcardStandings.Add(wildcardStandings);
                    break;
                default:
                    break;
            }
        }
    }
}
