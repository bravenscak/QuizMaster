using System.ComponentModel.DataAnnotations;

namespace QuizMaster.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ParticipantCount { get; set; }

        public int? FinalPosition { get; set; } 

        public int QuizId { get; set; }
        public int UserId { get; set; } 

        public Quiz Quiz { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}