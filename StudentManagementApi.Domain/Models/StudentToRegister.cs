using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain.Models
{
    public class StudentToRegister
    {
        [Required()]
        public string FirstName { get; set; }
        
        [Required()]
        public string LastName { get; set; }
        
        [Required()]
        [MaxLength(13)]
        public string SocialSecurityNumber { get; set; }
    }
}
