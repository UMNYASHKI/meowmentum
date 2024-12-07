using Meowmentum.Server.Dotnet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Task
{
    public class TaskFilterRequest
    {
        public long? TaskId { get; set; }
        public List<TaskStatus?> Status { get; set; } = new List<TaskStatus?>();
        //public List<long?> TagIds { get; set; } = new List<long?>();
        public List<TaskPriority?> Priorities { get; set; } = new List<TaskPriority?>();
    }
}
