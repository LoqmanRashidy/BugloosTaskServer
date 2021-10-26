using CommonLayer;
using Datalayer.Models;
using Datalayer.Models.Users;
using Datalayer.ViewModels;
using DataLayer.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;


namespace ServiceLayer.Users
{
    public interface IUsersService : IAsyncService<User>
    {
        Task<string> GetSerialNumberAsync(long userId, ApplicationDbContext context = null);
        Task<User> FindUserAsync(string username, string password, ApplicationDbContext context = null);
        Task<User> FindUserByUserNameAsync(string username, ApplicationDbContext context = null);
        Task UpdateUserLastActivityDateAsync(long userId, ApplicationDbContext context = null);
        Task<User> GetCurrentUserAsync(ApplicationDbContext context = null);
        long GetCurrentUserId(ApplicationDbContext context = null);
        Task<bool> ChangePasswordUser(ChangePasswordByAdmin ChangePasswordByAdmin, ApplicationDbContext context = null);
        Task<(bool Succeeded, string Error)> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        dynamic JsonList(List<User> list);
        dynamic JsonSingle(User x);
    }
    public class UsersService : GenericService<User>, IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Person> _person;
        private readonly DbSet<User> _users;
        private readonly ISecurityService _securityService;
        private readonly IHttpContextAccessor _contextAccessor;

