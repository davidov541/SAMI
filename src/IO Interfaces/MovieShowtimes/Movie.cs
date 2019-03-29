using System;

namespace SAMI.IOInterfaces.Movie
{
    internal class Movie
    {
        public String title { get; set; }
        public String description { get; set; }
        public String rating { get; set; }

        public Movie(String title, String description, String rating)
        {
            this.title = title;
            this.description = description;
            this.rating = rating;
        }


        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            Movie m = obj as Movie;
            if ((object)m == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.title.Equals(m.title);
        }

        public bool Equals(Movie m)
        {
            // Return true if the title fields matches:
            return this.title.Equals(m.title);
        }
    }
}