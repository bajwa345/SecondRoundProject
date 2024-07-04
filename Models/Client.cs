namespace SecondRoundProject.Models
{
    public class Client
    {
        public int Id { get; set; }
        public required string PersonalId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePhoto { get; set; } = null;
        public required string MobileNumber { get; set; }
        public required string Sex { get; set; }
        public string? Country { get; set; } = null;
        public string? City { get; set; } = null;
        public string? Street { get; set; } = null;
        public string? ZipCode { get; set; } = null;
        public required List<ClientAccount> Accounts { get; set; }
        public required int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = null;
    }

    public class ClientAccount
    {
        public int Id { get; set; }
        public required string AccountNumber { get; set; }
    }
}
