
using CommonLayer;
using Datalayer.Models.Users;
using DataLayer.Models;
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

namespace Datalayer.Models
{
    [Table(nameof(StudentCourse), Schema = DbSchemas.Course)]
    public class StudentCourse : BaseEntity
    {
        public StudentCourse()
        {
        }
        public long PersonId { get; set; }
        public long CourseId { get; set; }
     
        public virtual Person Person { get; set; }
        public virtual Course Course { get; set; }
    }

    public class AdressValidator : AbstractValidator<StudentCourse>
    {
        public AdressValidator()
        {
            RuleFor(q => q.PersonId)
                         .InclusiveBetween(1, long.MaxValue)
                         .WithMessage("Not Valid Student Id");


            RuleFor(q => q.CourseId)
                              .InclusiveBetween(1, long.MaxValue)
                              .WithMessage("Not Valid Course Id");

        }

    }

}
