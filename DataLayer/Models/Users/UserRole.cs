
using CommonLayer;
using Datalayer.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datalayer.Models.Users
{
    [Table(nameof(UserRole), Schema = DbSchemas.Users)]
    [Description("نقش کاربران")]
    public class UserRole: BaseEntity
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}