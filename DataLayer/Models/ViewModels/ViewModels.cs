using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{

    public class CourseViewModels
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public long PersonId { get; set; }

        public decimal Price { get; set; }

        public int Rate { get; set; }

        public Guid FileId { get; set; }
        public string Ext { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string TeacherName { get; set; }
        public string TeacherLastName { get; set; }
        public byte Gender { get; set; }
        public string GenderTitle { get; set; }

        public Guid TeacherFileId { get; set; } 
        public string TeacherExt { get; set; }
        public string TeacherFileName { get; set; }
        public bool IsReg { get; set; } = false;

    }


    public class PersonViewModels
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public byte Gender { get; set; } 

        public string GenderTitle { get; set; }
        public DateTime? BirthDate { get; set; }

        public Guid FileId { get; set; } 
        public string Ext { get; set; }
        public string FileName { get; set; }

    }
}
