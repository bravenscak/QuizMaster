using System.ComponentModel.DataAnnotations;

namespace QuizMaster.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RelatedEntityId { get; set; }
    }

    public class CreateNotificationDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        public int? RelatedEntityId { get; set; }
    }

    public class MarkAsReadDto
    {
        [Required]
        public bool IsRead { get; set; } = true;
    }

    public static class NotificationTypes
    {
        public const string NewQuiz = "NEW_QUIZ";
        public const string QuizStarting = "QUIZ_STARTING";
        public const string QuizResults = "QUIZ_RESULTS";
        public const string TeamRegistered = "TEAM_REGISTERED";
    }
}
