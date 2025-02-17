namespace DotnetAPI.Models
{
    public partial class UserJobInfo
    {
        public int UserId { get; set; }
        public string JobTitle { get; set; } = default!;
        public string Department { get; set; } = default!;

        public UserJobInfo()
        {
            if(JobTitle == null)
            {
                JobTitle = string.Empty;    
            }
            if(Department == null)
            {
                Department = string.Empty;
            }
        }
    }
}
