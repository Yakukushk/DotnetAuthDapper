namespace DotnetAPI.Models.DTOs
{
    public partial class UserDTO
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public bool Active { get; set; } = default!;

        public UserDTO()
        {
            if (FirstName == null)
                FirstName = string.Empty;

            if (LastName == null)
                LastName = string.Empty;

            if (Email == null)
                Email = string.Empty;

            if (Gender == null)
                Gender = string.Empty;
        }
    }
}
