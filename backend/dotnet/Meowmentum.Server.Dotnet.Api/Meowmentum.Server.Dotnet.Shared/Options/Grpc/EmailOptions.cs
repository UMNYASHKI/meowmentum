namespace Meowmentum.Server.Dotnet.Shared.Options.Grpc;

public class EmailOptions
{
    public const string SectionName = "Grpc:EmailOptions";
    public string Host { get; set; }
    public string Port { get; set; }
    public string Address => $"{Host}:{Port}";
}
