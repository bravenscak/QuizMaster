namespace QuizMaster.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;

        public int SubscriberId { get; set; } 
        public int OrganizerId { get; set; } 

        public User Subscriber { get; set; } = null!;
        public User Organizer { get; set; } = null!;
    }
}