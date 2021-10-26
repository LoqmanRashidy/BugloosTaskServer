
using CommonLayer;
using CommonLayer.Helper;
using Datalayer.Models.Users;
using DataLayer.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;
using static CommonLayer.EnumExtensions;

namespace Datalayer.Models
{
    [Table(nameof(Person), Schema = DbSchemas.BaseSystem)]
    public class Person : BaseEntity
    {
        public Person()
        {
            Courses = new List<Course>();
            StudentCourses = new List<StudentCourse>();
            UserTokens = new List<UserToken>();
        }


        public string Name { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public byte Gender { get; set; } = EnumGenderType.Man.ToByte();

        [NotMapped]
        public string GenderTitle { get { return GetEnumDescription((EnumGenderType)Gender); } }
        public DateTime? BirthDate { get; set; }

        public Guid FileId { get; set; } = new Guid();
        public string Ext { get; set; }
        public string FileName { get; set; }

        public string Address { get; set; }
        public byte Grade { get; set; } = EnumGrade.Bachelor.ToByte();
        public string GradeTitle { get { return GetEnumDescription((EnumGrade)Grade); } }
        public bool IsVideo { get; set; }

        [NotMapped]
        public string Password { get; set; }
        public virtual User User { get; set; }

        public virtual List<Course> Courses { get; set; }
        public virtual List<UserToken> UserTokens { get; set; }
        public virtual List<StudentCourse> StudentCourses { get; set; }
    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(q => q.Name)
                .NotEmpty()
                .WithMessage("Name Not Emty")
                .MinimumLength(1)
                .WithMessage("The name must have at least one character")
                .MaximumLength(50)
                .WithMessage("The name must have a maximum of 50 characters");

            RuleFor(q => q.LastName)
                .NotEmpty()
                .WithMessage("Last Name Not Emty")
                .MinimumLength(1)
                .WithMessage("The lastname must have at least one character")
                .MaximumLength(50)
                .WithMessage("The lastname must have a maximum of 50 characters");

            RuleFor(q => q.Gender)
              .InclusiveBetween((byte)0, (byte)Enum.GetNames(typeof(EnumGenderType)).Length)
           .WithMessage("Gender Not Valid");

            

        }


    }
}
