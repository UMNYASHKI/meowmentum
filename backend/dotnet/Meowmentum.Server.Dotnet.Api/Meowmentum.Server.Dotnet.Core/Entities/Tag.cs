using Meowmentum.Server.Dotnet.Core.Interfaces;

namespace Meowmentum.Server.Dotnet.Core.Entities;

public class Tag : ITrackingEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set;}
    public long UserId { get; set; }
    public virtual AppUser User { get; set; }
}
