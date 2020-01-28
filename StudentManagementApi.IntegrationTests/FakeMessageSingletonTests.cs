//using Microsoft.Extensions.DependencyInjection;
//using StudentManagementApi.IntegrationTests.Helpers;
//using System;
//using Xunit;

//namespace StudentManagementApi.IntegrationTests
//{
//    public class FakeMessageSingletonTests : 
//        IClassFixture<CustomWebApplicationFactory<Startup>>, 
//        IDisposable
//    {
//        private readonly CustomWebApplicationFactory<Startup> _factory;

//        //Run between each test
//        public FakeMessageSingletonTests(CustomWebApplicationFactory<Startup> factory)
//        {
//            _factory = factory;
//        }

//        //Run between each test
//        public void Dispose()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var sender = TestUtility.GetFakeMessageSender(scope);
//            sender.ShouldSend = true;
//        }

//        [Fact]
//        public void FakeMessageSenderTest_WhenRun1()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var sender = TestUtility.GetFakeMessageSender(scope);
//            sender.ShouldSend = false;
//            Assert.False(sender.ShouldSend);
//        }

//        //Test above may carry along ShouldSend=false if run before this test.
//        [Fact]
//        public void FakeMessageSenderTest_WhenRun2()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var sender = TestUtility.GetFakeMessageSender(scope);
//            Assert.True(sender.ShouldSend);
//        }
//    }
//}
