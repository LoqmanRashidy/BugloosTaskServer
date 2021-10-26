using CommonLayer;
using Datalayer.Models;
using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datalayer.Models.Users
{
    [Table(nameof(Role), Schema = DbSchemas.Users)]
    [Description("نقش‌ها")]
    public class Role: BaseEntity
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
        }
        
        public string EnTitle { get; set; }
        public string FaTitle { get; set; }

        public virtual List<UserRole> UserRoles { get; set; }

    }

    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator()
        {

        }
    }
}