/*using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    [TestFixture]
    public class RedistClientTest
    {
        INetworkClient _networkClient;
        RedisServerService _redisClientService;

        [SetUp]
        public void Setup()
        {
            _networkClient = Substitute.For<INetworkClient>();
            _redisClientService = new RedisServerService(_networkClient);
        }

        void Connect()
        {
            _redisClientService.Connect("127.0.0.1", 1234);
            _networkClient.IsConnected.Returns(true);
        }

        [Test]
        public void TestConnection()
        {
            Connect();
            Assert.IsTrue(_redisClientService.IsConnected, "Failed to connect to server!");
        }

        [Test]
        public void TestSet()
        {
            var response = _redisClientService.Set("TestKey", "TestValue").Result;
            Assert.AreNotEqual(response, "OK", "Server should not be connected");

            Connect();

            _networkClient.Receive().Returns("OK");

            response = _redisClientService.Set("TestKey", "TestValue").Result;
            Assert.IsTrue((response == "OK"), "Set flow is broken");
        }

        [Test]
        public void TestSetEx()
        {
            var response = _redisClientService.SetEx("TestKey", "TestValue", 10).Result;
            Assert.AreNotEqual(response, "OK", "Server should not be connected");

            Connect();

            _networkClient.Receive().Returns("OK");

            response = _redisClientService.SetEx("TestKey", "TestValue", 10).Result;
            Assert.IsTrue((response == "OK"), "Set flow is broken");
        }

        [Test]
        public void TestGet()
        {
            var response = _redisClientService.Get("TestKey").Result;
            Assert.AreNotEqual(response, "OK", "Server should not be connected");

            Connect();

            _networkClient.Receive().Returns("TestValue");

            response = _redisClientService.Get("TestKey").Result;
            Assert.IsTrue((response == "TestValue"), "Set flow is broken");
        }

        [Test]
        public void TestDel()
        {
            var response = _redisClientService.Del("TestKey").Result;
            Assert.AreNotEqual(response, "OK", "Server should not be connected");

            Connect();

            _networkClient.Receive().Returns("OK");

            response = _redisClientService.Del("TestKey").Result;
            Assert.IsTrue((response == "OK"), "Set flow is broken");
        }

        [Test]
        public void TestDBSize()
        {
            var response = _redisClientService.DBSize().Result;
            Assert.AreNotEqual(response, "OK", "Server should not be connected");

            Connect();

            _networkClient.Receive().Returns("0");

            response = _redisClientService.DBSize().Result;
            Assert.IsTrue((response == "0"), "Set flow is broken");
        }

        [Test]
        public void TestIncr()
        {
            var response = _redisClientService.Incr("TestKey").Result;
            Assert.AreNotEqual(response, "OK", "Server should not be connected");

            Connect();

            _networkClient.Receive().Returns("OK");

            response = _redisClientService.Incr("TestKey").Result;
            Assert.IsTrue((response == "OK"), "Set flow is broken");
        }

        [TearDown]
        public void Cleanup()
        {
            _networkClient = null;
            _redisClientService = null;
        }
    }
}
*/