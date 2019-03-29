using System;

namespace SAMI.IOInterfaces.Movie
{
    internal class Theater
    {
        public String id { get; set; }
        public String name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    
}
