namespace CollectorApp.Data
{
    public class Login
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public DateTime? LastIncorrectLogin { get; set; }
        public string? SessionToken { get;set; }
    }
}
