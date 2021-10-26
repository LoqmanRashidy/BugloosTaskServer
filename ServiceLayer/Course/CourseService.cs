
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
using ServicesLayer.Courses;
using System.Linq.Expressions;
using ServiceLayer;
using ServiceLayer.BaseSystem;
using DataLayer.Models;
using Datalayer.Models;

namespace ServicesLayer.Courses
{
    public interface ICourseService : IAsyncService<Course>
    {

        Task<dynamic> FindByItemAsync(string Param, long id = 0, ApplicationDbContext context = null);
        Task<List<Course>> GetCourseByPersonId(long peronId, ApplicationDbContext context = null);
        List<CourseViewModels> JsonList(List<Course> list, long personId = 0);
        CourseViewModels JsonSingle(Course x);
    }

    public class CourseService : GenericService<Course>, ICourseService
    {
        private readonly IUnitOfWork _uow;
        private readonly IPersonService _personService;
        private readonly IStudentCourseService _studentCourseService;
        
        public CourseService(
           ApplicationDbContext context,
             IPersonService personService,
             IStudentCourseService studentCourseService,
            IUnitOfWork uow) : base(context)

        {


            _context = context;
            _context.CheckArgumentIsNull(nameof(context));

            _studentCourseService = studentCourseService;
            _studentCourseService.CheckArgumentIsNull(nameof(studentCourseService));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _personService = personService;
            _personService.CheckArgumentIsNull(nameof(personService));
        }


        public override async Task<List<Course>> FindConditionAsync(Expression<Func<Course, bool>> predicate, ApplicationDbContext context = null)
        {

            if (context == null) context = _context;
            var list = context.Set<Course>()
                     .Include(x => x.Person);
            return await list.Where(predicate).ToListAsync();
        }
        public override async Task<Course> FindByIdAsync(long id, ApplicationDbContext context = null)
        {
            try
            {

                if (context == null) context = _context;
                Task<Course> data = context.Set<Course>().Where(x => x.Id == id)
                    .Include(x => x.Person)
                    .FirstOrDefaultAsync();

                return await data;
            }
            catch
            {
                return null;
            }
        }

        public virtual async Task<List<Course>> GetCourseByPersonId(long peronId, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                Task<List<Course>> data = context.Set<Course>().Where(x => x.PersonId == peronId)
                    .Include(x => x.Person).ToListAsync();
                return await data;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<List<Course>> FindAllAsync(ApplicationDbContext context = null)
        {

            if (context == null) context = _context;
            IQueryable<Course> query = context.Set<Course>().Include(x => x.Person);

            return await query.ToListAsync();
        }
        public override async Task<Course> AddOrUpdateAsync(Course entity, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                if (entity.Id == 0)
                {
                    await context.Set<Course>().AddAsync(entity);
                }
                else if (entity.Id > 0)
                {
                    var loc = context.Set<Course>().Local.SingleOrDefault(p => p.Id == entity.Id);
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
                var list = await context.Set<Course>().Where(x => x.IsActive == true && (x.Person.Name.Contains(Param) || x.Person.LastName.Contains(Param) || x.Person.Mobile.Contains(Param)))
                    .Include(x => x.Person).ToListAsync();

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
                var _list = context.GetIncludePaths(typeof(Course));
                var entity = await context.Set<Course>().Where(x => x.Id == id)
                         .Include(x => x.Person).FirstOrDefaultAsync();


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



        public List<CourseViewModels> JsonList(List<Course> list,long personId=0)
        {
            try
            {
                List<CourseViewModels> data = null;
                if (personId > 0)
                {
                    data = list.Select(x => new CourseViewModels
                    {
                        Id = x.Id,
                        Title = x.Title,
                        PersonId = x.PersonId,
                        Price = x.Price,
                        Rate = x.Rate,
                        FileId = x.FileId,
                        Ext = x.Ext,
                        FileName = x.FileName,
                        CreatedDate = x.CreatedDate,
                        TeacherName = x.Person.Name,
                        TeacherLastName = x.Person.LastName,
                        Gender = x.Person.Gender,
                        GenderTitle = x.Person.GenderTitle,
                        TeacherFileId = x.Person.FileId,
                        TeacherExt = x.Person.Ext,
                        TeacherFileName = x.Person.FileName,
                        IsReg = _studentCourseService.IsExistByCourseId(x.Id, personId).Result
                    }).ToList();
                }
                else
                {
                    data = list.Select(x => new CourseViewModels
                    {
                        Id = x.Id,
                        Title = x.Title,
                        PersonId = x.PersonId,
                        Price = x.Price,
                        Rate = x.Rate,
                        FileId = x.FileId,
                        Ext = x.Ext,
                        FileName = x.FileName,
                        CreatedDate = x.CreatedDate,
                        TeacherName = x.Person.Name,
                        TeacherLastName = x.Person.LastName,
                        Gender = x.Person.Gender,
                        GenderTitle = x.Person.GenderTitle,
                        TeacherFileId = x.Person.FileId,
                        TeacherExt = x.Person.Ext,
                        TeacherFileName = x.Person.FileName,
                        IsReg = false
                    }).ToList();
                }
              
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CourseViewModels JsonSingle(Course x)
        {
            try
            {

                if (x == null) return null;

                var data = new CourseViewModels
                {
                    Id = x.Id,
                    Title = x.Title,
                    PersonId = x.PersonId,
                    Price = x.Price,
                    Rate = x.Rate,
                    FileId = x.FileId,
                    Ext = x.Ext,
                    FileName = x.FileName,
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
