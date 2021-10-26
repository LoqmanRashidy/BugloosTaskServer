
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using DataLayer.Context;
using CommonLayer;
using DataLayer.ViewModels;
using Datalayer.ViewModels;
using CommonLayer.Extension;
using System.Linq.Expressions;
using ServiceLayer;
using ServiceLayer.BaseSystem;
using DataLayer.Models;
using Datalayer.Models;

namespace ServicesLayer.Courses
{
    public interface IStudentCourseService : IAsyncService<StudentCourse>
    {

        Task<dynamic> FindByItemAsync(string Param, long id = 0, ApplicationDbContext context = null);
        Task<bool> IsExistByCourseId(long id = 0,long personId= 0, ApplicationDbContext context = null);
        Task<List<StudentCourse>> GetStudentCourseByPersonId(long peronId, ApplicationDbContext context = null);
        List<CourseViewModels> JsonList(List<StudentCourse> list);
        CourseViewModels JsonSingle(StudentCourse x);
    }

    public class StudentCourseService : GenericService<StudentCourse>, IStudentCourseService
    {
        private readonly IUnitOfWork _uow;
        private readonly IPersonService _personService;
        public StudentCourseService(
           ApplicationDbContext context,
             IPersonService personService,
            IUnitOfWork uow) : base(context)

        {


            _context = context;
            _context.CheckArgumentIsNull(nameof(context));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _personService = personService;
            _personService.CheckArgumentIsNull(nameof(personService));
        }


        public override async Task<List<StudentCourse>> FindConditionAsync(Expression<Func<StudentCourse, bool>> predicate, ApplicationDbContext context = null)
        {

            if (context == null) context = _context;
            var list = context.Set<StudentCourse>()
                     .Include(x => x.Person).Include(x => x.Course);
            return await list.Where(predicate).ToListAsync();
        }
        public override async Task<StudentCourse> FindByIdAsync(long id, ApplicationDbContext context = null)
        {
            try
            {

                if (context == null) context = _context;
                Task<StudentCourse> data = context.Set<StudentCourse>().Where(x => x.Id == id)
                    .Include(x => x.Person).Include(x => x.Course)
                    .FirstOrDefaultAsync();

                return await data;
            }
            catch
            {
                return null;
            }
        }
        public virtual async Task<bool> IsExistByCourseId(long id = 0,long personId=0, ApplicationDbContext context = null)
        {
            try
            {

                if (context == null) context = _context;
                Task<StudentCourse> data = context.Set<StudentCourse>().Where(x => x.CourseId == id && x.PersonId== personId)
                  
                    .FirstOrDefaultAsync();

                if (data.Result != null && data.Result.Id > 0) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }
        public virtual async Task<List<StudentCourse>> GetStudentCourseByPersonId(long peronId, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                Task<List<StudentCourse>> data = context.Set<StudentCourse>().Where(x => x.PersonId == peronId)
                    .Include(x => x.Person).Include(x => x.Course).ToListAsync();
                return await data;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<List<StudentCourse>> FindAllAsync(ApplicationDbContext context = null)
        {

            if (context == null) context = _context;
            IQueryable<StudentCourse> query = context.Set<StudentCourse>()
                  .Include(x => x.Person).Include(x => x.Course);

            return await query.ToListAsync();
        }
        public override async Task<StudentCourse> AddOrUpdateAsync(StudentCourse entity, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                if (entity.Id == 0)
                {
                    await context.Set<StudentCourse>().AddAsync(entity);
                }
                else if (entity.Id > 0)
                {
                    var loc = context.Set<StudentCourse>().Local.SingleOrDefault(p => p.Id == entity.Id);
                    if (loc != null)
                        context.Entry(loc).State = EntityState.Detached;
                    context.Entry(entity).State = EntityState.Modified;


                }
                if (await context.SaveChangesAsync() > 0)
                {

                    return entity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<dynamic> FindByItemAsync(string Param, long id = 0, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                var list = await context.Set<StudentCourse>().Where(x => x.IsActive == true && (x.Person.Name.Contains(Param) || x.Person.LastName.Contains(Param) || x.Person.Mobile.Contains(Param)))
                      .Include(x => x.Person).Include(x => x.Course).ToListAsync();

                var data = list.Select(x => new { Id = x.Id, PersonId = x.PersonId, FName = x.Person.Name, LName = x.Person.LastName, Mobile = x.Person.Mobile, Type = 1 }).ToList();

                return data;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<(bool IsSuccess, string childs)> RemoveAsync(long id, bool physically = false, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                var _list = context.GetIncludePaths(typeof(StudentCourse));
                var entity = await context.Set<StudentCourse>().Where(x => x.Id == id)
                           .Include(x => x.Person).Include(x => x.Course).FirstOrDefaultAsync();


                var childs = HasChildProperty(entity);

                if (childs.Count == 0 || ((childs.ToArray().Distinct().Count() == 1) && childs[0].Contains("فایل")))
                {
                    if (entity != null && !physically)
                    {
                        entity.IsDelete = true;
                        context.Entry(entity).State = EntityState.Modified;
                    }
                    if (await context.SaveChangesAsync() > 0)
                    {
                        return (true, "");
                    }
                    else
                    {
                        return (false, string.Join("، ", childs));
                    }
                }
                else
                    return (false, string.Join("، ", childs));

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public List<CourseViewModels> JsonList(List<StudentCourse> list)
        {
            try
            {
                var data = list.Select(x => new CourseViewModels
                {
                    Id = x.Course.Id,
                    Title = x.Course.Title,
                    PersonId = x.Course.PersonId,
                    Price = x.Course.Price,
                    Rate = x.Course.Rate,
                    FileId = x.Course.FileId,
                    Ext = x.Course.Ext,
                    FileName = x.Course.FileName,
                    CreatedDate = x.CreatedDate,
                    TeacherName = x.Person.Name,
                    TeacherLastName = x.Person.LastName,
                    Gender = x.Person.Gender,
                    GenderTitle = x.Person.GenderTitle,
                    TeacherFileId = x.Person.FileId,
                    TeacherExt = x.Person.Ext,
                    TeacherFileName = x.Person.FileName,
                    IsReg = true
                }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CourseViewModels JsonSingle(StudentCourse x)
        {
            try
            {

                if (x == null) return null;

                var data = new CourseViewModels
                {
                    Id = x.Course.Id,
                    Title = x.Course.Title,
                    PersonId = x.Course.PersonId,
                    Price = x.Course.Price,
                    Rate = x.Course.Rate,
                    FileId = x.Course.FileId,
                    Ext = x.Course.Ext,
                    FileName = x.Course.FileName,
                    CreatedDate = x.CreatedDate,
                    TeacherName = x.Person.Name,
                    TeacherLastName = x.Person.LastName,
                    Gender = x.Person.Gender,
                    GenderTitle = x.Person.GenderTitle,
                    TeacherFileId = x.Person.FileId,
                    TeacherExt = x.Person.Ext,
                    TeacherFileName = x.Person.FileName
                };
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
