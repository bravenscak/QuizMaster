namespace QuizMaster.DTOs
{
    public class QuizSearchDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? OrganizerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public QuizSortBy? SortBy { get; set; } = QuizSortBy.DateTime;
        public SortDirection? SortDirection { get; set; } = DTOs.SortDirection.Descending;
    }

    public enum QuizTimeFilter
    {
        All,
        Upcoming,
        Past
    }

    public enum QuizSortBy
    {
        DateTime,
        Name,
        RegisteredTeams,
        CategoryName
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
