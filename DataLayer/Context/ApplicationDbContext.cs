
using Datalayer.Models;
using Datalayer.Models.Users;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Text;
using static CommonLayer.EnumExtensions;

namespace DataLayer.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public ApplicationDbContext() : base()
        {

        }

        #region Users

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserToken> UserToken { get; set; }
        #endregion Users

        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Person> Person { get; set; }

        public virtual DbSet<Course> Course { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region IgnoreIsDelete

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // 2. Create the query filter
                var parameter = Expression.Parameter(entityType.ClrType);

                var propertyMethodInfo = typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(bool));

                var IsDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDelete"));

                BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, IsDeletedProperty, Expression.Constant(false));

                // post => EF.Property<bool>(post, "IsDeleted") == false
                var lambda = Expression.Lambda(compareExpression, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
            #endregion IgnoreIsDelete


            #region Users                                       

            builder.Entity<User>(entity =>
            {
                entity.HasIndex(x => x.Id).IsUnique();
                entity.HasIndex(x => x.PersonId);


                entity.Property(x => x.Username).HasMaxLength(450).IsRequired();
                entity.Property(x => x.Password).IsRequired();
                entity.Property(x => x.SerialNumber).HasMaxLength(450);
                entity.Property(x => x.Name).HasMaxLength(20);
                entity.Property(x => x.LastName).HasMaxLength(50);
                entity.Property(x => x.Mobile);

                entity.Property(x => x.IsActive);
                entity.Property(x => x.IsSystem).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.LastLoggedIn);
                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.PersonId);

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);
            });


            builder.Entity<Role>(entity =>
            {
                entity.HasIndex(x => x.Id);
                entity.HasIndex(x => x.EnTitle).IsUnique();

                entity.Property(x => x.EnTitle).HasMaxLength(450).IsRequired();
                entity.Property(x => x.FaTitle).IsRequired();

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(x => new { x.UserId, x.RoleId });
                entity.HasIndex(x => x.Id);

                entity.HasOne(x => x.Role).WithMany(y => y.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.User).WithMany(y => y.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.UserId);
                entity.HasIndex(x => x.RoleId);

                entity.Property(x => x.UserId);
                entity.Property(x => x.RoleId);

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);

            });


            builder.Entity<UserToken>(entity =>
            {
                entity.HasIndex(x => x.Id);
                entity.HasOne(x => x.User).WithMany(y => y.UserTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Person).WithMany(y => y.UserTokens).HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(x => x.AccessTokenHash);
                entity.Property(x => x.RefreshTokenIdHash).HasMaxLength(450).IsRequired();
                entity.Property(x => x.RefreshTokenIdHashSource).HasMaxLength(450);
                entity.Property(x => x.AccessTokenExpiresDateTime);
                entity.Property(x => x.RefreshTokenExpiresDateTime);

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);
            });

            #endregion Users - Roles
            #region Settings

            builder.Entity<Setting>(entity =>
            {
                entity.HasIndex(x => x.Key).IsUnique();

                entity.Property(x => x.Value).IsRequired();
                entity.Property(x => x.FaTitle).IsRequired();
                entity.Property(x => x.DateChanged).IsRequired().HasDefaultValue(DateTime.Now);


                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);
            });

            #endregion Settings


            #region Person
            builder.Entity<Person>(entity =>
            {
                entity.HasIndex(x => x.Id);

                entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
                entity.Property(x => x.LastName).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Gender).IsRequired();
                entity.Property(x => x.Mobile).IsRequired();
                entity.Property(x => x.FileId).IsRequired();
                entity.Property(x => x.Ext).IsRequired();
                entity.Property(x => x.FileName).IsRequired();
                entity.Property(x => x.Grade).IsRequired();
                
                entity.Property(x => x.Address).IsRequired();
                
                entity.Property(x => x.BirthDate).IsRequired();
                entity.Property(x => x.IsVideo);

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);

                entity.HasOne<User>(x => x.User).WithOne(y => y.Person).HasForeignKey<User>(ad => ad.PersonId);

            });

            #endregion Person

            #region Course
            builder.Entity<Course>(entity =>
            {
                entity.HasIndex(x => x.Id);

                entity.Property(x => x.Title).HasMaxLength(150).IsRequired();
                entity.Property(x => x.PersonId).IsRequired();
                entity.Property(x => x.Price).IsRequired();
                entity.Property(x => x.Rate).IsRequired();
                entity.Property(x => x.FileId).IsRequired();
                entity.Property(x => x.Ext).IsRequired();
                entity.Property(x => x.FileName).IsRequired();

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);

                entity.HasOne(x => x.Person).WithMany(y => y.Courses).HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<StudentCourse>(entity =>
            {
                entity.HasIndex(x => x.Id);

                entity.Property(x => x.PersonId).IsRequired();
                entity.Property(x => x.CourseId).IsRequired();

                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
                entity.Property(x => x.CreatedDate).IsRequired().HasDefaultValue(DateTime.Now);
                entity.Property(x => x.UserId);

                entity.HasOne(x => x.Person).WithMany(y => y.StudentCourses).HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Course).WithMany(y => y.StudentCourses).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
            });

            #endregion Course
        }
    }
}
