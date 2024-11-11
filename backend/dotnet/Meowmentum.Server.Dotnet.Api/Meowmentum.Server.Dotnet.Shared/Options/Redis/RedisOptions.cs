namespace Meowmentum.Server.Dotnet.Shared.Options.Redis;

public class RedisOptions
{
    public const string SectionName = "RedisConfiguration:RedisConnection";
    public required string Host { get; set; }
    public required int Port { get; set; }
    public string Password { get; set; }
    public bool Ssl { get; set; }
    public required int ConnectTimeout { get; set; }
    public required int ConnectRetry { get; set; }
}
