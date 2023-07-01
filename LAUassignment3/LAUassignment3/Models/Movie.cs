namespace LAUassignment3.Models
{
    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public int rating { get; set; }
        public string genre { get; set; }

        public string director { get; set; }

        public int year { get; set; }

        public string fileName { get; set; }

        public string uploadUser { get; set; }

        public Movie(int id, string title, int rating, string genre, string director, int year, string fileName, string uploadUser)
        {
            this.id = (int)id;
            this.title = title;
            this.rating = (int)rating;
            this.genre = genre;
            this.director = director;
            this.year = (int)year;
            this.fileName = fileName;
            this.uploadUser = uploadUser;
        }

        public Movie()
        {

        }
    }
}
