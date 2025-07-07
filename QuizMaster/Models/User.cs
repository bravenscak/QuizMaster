using System.ComponentModel.DataAnnotations;

namespace QuizMaster.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        public string? OrganizationName { get; set; }
        public string? Description { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}