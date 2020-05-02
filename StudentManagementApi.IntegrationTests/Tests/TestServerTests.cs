using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Xunit;

namespace StudentManagementApi.IntegrationTests.Tests
{
    public class TestServerTests
    {
        [Fact]
        public async Task OldWay_WithWebHostBuilder_ShouldReturnHelloWorld()
        {
            //Arrange
            var webHostBuilder = new WebHostBuilder()
                .Configure(app => app.Run(async ctx =>
                    await ctx.Response.WriteAsync("Hello World!")));

            var server = new TestServer(webHostBuilder);
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("/");

            //Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World!", responseString);
        }

        [Fact]
        public async Task NewWay_WithHostBuilder_ShouldReturnHelloWorld()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    //Compare with previous that uses new TestServer(...)
                    webHost.UseTestServer();
                    webHost.Configure(app => app.Run(async ctx =>
                        await ctx.Response.WriteAsync("Hello World!")));
                });

            //Compare with previous that uses CreateClient()
            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World!", responseString);
        }
    }
}
