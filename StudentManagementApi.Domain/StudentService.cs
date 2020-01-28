using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StudentManagementApi.Domain
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;
        private readonly IMessageSender _messageSender;

        public StudentService(IStudentRepository repository, IMessageSender messageSender)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IStudentRepository));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(IMessageSender));
        }

        public async Task<Student> GetBySocialSecurityNumber(string socialSecurityNumber)
        {
            return await _repository.GetBySocialSecurityNumberAsync(socialSecurityNumber);
        }

        public async Task Register(Student student)
        {
            student.Created = DateTimeOffset.Now;
            await _repository.RegisterAsync(student);

            var message = new Message
            {
                MessageType = MessageType.Registered,
                Content = student.SocialSecurityNumber
            };

            if (!_messageSender.Send(message))
            {
                throw new InvalidOperationException($"Failed to send");
            }
            await _repository.SaveChangesAsync();
        }
    }
}
