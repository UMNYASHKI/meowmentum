namespace Meowmentum.Server.Dotnet.Shared.Options.Grpc;

public class EmailOptions
{
    public const string SectionName = "Grpc:EmailOptions";
    public required string Address { get; set; }
}
