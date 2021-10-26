
using CommonLayer;
using Datalayer.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using static CommonLayer.EnumExtensions;
namespace DataLayer.Models
{
    [Table(nameof(Course), Schema = DbSchemas.Course)]
    public class Course : BaseEntity
    {
        public Course()
        {
            StudentCourses = new List<StudentCourse>();
        }

        [Description("Title")]
        public string Title { get; set; }

        [Description("TeacherId")]
        public long PersonId { get; set; }

        [Description("Price")]
        public decimal Price { get; set; }

        [Description("Rate")]
        public int Rate { get; set; }

        public Guid FileId { get; set; } = new Guid();
        public string Ext { get; set; }
        public string FileName { get; set; }

        public virtual Person Person { get; set; }

        public virtual List<StudentCourse> StudentCourses { get; set; }

    }

    public class CustomerValidator : AbstractValidator<Course>
    {
        public CustomerValidator()
        {
            RuleFor(q => q.PersonId)
                         .InclusiveBetween(1, long.MaxValue)
                         .WithMessage("Person Id Not Valid");
            RuleFor(q => q.Title)
             .NotEmpty()
             .WithMessage("Title Not Emty")
             .MinimumLength(1)
             .WithMessage("The title must have at least one character")
             .MaximumLength(50)
             .WithMessage("The title must have a maximum of 100 characters");


            RuleFor(q => q.Person).SetValidator(q => new PersonValidator()).When(q => q.Person != null);
       
            
           
        }

    }

}
