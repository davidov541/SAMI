using System;
using System.Collections.Generic;
using System.Linq;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// Representation of the standings at a given time for a league or conference in baseball.
    /// </summary>
    public class BaseballStandings
    {
        private List<BaseballTeamStanding> _divisionalStandings = new List<BaseballTeamStanding>();

        /// <summary>
        /// Adds the standings for a single team to our list.
        /// </summary>
        /// <param name="divisionStanding">Standings for a single team.</param>
        public void AddStandingsForTeam(BaseballTeamStanding divisionStanding)
        {
            _divisionalStandings.Add(divisionStanding);
        }

        /// <summary>
        /// Tries to get the current standings for a single team.
        /// </summary>
        /// <param name="team">Team to query for in the standings.</param>
        /// <param name="divisionStandings">The division standings, if any were found. If not, this may be anything, but is most likely null.</param>
        /// <returns>True if that team was found, false otherwise.</returns>
        public bool TryGetStandingsForTeam(BaseballTeam team, out BaseballTeamStanding divisionStandings)
        {
            divisionStandings = null;
            Func<BaseballTeamStanding, bool> teamNameCheck = division => division.Team.Key.Equals(team.Key);
            if (_divisionalStandings.Any(teamNameCheck))
            {
                divisionStandings = _divisionalStandings.Single(teamNameCheck);
            }

            return divisionStandings != null;
        }
    }
}
