using System;

namespace StudentManagementApi.Domain.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
