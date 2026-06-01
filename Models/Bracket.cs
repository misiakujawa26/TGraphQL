namespace Tapi.Models
{
    public class Bracket
    {
        public int Id { get; set; }
        public List<Match> Matches { get; set; } = new();
    }
}

