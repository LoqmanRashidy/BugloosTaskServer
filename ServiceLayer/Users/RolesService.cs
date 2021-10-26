using CommonLayer;
using Datalayer.Models.Users;
using DataLayer.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ServiceLayer.Users
{
    public interface IRolesService : IAsyncService<Role>
    {
        Task<List<Role>> FindUserRolesAsync(long userId, ApplicationDbContext context = null);
        Task<bool> IsUserInRoleAsync(int userId, string roleName, ApplicationDbContext context = null);
        Task<List<User>> FindUsersInRoleAsync(string roleName, ApplicationDbContext context = null);
        dynamic JsonList(List<Role> list);
        dynamic JsonSingle(Role x);
    }

    public class RolesService : GenericService<Role>, IRolesService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Role> _roles;
        private readonly IUsersService _usersService;
        private readonly ISecurityService _securityService;
        private readonly IHttpContextAccessor _contextAccessor;

        public RolesService(
              ApplicationDbContext context,
              IUnitOfWork uow,
              IUsersService usersService,
              ISecurityService securityService,
              IHttpContextAccessor contextAccessor) : base(context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _roles = _uow.Set<Role>();


            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(securityService));

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(contextAccessor));
        }

        public Task<List<Role>> FindUserRolesAsync(long userId, ApplicationDbContext context = null)
        {
            IQueryable<Role> userRolesQuery = from role in _roles
                                              from userRoles in role.UserRoles
                                              where userRoles.UserId == userId
                                              select role;

            return userRolesQuery.OrderBy(x => x.EnTitle).ToListAsync();
        }

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName, ApplicationDbContext context = null)
        {
            IQueryable<Role> userRolesQuery = from role in _roles
                                              where role.EnTitle == roleName
                                              from user in role.UserRoles
                                              where user.UserId == userId
                                              select role;
            Role userRole = await userRolesQuery.FirstOrDefaultAsync();
            return userRole != null;

        }

        public Task<List<User>> FindUsersInRoleAsync(string roleName, ApplicationDbContext context = null)
        {
            IQueryable<long> roleUserIdsQuery = from role in _roles
                                   where role.EnTitle == roleName
                                   from user in role.UserRoles
                                   select user.UserId;
            Task<List<User>> userInRoles = _usersService.FindConditionAsync(x => roleUserIdsQuery.Contains(x.Id));
            return userInRoles;
        }

        public override async Task<List<Role>> FindAllAsync(ApplicationDbContext context = null)
        {
            try
            {
                var query = _context.Role.Where(x => x.IsDelete == false)
                    .Include(x => x.UserRoles).ThenInclude(ur => ur.User);

                return await query.ToListAsync();
            }
            catch 
            {
                return Enumerable.Empty<Role>().ToList();
            }

        }

        public override async Task<Role> FindByIdAsync(long id, ApplicationDbContext context = null)
        {
            try
            {
                var query = _context.Role.Where(x => x.IsDelete == false)
                    .Include(x => x.UserRoles).ThenInclude(ur => ur.User);

                var item = await query.FirstOrDefaultAsync(x => x.Id == id);

                return item;

            }
            catch
            {
                return null;
            }
        }

        public dynamic JsonList(List<Role> list)
        {
            try
            {
                var data = list.Select(x => new {
                    x.Id,
                    x.EnTitle,
                    x.FaTitle,
                    UserRoles = x.UserRoles != null ? x.UserRoles.Select(y => new { y.Id, y.IsDelete, y.RoleId, y.UserId, User = new { y.User.Id, y.User.Email, y.User.IsDelete, y.User.IsSystem, y.User.Mobile, y.User.Name, y.User.Password, y.User.PersianLastLoggedIn, y.User.PersonId, y.User.SerialNumber, y.User.Username } }) : null,

                });
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public dynamic JsonSingle(Role x)
        {
            try
            {
                var data = new
                {
                    x.Id,
                    x.EnTitle,
                    x.FaTitle,
                    UserRoles = x.UserRoles != null ? x.UserRoles.Select(y => new { y.Id, y.IsDelete, y.RoleId, y.UserId, User = new { y.User.Id, y.User.Email, y.User.IsDelete, y.User.IsSystem, y.User.Mobile, y.User.Name, y.User.Password, y.User.PersianLastLoggedIn, y.User.PersonId, y.User.SerialNumber, y.User.Username } }) : null,

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
