namespace StudentManagementApi.Domain.Interfaces
{
    public interface IRabbitConfiguration
    {
        string Host { get; }
        string VirtualHost { get; }
        string User { get; }
        string Password { get; }
    }
}
