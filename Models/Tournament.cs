namespace Tapi.Models
{
    public class Tournament
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "Stworzony"; 

        public int BracketId { get; set; }
        public Bracket Bracket { get; set; } = null!;
    }
}

