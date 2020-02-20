using Microsoft.Extensions.DependencyInjection;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.IntegrationTests.Repositories;
using System.Net.Http;
using System.Text;

namespace StudentManagementApi.IntegrationTests.Helpers
{
    public static class TestUtility
    {
        public static IDbTestRepository GetCleanTestRepository(IServiceScope scope)
        {
            var testRepo = scope.ServiceProvider.GetRequiredService<IDbTestRepository>();
            testRepo.DropTables();
            testRepo.CreateTables();
            return testRepo;
        }

        public static HttpRequestMessage GetRequestMessage(
            string json, string uri, HttpMethod httpMethod)
        {
            return new HttpRequestMessage(httpMethod, uri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        // Careful: this is a Singleton! Tests may be affecting each other if state is affected in FakeMessageSender.
        // Name of tests affects order of test-execution which may cause problems.
        // Make sure state is set to defaults after each test has run!
        // Better is to set up ConfigureTestServices in each test or constructor as part of Arrange 
        public static FakeMessageSender GetFakeMessageSender(IServiceScope scope) =>
            (FakeMessageSender)scope.ServiceProvider.GetRequiredService<IMessageSender>();

    }
}
