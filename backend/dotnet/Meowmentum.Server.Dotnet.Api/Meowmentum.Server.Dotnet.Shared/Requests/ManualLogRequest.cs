using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Requests;

public class ManualLogRequest
{
    public long TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public int SpendedTime { get; set; } // В минутах
    public string? Description { get; set; }
}
