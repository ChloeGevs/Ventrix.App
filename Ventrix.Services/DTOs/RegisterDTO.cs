namespace Ventrix.Application.DTOs
{
    public class RegisterDTO 
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }

        public Ventrix.Domain.Enums.UserRole Role { get; set; }
    }
}