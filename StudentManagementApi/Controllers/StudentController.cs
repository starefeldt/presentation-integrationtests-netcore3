using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [ProducesResponseType(500)]
    [Produces("application/json")]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(IStudentService));
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Student))]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentToRegister studentToRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = new Student
            {
                FirstName = studentToRegister.FirstName,
                LastName = studentToRegister.LastName,
                SocialSecurityNumber = studentToRegister.SocialSecurityNumber
            };

            await _studentService.Register(student);

            return CreatedAtRoute(
                routeName: nameof(GetStudent),
                routeValues: new { student.SocialSecurityNumber },
                value: student);
        }

        [HttpGet("{socialSecurityNumber}", Name = nameof(GetStudent))]
        [ProducesResponseType(204)]
        [ProducesResponseType(200, Type = typeof(Student))]
        public async Task<IActionResult> GetStudent(string socialSecurityNumber)
        {
            var student = await _studentService.GetBySocialSecurityNumber(socialSecurityNumber);

            if (student == null)
            {
                return NoContent();
            }

            return Ok(student);
        }
    }
}
