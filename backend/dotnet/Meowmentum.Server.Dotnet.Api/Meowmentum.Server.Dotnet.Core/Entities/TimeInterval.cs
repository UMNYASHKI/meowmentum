using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meowmentum.Server.Dotnet.Core.Entities
{
    public class TimeInterval
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Description { get; set; }

        [Required]
        [ForeignKey("Task")]
        public long TaskId { get; set; }

        [JsonIgnore]
        public virtual Task Task { get; set; }
    }
}
