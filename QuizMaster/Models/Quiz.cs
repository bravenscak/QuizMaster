using System.ComponentModel.DataAnnotations;

namespace QuizMaster.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string LocationName { get; set; } = string.Empty; 

        [Required]
        public string Address { get; set; } = string.Empty; 

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public decimal? EntryFee { get; set; } 

        [Required]
        public DateTime DateTime { get; set; }

        public int MaxParticipantsPerTeam { get; set; }
        public int MaxTeams { get; set; }

        public int? DurationMinutes { get; set; }

        public string? Description { get; set; }

        public int UserId { get; set; } 
        public int CategoryId { get; set; }

        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}