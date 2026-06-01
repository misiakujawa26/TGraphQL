namespace Tapi.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int Round { get; set; }

        public int Player1Id { get; set; }
        public User Player1 { get; set; } = null!;

        public int Player2Id { get; set; }
        public User Player2 { get; set; } = null!;

        public int? WinnerId { get; set; }
        public User? Winner { get; set; }
    }
}

