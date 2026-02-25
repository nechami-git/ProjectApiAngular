namespace server.Models.DTO
{
    public class AuthUserDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public Role? Role { get; set; }
    }
}
