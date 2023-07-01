namespace LAUassignment3.Models
{
    public class Comment
    {
        public int id { get; set; }
        public string movieTitle { get; set; }

        public string user { get; set; }

        public int rating { get; set; }

        public DateTime? lastEditDateTime { get; set; }

        public string comment { get; set; }

        public Comment() { }
    }
}
