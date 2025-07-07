﻿using System.ComponentModel.DataAnnotations;

namespace QuizMaster.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty; 

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? RelatedEntityId { get; set; } 

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}