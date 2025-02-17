namespace DotnetAPI.Models.DTOs
{
    public partial class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginDto()
        {
            if (Email == null)
                Email = string.Empty;
            if (Password == null)
                Password = string.Empty;
        }
    }
}
