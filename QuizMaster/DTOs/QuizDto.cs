using System.ComponentModel.DataAnnotations;

namespace QuizMaster.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public decimal? EntryFee { get; set; }
        public DateTime DateTime { get; set; }
        public int MaxParticipantsPerTeam { get; set; }
        public int MaxTeams { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Description { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public int OrganizerId { get; set; }  
        public string CategoryName { get; set; } = string.Empty;
        public int RegisteredTeamsCount { get; set; }
    }

    public class CreateQuizDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Range(0, 10000)]
        public decimal? EntryFee { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [Range(1, 20)]
        public int MaxParticipantsPerTeam { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxTeams { get; set; }

        [Range(30, 480)]
        public int? DurationMinutes { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

    public class UpdateQuizDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Range(0, 10000)]
        public decimal? EntryFee { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [Range(1, 20)]
        public int MaxParticipantsPerTeam { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxTeams { get; set; }

        [Range(30, 480)]
        public int? DurationMinutes { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
