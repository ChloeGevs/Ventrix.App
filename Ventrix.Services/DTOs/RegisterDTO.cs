namespace Ventrix.Application.DTOs
{
    public class RegisterDto
    {
        public string UserId { get; set; } // The ID number (2024-001)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // You can pass "Student" here
    }
}