namespace Meowmentum.Server.Dotnet.Shared.Options.Redis;

public class BaseRedisDbConfig
{
    public int? DbNumber { get; set; }
    public required string Prefix { get; set; }
}

public class OtpDbConfig : BaseRedisDbConfig 
{
    public const string SectionName = "RedisConfiguration:OtpDbSettings";
    public required int ExpirationTimeInMinutes { get; set; }

}
public class TokenBlacklistDbConfig : BaseRedisDbConfig 
{ 
    public const string SectionName = "RedisConfiguration:TokenBlacklistDbSettings";
}
