using CommonLayer;
using Datalayer.Models;
using Datalayer.Models.Users;
using DataLayer.Context;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CommonLayer.EnumExtensions;

namespace ServiceLayer
{
    public interface IDbInitializerService
    {
        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        void Initialize();
        void SeedData();
        /// <summary>
        /// Adds some default values to the Db
        /// </summary>
        //void SeedData();
    }

    public class DbInitializerService : IDbInitializerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISecurityService _securityService;

        public DbInitializerService(
          IServiceScopeFactory scopeFactory,
          ISecurityService securityService)
        {
            _scopeFactory = scopeFactory;
            _scopeFactory.CheckArgumentIsNull(nameof(_scopeFactory));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));
        }

        public void Initialize()
        {
            using (IServiceScope servicScope = _scopeFactory.CreateScope())
            {
                using (ApplicationDbContext context = servicScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedData()
        {
            using (IServiceScope serviceScope = _scopeFactory.CreateScope())
            {
                using (ApplicationDbContext context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {

                    #region User           
                    // Add default roles
                    Role adminRole = context.Role.FirstOrDefault(q => q.EnTitle == CustomRoles.Admin) ?? new Role { EnTitle = CustomRoles.Admin, FaTitle = "مدیر سیستم" };
                    Role userRole = context.Role.FirstOrDefault(q => q.EnTitle == CustomRoles.User) ?? new Role { EnTitle = CustomRoles.User, FaTitle = "کاربر معمولی" };
                    Role sellerRole = context.Role.FirstOrDefault(q => q.EnTitle == CustomRoles.Teacher) ?? new Role { EnTitle = CustomRoles.Teacher, FaTitle = "معلم" };
                    Role shoperRole = context.Role.FirstOrDefault(q => q.EnTitle == CustomRoles.Student) ?? new Role { EnTitle = CustomRoles.Student, FaTitle = "دانش آموز" };
                  
                    if (!context.Role.Any())
                    {
                        context.Role.AddRange(
                            shoperRole,
                            sellerRole,
                            userRole,
                            adminRole
                            );
                        context.SaveChanges();

                    }

                    #region Province,City,Station

                    if (!context.Person.Any())
                    {

                        #region Person

                        Person person1 = new Person
                        {
                            Name = "Loghman",
                            LastName = "Rashidi",
                            Mobile = "09158286357",
                            Gender = 1,
                            BirthDate = new DateTime(1990, 1, 1),
                            FileId=new Guid("1a99910e-6b52-48ad-879f-b36be856eb47"),
                            Ext= ".jpg",
                            FileName="loqman.jpg",
                            Grade=2,
                            Address="Khorasan Razavi - Mashhad",
                            Courses = new List<Course>
                            {
                                new Course
                                {
                                    Title=".NET Core programming course",
                                    Price=1000,
                                    Rate=4,

                                    FileId=new Guid("5a21e95a-9794-42fc-b340-7b7c576133f5"),
                                    Ext= ".jpg",
                                    FileName="NET Core.jpg",
                                },
                                 new Course
                                {
                                    Title="Angular programming course",
                                    Price=800,
                                    Rate=4,

                                    FileId=new Guid("9a5a7a0e-1b8d-43ef-8de3-53167058a01d"),
                                    Ext= ".jpg",
                                    FileName="Angular.jpg",
                                },
                                  new Course
                                {
                                    Title="Sql Server programming course",
                                    Price=850,
                                    Rate=3,

                                    FileId=new Guid("78c0c8f9-6c16-4181-b9d7-9e1d4324b5cc"),
                                    Ext= ".jpg",
                                    FileName="Sql.jpg",
                                },
                                   new Course
                                {
                                    Title="C# programming course",
                                    Price=1000,
                                    Rate=5,

                                    FileId=new Guid("8e9a577c-c78e-4c07-8469-fe4d19f03b05"),
                                    Ext= ".jpg",
                                    FileName="C#.jpg",
                                },
                             },
                        };
                        Person person2 = new Person
                        {
                            Name = "Noman",
                            LastName = "Rashidi",
                            Mobile = "09158286358",
                            Gender = 1,
                            BirthDate = new DateTime(1995, 1, 1),
                            FileId = new Guid("0a72f8f2-0b4a-43e1-aaf7-3b5006090f8b"),
                            Ext = ".jpg",
                            FileName = "Noman.jpg",
                            Grade = 2,
                            Address = "Khorasan Razavi - Mashhad",
                            Courses = new List<Course>
                            {
                                new Course
                                {
                                    Title="Forex course",
                                    Price=1100,
                                    Rate=4,

                                    FileId=new Guid("79dae1a4-ef77-421e-a6cc-2c8bab7519e8"),
                                    Ext= ".jpg",
                                    FileName="Forex.jpg",
                                },
                                 new Course
                                {
                                    Title="Accounting course",
                                    Price=600,
                                    Rate=4,

                                    FileId=new Guid("8ef8ede9-682e-4d81-8372-fb32dce80612"),
                                    Ext= ".jpg",
                                    FileName="Accounting.jpg",
                                },
                                  new Course
                                {
                                    Title="Accounting Principles course",
                                    Price=600,
                                    Rate=3,

                                    FileId=new Guid("73c716d6-f471-4474-9d42-c19a3962059c"),
                                    Ext= ".jpg",
                                    FileName="Accounting_Principles.jpg",
                                }
                             },
                        };
                        Person person3 = new Person
                        {
                            Name = "Iman",
                            LastName = "Rashidi",
                            Mobile = "09358735463",
                            Gender = 1,
                            BirthDate = new DateTime(2000, 1, 1),
                            FileId = new Guid("0e3182a5-90df-4d95-9d05-920089adee79"),
                            Ext = ".jpg",
                            FileName = "Iman.jpg",
                            Grade = 3,
                            Address = "Khorasan Razavi - Mashhad",
                            Courses = new List<Course>
                            {
                                new Course
                                {
                                    Title="Nerve control course",
                                    Price=700,
                                    Rate=4,

                                    FileId=new Guid("7dff6daf-251a-45d3-ad77-c4a36fff7c7e"),
                                    Ext= ".jpg",
                                    FileName="Nerve.jpg",
                                },
                                 new Course
                                {
                                    Title="Clinical Psychology course",
                                    Price=800,
                                    Rate=3,

                                    FileId=new Guid("72d4ad04-bf39-4330-b67d-9c22eac5b157"),
                                    Ext= ".jpg",
                                    FileName="Clinical.jpg",
                                },
                                  new Course
                                {
                                    Title="Socialist psychology course",
                                    Price=650,
                                    Rate=3,

                                    FileId=new Guid("61ad3e65-749f-49a9-ba39-fe5c1d26bffd"),
                                    Ext= ".jpg",
                                    FileName="Socialist.jpg",
                                },
                             },
                        };
                        Person person4 = new Person
                        {
                            Name = "Ali",
                            LastName = "Arhami",
                            Mobile = "09158286323",
                            Gender = 1,
                            BirthDate = new DateTime(1990, 1, 1),
                            FileId = new Guid("0c08bba8-5a3d-43a0-8410-b35319482a04"),
                            Ext = ".jpg",
                            Grade = 4,
                            Address = "Khorasan Razavi - Mashhad",
                            FileName = "Ali.jpg",
                            Courses = new List<Course>
                            {
                                new Course
                                {
                                    Title="Advanced Mathematics course",
                                    Price=800,
                                    Rate=4,

                                    FileId=new Guid("65c4155a-8358-499c-8cf7-5a1f93c7175d"),
                                    Ext= ".jpg",
                                    FileName="Mathematics.jpg",
                                },
                                 new Course
                                {
                                    Title="Advanced Physics course",
                                    Price=600,
                                    Rate=3,

                                    FileId=new Guid("5ec07ef4-3e9d-4d8c-8985-34563d27208d"),
                                    Ext= ".jpg",
                                    FileName="Physics.jpg",
                                },
                             },
                        };
                        Person[] Persons = new[] { person4, person3,person2, person1 };
                        context.AddRange(Persons);
                        context.SaveChanges();
                        var defaultUser1 = context.User.FirstOrDefault(q => q.Username == "admin") ?? new User
                        {
                            //UserGroup = oneUserGroup,
                            Person = person1,
                            Username = "admin",
                            Name = "لقمان",
                            LastName = "رشیدی",
                            IsActive = true,
                            IsSystem = true,
                            LastLoggedIn = null,
                            Email = "loqman.rashidi@yahoo.com",
                            Mobile = "09158286357",
                            Password = _securityService.GetSha256Hash("admin"),
                            SerialNumber = Guid.NewGuid().ToString("N")
                        };
                        context.SaveChanges();


                        if (!context.UserRole.Any())
                        {
                            var adminrole1 = new UserRole { Role = adminRole, User = defaultUser1 };
                            context.AddRange(adminrole1);
                            context.SaveChanges();
                        }

                        #endregion Person


                        context.SaveChanges();
                    }

                    #endregion  Province,City,Station

                    //var adminPersons = new[] {


                    // Add Admin user


                    #endregion User


                }
            }
        }
    }
}
