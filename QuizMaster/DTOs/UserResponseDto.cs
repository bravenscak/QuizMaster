namespace QuizMaster.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? OrganizationName { get; set; }
        public string? Description { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
