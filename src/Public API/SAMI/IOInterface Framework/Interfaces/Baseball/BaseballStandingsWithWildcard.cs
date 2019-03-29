using System;
using System.Collections.Generic;
using System.Linq;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// Represents the standings for a league or conference in baseball, when a wild card is important.
    /// </summary>
    public class BaseballStandingsWithWildcard : BaseballStandings
    {
        private List<BaseballTeamStanding> _wildcardStandings = new List<BaseballTeamStanding>();

        /// <summary>
        /// Adds the current standings for a given team.
        /// </summary>
        /// <param name="divisionStanding">The team's current standings in terms of their division.</param>
        /// <param name="wildcardStanding">The team's current standings in terms of their wildcard position.</param>
        public void AddStandingsForTeam(BaseballTeamStanding divisionStanding, BaseballTeamStanding wildcardStanding)
        {
            AddStandingsForTeam(divisionStanding);
            _wildcardStandings.Add(wildcardStanding);
        }

        /// <summary>
        /// Tries to get the divisional and wildcard standings for the requested team.
        /// </summary>
        /// <param name="team">The name of the team to query for.</param>
        /// <param name="divisionStandings">The divisional standings for the requested team.</param>
        /// <param name="wildcardStandings">The wildcard standings for the requested team.</param>
        /// <returns>True if standings for the requested team was able to be found.</returns>
        public bool TryGetStandingsForTeam(BaseballTeam team, out BaseballTeamStanding divisionStandings, out BaseballTeamStanding wildcardStandings)
        {
            wildcardStandings = null;
            Func<BaseballTeamStanding, bool> teamNameCheck = division => division.Team.Key.Equals(team.Key);
            if (_wildcardStandings.Any(teamNameCheck))
            {
                wildcardStandings = _wildcardStandings.Single(teamNameCheck);
            }

            return TryGetStandingsForTeam(team, out divisionStandings) && wildcardStandings != null;
        }
    }
}
