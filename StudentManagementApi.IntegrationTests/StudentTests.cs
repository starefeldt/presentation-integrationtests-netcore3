﻿using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using StudentManagementApi.IntegrationTests.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace StudentManagementApi.IntegrationTests
{
    public class StudentTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        //Run between each test
        public StudentTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var fakeSender = new FakeMessageSender(shouldSend: true);
                    services.AddScoped<IMessageSender>(sp => fakeSender);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task RegisterStudent_WhenMissingSocialSecurityNumber_Returns400()
        {
            //Arrange
            var json = @"
                    {
                        ""FirstName"":""Peter"",
                        ""LastName"":""Testsson"",
                        ""SocialSecurityNumber"":""""
                    }";

            var request = TestUtility.GetRequestMessage(json, "/api/student", HttpMethod.Post);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("SocialSecurityNumber", responseString);
        }

        [Fact]
        public async Task RegisterStudent_WhenValidData_Returns201()
        {
            //Arrange
            var json = JsonConvert.SerializeObject(new StudentToRegister
            {
                FirstName = "Peter",
                LastName = "Testsson",
                SocialSecurityNumber = "19010101-1234"
            });
            var request = TestUtility.GetRequestMessage(json, "/api/student", HttpMethod.Post);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task RegisterStudent_WhenValidData_SavesToDb()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var context = TestUtility.GetCleanDbContext(scope);

            var studentToRegister = new StudentToRegister
            {
                FirstName = "Peter",
                LastName = "Testsson",
                SocialSecurityNumber = "19010101-1234"
            };
            var json = JsonConvert.SerializeObject(studentToRegister);
            var request = TestUtility.GetRequestMessage(json, "/api/student", HttpMethod.Post);

            //Act
            await _client.SendAsync(request);

            //Assert
            var student = context.Students.Single();
            Assert.Equal(1, student.Id);
            Assert.Equal(studentToRegister.FirstName, student.FirstName);
            Assert.Equal(studentToRegister.LastName, student.LastName);
            Assert.Equal(studentToRegister.SocialSecurityNumber, student.SocialSecurityNumber);
        }

        [Fact]
        public async Task RegisterStudent_WhenFailedSend_DoesNotSaveToDb()
        {
            //Arrange
            var localClient = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var fakeSender = new FakeMessageSender(shouldSend: false);
                    services.AddScoped<IMessageSender>(sp => fakeSender);
                });
            }).CreateClient();

            using var scope = _factory.Services.CreateScope();
            var context = TestUtility.GetCleanDbContext(scope);

            var studentToRegister = new StudentToRegister
            {
                FirstName = "Peter",
                LastName = "Testsson",
                SocialSecurityNumber = "19010101-1234"
            };
            var json = JsonConvert.SerializeObject(studentToRegister);
            var request = TestUtility.GetRequestMessage(json, "/api/student", HttpMethod.Post);

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => localClient.SendAsync(request));
            Assert.Empty(context.Students);
        }
    }
}
