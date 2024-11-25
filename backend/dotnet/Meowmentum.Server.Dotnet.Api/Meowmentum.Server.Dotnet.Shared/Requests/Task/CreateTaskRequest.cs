using Meowmentum.Server.Dotnet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Task
{
    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }

        public TaskPriority? Priority { get; set; }
        public TaskStatus? Status { get; set; }

        public long? TagId { get; set; }
    }
}
