using FreeSql.DataAnnotations;

namespace IGeekFan.FreeKit.xUnit.Models
{
    public class UserRole
    {
        [Column(IsPrimary = true)]
        public int UserId { get; set; }
        [Column(IsPrimary = true)]
        public int RoleId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
