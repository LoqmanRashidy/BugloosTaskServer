using CommonLayer;
using Datalayer.Models;
using DataLayer.Context;
using DataLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BaseSystem
{

    public interface IPersonService : IAsyncService<Person>
    {
        List<PersonViewModels> JsonList(List<Person> list);
        PersonViewModels JsonSingle(Person x);
    }
    public class PersonService : GenericService<Person>, IPersonService
    {
        private readonly IUnitOfWork _uow;
        public PersonService(
            ApplicationDbContext context,
            IUnitOfWork uow
            ) : base(context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
        }
        public override async Task<List<Person>> FindConditionAsync(Expression<Func<Person, bool>> predicate, ApplicationDbContext context = null)
        {

            if (context == null) context = _context;
            var list = context.Set<Person>();
            return await list.Where(predicate).ToListAsync();
        }
        public override async Task<Person> FindByIdAsync(long id, ApplicationDbContext context = null)
        {
            try
            {

                if (context == null) context = _context;
                Task<Person> data = context.Set<Person>().Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                return await data;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<Person> AddOrUpdateAsync(Person entity, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                if (entity.Id == 0)
                {
                    await context.Set<Person>().AddAsync(entity);
                }
                else if (entity.Id > 0)
                {
                    var loc = context.Set<Person>().Local.SingleOrDefault(p => p.Id == entity.Id);
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
        public override async Task<(bool IsSuccess, string childs)> RemoveAsync(long id, bool physically = false, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                var _list = context.GetIncludePaths(typeof(Person));
                var entity = await context.Set<Person>().Where(x => x.Id == id)
                         .Include(x => x.StudentCourses).FirstOrDefaultAsync();


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

        public List<PersonViewModels> JsonList(List<Person> list)
        {
            var data = list.Select(x => new PersonViewModels
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                Mobile = x.Mobile,
                Gender = x.Gender,
                GenderTitle = x.GenderTitle,
                BirthDate = x.BirthDate,
                FileId = x.FileId,
                Ext = x.Ext,
                FileName = x.FileName,
            }).ToList();
            return data;
        }

        public PersonViewModels JsonSingle(Person x)
        {
            var data = new PersonViewModels
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                Mobile = x.Mobile,
                Gender = x.Gender,
                GenderTitle = x.GenderTitle,
                BirthDate = x.BirthDate,
                FileId = x.FileId,
                Ext = x.Ext,
                FileName = x.FileName
            };
            return data;
        }
    }
}
