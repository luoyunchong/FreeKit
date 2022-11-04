using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace IGeekFan.FreeKit.xUnit.Models
{
    [Table(Name = "todo")]
    public class Todo : FullAuditEntity
    {
        public string Message { get; set; }
        public DateTime? NotifictionTime { get; set; }

        public bool IsDone { get; set; }
    }
}