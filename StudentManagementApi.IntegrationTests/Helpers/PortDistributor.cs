using System.Collections.Concurrent;

namespace StudentManagementApi.IntegrationTests.Helpers
{
    public sealed class PortDistributor
    {
        private static ConcurrentDictionary<string, int> _ports = new ConcurrentDictionary<string, int>();
        private static int _currentPort = 1433;
        private static readonly object _theLock = new object();


        public static int GetPortFor(string containerName)
        {
            lock (_theLock)
            {
                if (_ports.TryGetValue(containerName, out int port))
                {
                    return port;
                }
                else
                {
                    while (!_ports.TryAdd(containerName, _currentPort));
                    return _currentPort++;      //return value and then post-increment
                }
            }
        }
    }
}
