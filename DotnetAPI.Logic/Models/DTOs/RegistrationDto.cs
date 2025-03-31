using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Models.DTOs
{
    public partial class RegistrationDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [PasswordPropertyText]
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }   
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public decimal AvgSalary { get; set; }
        public bool Active { get; set; }
        public int UserId { get; set; }

        public RegistrationDto()
        {
            if (Email == null)
            {
                Email = "";
            }
            if (Password == null)
            {
                Password = "";
            }
            if (PasswordConfirm == null)
            {
                PasswordConfirm = "";
            }
            if (FirstName == null)
            {
                FirstName = "";
            }
            if (LastName == null)
            {
                LastName = "";
            }
            if (Gender == null)
            {
                Gender = "";
            }
        }
    }
}
