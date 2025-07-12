using System.ComponentModel.DataAnnotations;

namespace QuizMaster.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string? OrganizationName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
