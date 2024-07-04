namespace SecondRoundProject.DTOs
{
    public class ClientDTO
    {
        public required string PersonalId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePhoto { get; set; } = null;
        public required string MobileNumber { get; set; }
        public required string Sex { get; set; }
        public AddressDTO? Address { get; set; } = null;
        public required List<ClientAccountDTO> Accounts { get; set; } = new List<ClientAccountDTO>();

        public int? CreatedBy { get; set; } = null;
    }
}
