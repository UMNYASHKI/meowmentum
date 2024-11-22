using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Core.Entities
{
    public class TimeInterval
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; }

        [Required]
        [ForeignKey("Task")] // Указывает, что TaskId — это внешний ключ
        public long TaskId { get; set; }

        public virtual Task Task { get; set; } // Связь с Task
    }
}
