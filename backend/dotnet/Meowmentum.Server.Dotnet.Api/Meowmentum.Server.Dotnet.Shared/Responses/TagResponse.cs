namespace Meowmentum.Server.Dotnet.Shared.Responses;

public class TagResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
