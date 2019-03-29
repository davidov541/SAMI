using System;
using SAMI.IOInterfaces.Interfaces.Baseball;

namespace SAMI.IOInterfaces.Baseball
{
    internal class MLBTeam : BaseballTeam
    {
        public MLBDivision Division
        {
            get;
            private set;
        }

        public MLBTeam(String city, String name, String key, MLBDivision division)
            : base(city, name, key, ConvertDivisionsToName(division))
        {
            Division = division;
        }

        private static String ConvertDivisionsToName(MLBDivision division)
        {
            switch (division)
            {
                case MLBDivision.NLEast:
                    return "N L East";
                case MLBDivision.NLCentral:
                    return "N L Central";
                case MLBDivision.NLWest:
                    return "N L West";
                case MLBDivision.ALEast:
                    return "A L East";
                case MLBDivision.ALCentral:
                    return "A L Central";
                case MLBDivision.ALWest:
                    return "A L West";
                default:
                    break;
            }
            return String.Empty;
        }

        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
            {
                return false;
            }

            MLBTeam other = obj as MLBTeam;
            if(other == null)
            {
                return false;
            }

            return other.Division == Division;
        }
    }
}
