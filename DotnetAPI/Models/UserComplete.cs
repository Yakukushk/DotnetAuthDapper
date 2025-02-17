namespace DotnetAPI.Models
{
    public partial class UserComplete
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public bool Active { get; set; }
        public string JobTitle { get; set; } = default!;
        public string Department { get; set; } = default!;
        public decimal Salary {  get; set; }
        public decimal AvgSalary { get; set; }


        public UserComplete()
        {
            if (JobTitle is null)
                JobTitle = string.Empty;
            if (Department is null)
                Department = string.Empty;
            if (FirstName is null)
                FirstName = string.Empty;
            if (LastName is null)
                LastName = string.Empty;
            if (Gender is null)
                Gender = string.Empty;
        }
    }
}
