namespace DotnetAPI.Models;
public partial class Users
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public bool Active { get; set; } = default!;

    public Users()
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
