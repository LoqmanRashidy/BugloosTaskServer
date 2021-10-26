using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommonLayer;
using CommonLayer.Custom;
using Datalayer.Models;
using Datalayer.Models.Users;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ServiceLayer.BaseSystem;
using ServiceLayer.Users;
using ServicesLayer.Courses;

namespace WebApp.Controllers.Courses
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnableCors("BuglossPolicy")]
   
    public class FreeController : ControllerBase
    {
        private readonly IOptionsSnapshot<UploadDownloadRoot> _configuration;
        private readonly IPersonService _personService;
        private readonly IUsersService _usersService;
        private readonly IRolesService _rolesService;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ICourseService _courseService;
        private readonly IStudentCourseService _studentCourseService;
        private readonly IServiceScopeFactory _scopeFactory;
        public FreeController(
         IHttpContextAccessor httpContextAccessor,
         ICourseService courseService,
         IStudentCourseService studentCourseService,
         IOptionsSnapshot<UploadDownloadRoot> configuration,
         IPersonService personService,
         IUsersService usersService,
         IRolesService rolesService,
        IServiceScopeFactory scopeFactory,
         IHostingEnvironment hostingEnvironment)
        {
            _scopeFactory = scopeFactory;
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor.CheckArgumentIsNull(nameof(httpContextAccessor));

            _courseService = courseService;
            _courseService.CheckArgumentIsNull(nameof(courseService));

            _studentCourseService = studentCourseService;
            _studentCourseService.CheckArgumentIsNull(nameof(studentCourseService));

            _personService = personService;
            _personService.CheckArgumentIsNull(nameof(personService));

            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _rolesService = rolesService;
            _rolesService.CheckArgumentIsNull(nameof(rolesService));

            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(configuration));

            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(hostingEnvironment));
        }

        #region Course

      
        [HttpGet]
        [Route("GetAllCourse")]
        [ProducesResponseType(typeof(List<Course>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCourse(long personId)
        {
            try
            {
                var data = await _courseService.FindAllAsync();
                var list = _courseService.JsonList(data, personId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }
        }

        [HttpGet]
        [Route("GetCourse")]
        [ProducesResponseType(typeof(Course), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCourse(long id)
        {
            try
            {
                var data = await _courseService.FindByIdAsync(id);
            
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { msg = ex.Message, error = true });
            }
        }




        #endregion Course

        #region Person

        [Route("AddEditWithUploadPerson")]
        [ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<IActionResult> AddEditWithUploadPerson()
        {

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                    {
                        try
                        {
                            var httpRequest = HttpContext.Request;
                            var _person = httpRequest.Form["Person"];
                            StringValues _type = httpRequest.Form["Type"];
                            Person person = JsonConvert.DeserializeObject<Person>(_person);
                            Nullable<Guid> systemFileName = null;
                            List<FileType> fileType = new List<FileType>();
                            string url = _configuration.Value.PersonRoot;
                            string filePath = null;
                            var uploads = Path.Combine(Directory.GetCurrentDirectory(), url);
                            if (!Directory.Exists(uploads))
                            {
                                Directory.CreateDirectory(uploads);
                            }
                            if (_type.Count > 0)
                            {
                                foreach (var item in _type)
                                {
                                    fileType.Add(JsonConvert.DeserializeObject<FileType>(item));
                                }
                            }
                            IFormFileCollection files = null;

                            long personId = person.Id;

                            if (_httpContextAccessor.HttpContext.Request.Form.Files.Count > 0)
                            {
                                files = _httpContextAccessor.HttpContext.Request.Form.Files;
                                int i = 0;

                                if (files != null)
                                {
                                    foreach (var file in files)
                                    {
                                        if (file.Length > 0)
                                        {
                                            var fileName = Path.GetFileName(file.FileName);
                                            FileType item = fileType.Where(x => x.Name == fileName && x.IsRead == false).FirstOrDefault();
                                            var itemIndex = fileType.FindIndex(x => x.Name == fileName && x.Type == item.Type && x.IsRead == false);
                                            item.IsRead = true;
                                            fileType[itemIndex] = item;
                                            if (personId > 0)
                                            {
                                                systemFileName = person.FileId;
                                                filePath = Path.Combine(uploads, systemFileName.ToString() + Path.GetExtension(person.FileName));
                                                var fileInfo = new System.IO.FileInfo(filePath);
                                                fileInfo.Delete();
                                                person.FileName = file.FileName;
                                                person.Ext = Path.GetExtension(file.FileName);

                                            }
                                            else
                                            {
                                                systemFileName = Guid.NewGuid();
                                                person.FileId = systemFileName.Value;
                                                person.FileName = file.FileName;
                                                person.Ext = Path.GetExtension(file.FileName);


                                            }

                                            filePath = Path.Combine(uploads, systemFileName.ToString() + Path.GetExtension(file.FileName));
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await file.CopyToAsync(fileStream);
                                            }
                                        }
                                    }

                                }

                            }

                            await _personService.AddOrUpdateAsync(person, context);

                            if (personId <=0)
                            {
                                User agoUser = await _usersService.FindUserByUserNameAsync(person.Mobile, context);

                                if (agoUser == null)
                                {

                                    User user = new User();
                                    user.PersonId = person.Id;
                                    user.Username = person.Mobile;
                                    user.Name = person.Name;
                                    user.LastName = person.LastName;
                                    user.IsActive = true;
                                    user.IsSystem = true;
                                    user.LastLoggedIn = null;
                                    user.Mobile = person.Mobile;
                                    user.Password = person.Password;
                                    user.Email = "loqman.rashidi@yahoo.com";
                                    Role role = _rolesService.FindConditionAsync(q => q.EnTitle == CustomRoles.Student, context).Result.FirstOrDefault();
                                    var userrole = new UserRole { RoleId = role.Id, UserId = user.Id };
                                    user.UserRoles.Add(userrole);
                                    await _usersService.AddOrUpdateAsync(user, context);
                                }
                                else
                                    return BadRequest(new { msg = ResourceFa.ErrorUserDuplicated.message, error = true, code = ResourceFa.ErrorUserDuplicated.code });

                            }


                            transaction.Commit();
                            return Ok(person);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            if (ex.HResult == -2146233088)
                                return BadRequest(new { msg = ResourceFa.ErrorSameTransactionAgoRuning.message, error = true, code = ResourceFa.ErrorSameTransactionAgoRuning.code });
                            else
                                return BadRequest(new { msg = ex.Message, error = true, code = ex.HResult });
                        }
                    }
                }
            }
        }

        #endregion Person

        #region GetEnum

        #region PersonEnum



        [HttpGet]
        [Route("GetEnumGenderType")]
        [ProducesResponseType(typeof(List<object>), (int)HttpStatusCode.OK)]
        public IActionResult GetEnumGenderType()
        {
            try
            {
                var data = EnumExtensions.EnumToList(typeof(EnumExtensions.EnumGenderType));
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpGet]
        [Route("GetEnumGrade")]
        [ProducesResponseType(typeof(List<object>), (int)HttpStatusCode.OK)]
        public IActionResult GetEnumGrade()
        {
            try
            {
                var data = EnumExtensions.EnumToList(typeof(EnumExtensions.EnumGrade));
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }
        #endregion PersonEnum


        #endregion GetEnum
    }



}
