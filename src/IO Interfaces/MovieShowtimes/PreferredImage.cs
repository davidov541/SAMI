using System;

namespace SAMI.IOInterfaces.Movie
{
    internal class PreferredImage
    {
        public String uri { get; set; }
        public String height { get; set; }
        public String width { get; set; }
        public String primary { get; set; }
        public String category { get; set; }
        public Caption caption { get; set; }
    }
}
