using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer
{
    public class DbSchemas
    {
        public const string BaseSystem = nameof(BaseSystem);
        public const string Users = nameof(Users);
        public const string Course = nameof(Course);
        
    }

    public static class CustomRoles
    {
        public const string Admin = nameof(Admin);
        public const string Teacher = nameof(Teacher);
        public const string Student = nameof(Student);
        public const string User = nameof(User);
    }

    public static class CustomPolicy
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public const string Teacher = nameof(Teacher);
        public const string Student = nameof(Student);
        public const string AdminTeacherStudent = nameof(AdminTeacherStudent);
    }

}