        public UsersService(
               ApplicationDbContext context,
               IUnitOfWork uow,
               ISecurityService securityService,
              IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _person = _uow.Set<Person>();
            _users = _uow.Set<User>();

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(_contextAccessor));
        }

        public async override Task<User> AddAsync(User entity, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            entity.Password = _securityService.GetSha256Hash(entity.Password);
            entity.SerialNumber = new Guid().ToString("N");
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async override Task<User> UpdateAsync(User entity, ApplicationDbContext context = null)
        {
            // In case AsNoTracking is used
            if (context == null) context = _context;
            entity.Password = _securityService.GetSha256Hash(entity.Password);
            entity.SerialNumber = Guid.NewGuid().ToString("N");
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
            //if (entity.Id > 0 && IsEntityAudit(entity)) await _context.ActivityLogs.AddAsync(FillLog(entity, ActionType.Delete));
        }

        public async Task<bool> ChangePasswordUser(ChangePasswordByAdmin ChangePasswordByAdmin, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            context.User.SingleOrDefault(q => q.Id == ChangePasswordByAdmin.Id).Password = _securityService.GetSha256Hash(ChangePasswordByAdmin.Password);
            await context.SaveChangesAsync();
            return true;
        }

        //public async override Task<User> AddOrUpdateAsync(User entity, ApplicationDbContext context = null)
        //{
        //    entity.SerialNumber = Guid.NewGuid().ToString("N");
        //    if (entity.Id > 0)
        //    {
        //        entity.Password = _context.User.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id).Password;
        //        var person = await _person.FindAsync(entity.PersonId);
        //        if (person != null)
        //        {
        //            _context.Entry(entity).State = EntityState.Modified;

        //        }
        //    }
        //    else if (entity.Id == 0)
        //    {
        //        entity.IsActive = true;
        //        await _context.AddAsync(entity);
        //    }

        //    await _context.SaveChangesAsync();
        //    _context.UserRole.RemoveRange(_context.UserRole.Where(x => x.UserId == entity.Id));

        //    foreach (var item in entity.ModelRoles)
        //    {
        //        await _context.UserRole.AddAsync(new UserRole
        //        {
        //            UserId = entity.Id,
        //            RoleId = item.Id
        //        });
        //    }

        //    await _context.SaveChangesAsync();

        //    if (entity != null)
        //    {
        //        var req = await FindByIdAsync(entity.Id);
        //        return entity;
        //    }
        //    return null;
        //}

        public async override Task<User> AddOrUpdateAsync(User entity, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;

            if (entity.Id == 0)
            {
                entity.Password = _securityService.GetSha256Hash(entity.Password);

                entity.SerialNumber = Guid.NewGuid().ToString("N");

                entity.IsActive = true;
                await context.AddAsync(entity);
            }
            else if (entity.Id > 0)
            {
                if (context == null) context = _context;
                entity.Password = context.User.AsNoTracking().SingleOrDefault(q => q.Id == entity.Id).Password;
                var person = await _person.FindAsync(entity.PersonId);
                context.UserRole.RemoveRange(context.UserRole.Where(x => x.UserId == entity.Id));
                foreach (var item in entity.UserRoles)
                {
                    await context.Set<UserRole>().AddAsync(new UserRole
                    {
                        UserId = entity.Id,
                        RoleId = item.Id
                    });

                    await context.SaveChangesAsync();
                }

                if (person != null)
                {
                    entity.UserRoles = null;
                    context.Entry(entity).State = EntityState.Modified;

                }
            }

            //await _context.SaveChangesAsync();
            // this block added because for now userrole and usegroup not update for this user


            await context.SaveChangesAsync();
            //if (entity.Id > 0 && IsEntityAudit(entity)) await _context.ActivityLogs.AddAsync(FillLog(entity, ActionType.Update));
            if (entity != null)
            {
                var req = await FindByIdAsync(entity.Id,context);
                return entity;
            }
            return null;
        }
        public async Task<User> FindUserAsync(string usernam, string password, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                string passwordHash = _securityService.GetSha256Hash(password);

                User user = await _users.Include(x => x.UserRoles).ThenInclude(y => y.Role)
                    .FirstOrDefaultAsync(z => z.Username == usernam && z.Password == passwordHash);
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }
        public async Task<User> FindUserByUserNameAsync(string usernam, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                User user = await _users.Where(z => z.Username == usernam || z.Email == usernam).Include(x => x.UserRoles).ThenInclude(y => y.Role)
                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public async Task<string> GetSerialNumberAsync(long userId, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            User user = await FindByIdAsync(userId);
            return user.SerialNumber;
        }

        public async Task UpdateUserLastActivityDateAsync(long userId, ApplicationDbContext context = null)
        {
            User user = await FindByIdAsync(userId);
            if (user.LastLoggedIn != null)
            {
                TimeSpan updateLastActivityDate = TimeSpan.FromMinutes(2);
                DateTimeOffset currentUtc = DateTimeOffset.UtcNow;
                TimeSpan timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }
            user.LastLoggedIn = DateTime.Now;
            await _uow.SaveChangesAsync();
        }

        public long GetCurrentUserId(ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            var cangeInfo = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified)
                .Select(t => new
                {
                    Original = t.OriginalValues.Properties.ToDictionary(pn => pn, pn => t.OriginalValues[pn]),
                    Current = t.CurrentValues.Properties.ToDictionary(pn => pn, pn => t.CurrentValues[pn]),
                });

            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userId = userDataClaim?.Value;
            return string.IsNullOrWhiteSpace(userId) ? 0 : long.Parse(userId);

        }

        public Task<User> GetCurrentUserAsync(ApplicationDbContext context = null)
        {
            var userId = GetCurrentUserId();
            return FindByIdAsync(userId);
        }
        public async Task<(bool Succeeded,String Error)> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            string currentPasswordHash = _securityService.GetSha256Hash(currentPassword);
            if (currentPasswordHash != user.Password)
            {
                return (false, "کلمه عبور نامعتبر است");
            }
            user.Password = _securityService.GetSha256Hash(newPassword);
            // user.SerialNumber = Guid.NewGuid().ToString("N"); // To force other logins to expire.
            await _uow.SaveChangesAsync();
            return (true, string.Empty);
        }

       public override async Task<List<User>> FindAllAsync(ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            IIncludableQueryable<User, Person> query = context.User
                    .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
                    .Include(p => p.Person);

            return await query.ToListAsync();
        }

        public override async Task<User> FindByIdAsync(long Id, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            IIncludableQueryable<User, Person> query = context.User
                  .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
                   .Include(p => p.Person);
            return await query.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public dynamic JsonList(List<User> list)
        {
            try
            {
                var data = list.Select(x => new
                {
                    x.Id,
                    x.PersonId,
                    x.Username,
                    x.SerialNumber,
                    x.Name,
                    x.Mobile,
                    x.Email,
                    x.IsActive,
                    x.IsSystem,
                    x.LastLoggedIn,
                    x.PersianLastLoggedIn,
                    //x.result,
                    Person = x.Person != null ? new
                    {
                        x.Person.Name,
                        x.Person.LastName,
                    } : null,

                    UserRoles = x.UserRoles != null ? x.UserRoles.Select(y => new { y.Id, y.IsDelete, y.RoleId, y.UserId, Role = new { y.Role.EnTitle, y.Role.FaTitle, y.Role.Id, y.Role.IsDelete } }) : null,
                    UserTokens = x.UserTokens != null ? x.UserTokens.Select(y => new { y.AccessTokenExpiresDateTime, y.AccessTokenHash, y.Id, y.IsDelete, y.RefreshTokenExpiresDateTime, y.RefreshTokenIdHash, y.RefreshTokenIdHashSource, y.UserId }) : null

                });
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public dynamic JsonSingle(User x)
        {
            try
            {


                var data = new
                {
                    x.Id,
                    x.PersonId,
                    x.Username,
                    x.SerialNumber,
                    x.Name,
                    x.LastName,
                    x.Mobile,
                    x.Email,
                    x.IsActive,
                    x.IsSystem,
                    x.LastLoggedIn,
                    x.PersianLastLoggedIn,
                    //x.result,
                    Person = x.Person != null ? new
                    {
                        x.Person.Name,
                        x.Person.LastName
                    } : null,
                    UserRoles = x.UserRoles != null ? x.UserRoles.Select(y => new { y.Id, y.IsDelete, y.RoleId, y.UserId, Role = new { y.Role.EnTitle, y.Role.FaTitle, y.Role.Id, y.Role.IsDelete } }) : null,
                    UserTokens = x.UserTokens != null ? x.UserTokens.Select(y => new { y.AccessTokenExpiresDateTime, y.AccessTokenHash, y.Id, y.IsDelete, y.RefreshTokenExpiresDateTime, y.RefreshTokenIdHash, y.RefreshTokenIdHashSource, y.UserId }) : null

                };
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
