using StudentManagementApi.Domain.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;

namespace StudentManagementApi.IntegrationTests.Helpers
{
    public class DockerConfiguration
    {
        public string ContainerName { get; set; }
        public int Port { get; set; }
    }

    //Needed Microsoft.Powershell.SDK 6.2.4 -> 2020-02-16
    public class DockerSqlHelper
    {
        private const string Password = "SA_PASSWORD=";
        private const string Image = "microsoft/mssql-server-windows-express";
        
        public IDbConnectionFactory DbConnectionFactory { get; }
        public DockerConfiguration Configuration { get; }

        public DockerSqlHelper(DockerConfiguration configuration, IDbConnectionFactory dbConnectionFactory)
        {
            Configuration = configuration;
            DbConnectionFactory = dbConnectionFactory;
        }

        public void StartContainer()
        {
            var script = new StringBuilder()
                .Append("docker run ")
                .Append("-d ")
                .Append("-e \"ACCEPT_EULA=Y\" ")
                .Append($"-e \"{Password}\" ")
                .Append($"-p {Configuration.Port}:1433 ")
                .Append($"--name {Configuration.ContainerName} ")
                .Append(Image)
                .ToString();

            using (var ps = PowerShell.Create())
            {
                WriteDebugMessage($"Starting Container create...");

                ps.AddScript(script);
                Invoke(ps, "Container was not created, check script used");
                WriteDebugMessage($"Container created");

                ps.AddScript($"docker container ls -f Name={Configuration.ContainerName} -f Status=running ");
                Invoke(ps, "Container is not running");
                WriteDebugMessage($"Container running");

                TestDbConnection();
                WriteDebugMessage($"DbConnection established");
            }
        }

        private void TestDbConnection()
        {
            WriteDebugMessage("Testing dbConnection");
            int retryCount = 0;
            int maxAttempts = 20;

            while (retryCount != maxAttempts)
            {
                if (DbConnectionFactory.CanOpenConnection())
                {
                    WriteDebugMessage($"Could open after retry: {retryCount}");
                    return;
                }

                Thread.Sleep(1_000 * retryCount);
                retryCount++;
                WriteDebugMessage($"Retry: {retryCount}/{maxAttempts}");
            }

            //let it throw so we can know what's wrong
            DbConnectionFactory.OpenConnection();
        }

        public void RemoveContainer()
        {
            using (var ps = PowerShell.Create())
            {
                WriteDebugMessage($"Starting Container remove...");
                
                ps.AddScript($"docker container stop {Configuration.ContainerName}");
                Invoke(ps, "Container was not stopped, check script used");
                WriteDebugMessage($"Container stopped");

                ps.AddScript($"docker container rm {Configuration.ContainerName}");
                Invoke(ps, "Container was not removed, check script used");
                WriteDebugMessage($"Container removed");
            }
        }

        private Collection<PSObject> Invoke(PowerShell ps, string errorMessage)
        {
            var result = ps.Invoke();

            if (!result.Any())
            {
                throw new InvalidPowerShellStateException(errorMessage);
            }
            return result;
        }
        
        public void WriteDebugMessage(string message)
        {
            Debug.WriteLine($"[{DateTime.Now}] {Configuration.ContainerName}:{Configuration.Port} - {message}");
        }
    }
}
