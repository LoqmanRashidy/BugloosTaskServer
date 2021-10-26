using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLayer;
using FluentValidation;

namespace Datalayer.Models.Users
{
    [Table(nameof(User), Schema = DbSchemas.Users)]
    [Description("کاربران")]
    public class User : BaseEntity
    {
        public User()
        {
            UserRoles = new List<UserRole>();      
            UserTokens = new List<UserToken>();
            ModelRoles = new List<Role>();
        }
        [Description("شناسه شخص")]
        public long PersonId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SerialNumber { get; set; }
        [Description("نام")]
        public string Name { get; set; }
        [Description("نام خانوادگی")]
        public string LastName { get; set; }
        [Description("ایمیل")]
        public string Email { get; set; }

        [Description("تلفن همراه")]
        public string Mobile { get; set; }

        [Description("سیستمی")]
        public bool IsSystem { get; set; } = false;

        [Description("آخرین زمان ورود به سیستم")]
        public DateTime? LastLoggedIn { get; set; }
        private string _PersianLastLoggedIn { get; set; }
        [NotMapped]
        public string PersianLastLoggedIn { get; set; }

        [NotMapped]
        public List<Role> ModelRoles { get; set; }

        public virtual Person Person { get; set; }        
        public virtual List<UserRole> UserRoles { get; set; }
        public virtual List<UserToken> UserTokens { get; set; }
        
    }

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
        }
    }
}
