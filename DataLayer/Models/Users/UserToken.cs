
using CommonLayer;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using static CommonLayer.EnumExtensions;

namespace Datalayer.Models.Users
{
    [Table(nameof(UserToken), Schema = DbSchemas.Users)]
    public class UserToken : BaseEntity
    {
        public string AccessTokenHash { get; set; }
        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }
        public string RefreshTokenIdHash { get; set; }
        public string RefreshTokenIdHashSource { get; set; }
        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }
        public long UserId { get; set; }
        public long PersonId { get; set; } = 0;
        public byte Type { get; set; } = UserTypeEnum.Student.ToByte();
        public virtual User User { get; set; }
        public virtual Person Person { get; set; }
    }
}