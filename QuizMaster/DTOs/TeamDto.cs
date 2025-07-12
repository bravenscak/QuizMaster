using System.ComponentModel.DataAnnotations;

namespace QuizMaster.DTOs
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ParticipantCount { get; set; }
        public int? FinalPosition { get; set; }
        public string CaptainName { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public DateTime QuizDateTime { get; set; }
        public int MaxParticipantsPerTeam { get; set; }
        public int QuizId { get; set; }
    }

    public class CreateTeamDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 20)]
        public int ParticipantCount { get; set; }

        [Required]
        public int QuizId { get; set; }
    }

    public class UpdateTeamDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 20)]
        public int ParticipantCount { get; set; }
    }

    public class TeamResultDto
    {
        [Required]
        [Range(1, 100)]
        public int FinalPosition { get; set; }
    }

    public class QuizTeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ParticipantCount { get; set; }
        public int? FinalPosition { get; set; }
        public string CaptainName { get; set; } = string.Empty;
        public string CaptainEmail { get; set; } = string.Empty;
    }
}
