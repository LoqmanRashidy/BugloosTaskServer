
using CommonLayer;
using Datalayer.Models;
using DataLayer.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.BaseSystem
{
    public interface ISettingService : IAsyncService<Setting>
    {
        //Task<Setting> AddOrUpdateAsync(Setting entity);
        Task<(bool IsSuccess, string childs)> RemoveByKeyAsync(string key, bool physically = false, ApplicationDbContext context = null);
        //Task<List<Setting>> FindAllAsync();
        Task<Setting> FindByKeyIdAsync(string key, ApplicationDbContext context = null);

        //List<string> GetAllTableName();
    }
    
    public class SettingService: GenericService<Setting>,ISettingService 
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        public SettingService(ApplicationDbContext context, IUnitOfWork uow) : base(context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(context));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            //_attachFile = _uow.Set<AttachFile>();
        }

        public override async Task<Setting> AddOrUpdateAsync(Setting entity, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            var duplicated = await context.Setting.AnyAsync(x => x.Key.Equals(entity.Key));
            if (!duplicated)
            {            
                await context.Set<Setting>().AddAsync(entity);
            }
            else
            {
                context.Entry(entity).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
            return entity;
          
        }

        public  async Task<(bool IsSuccess, string childs)> RemoveByKeyAsync(string key, bool physically = false, ApplicationDbContext context = null)
        {

            if (context == null) context = _context;
            var entity = context.Set<Setting>().Where(x=> x.Key==key).SingleOrDefault();
            var childs = HasChildProperty(entity);

            if (childs.Count == 0 || ((childs.ToArray().Distinct().Count() == 1) && childs[0].Contains("فایل")))
            {
                if (!physically)
                {
                    entity.IsDelete = true;
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

        public async Task<Setting> FindByKeyIdAsync(string key, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            var query = context.Set<Setting>().Where(x => x.IsDelete == false && x.Key.Equals(key));

            return await query.SingleOrDefaultAsync();
        }

       
    }
}
