namespace DotnetAPI.Models.DTOs
{
    public partial class LoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public LoginConfirmationDto()
        {
            if (PasswordHash == null)
                PasswordHash = new byte[0];
            if (PasswordSalt == null)
                PasswordSalt = new byte[0];
        }
    }
}
