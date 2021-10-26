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
using ServicesLayer.Courses;

namespace WebApp.Controllers.Courses
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnableCors("BuglossPolicy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = CustomPolicy.AdminTeacherStudent)]
    public class CourseController : ControllerBase
    {
        private readonly IOptionsSnapshot<UploadDownloadRoot> _configuration;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ICourseService _courseService;
        private readonly IStudentCourseService _studentCourseService;
        private readonly IServiceScopeFactory _scopeFactory;
        public CourseController(
         IHttpContextAccessor httpContextAccessor,
         ICourseService courseService,
         IStudentCourseService studentCourseService,
         IOptionsSnapshot<UploadDownloadRoot> configuration,
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


            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(configuration));

            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(hostingEnvironment));
        }

        #region Course

      
        [HttpGet]
        [Route("GetAllCourse")]
        [ProducesResponseType(typeof(List<Course>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCourse()
        {
            try
            {
                var data = await _courseService.FindAllAsync();
                return Ok(data);
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


        [HttpPost]
        [Route("AddEditCourse")]
        [ProducesResponseType(typeof(Course), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddEditCourse([FromBody] Course course)
        {
            try
            {
                await _courseService.AddOrUpdateAsync(course);
                return Ok(course);
          
            }
            catch (Exception ex)
            {

                return BadRequest(new { msg = ex.Message, error = true });
            }
        }

        [Route("AddEditWithUploadCourse")]
        [ProducesResponseType(typeof(Course), (int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<IActionResult> AddEditWithUploadCourse()
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
                            var _course = httpRequest.Form["Course"];
                            StringValues _type = httpRequest.Form["Type"];
                            Course course = JsonConvert.DeserializeObject<Course>(_course);
                            Nullable<Guid> systemFileName = null;
                            List<FileType> fileType = new List<FileType>();
                            string url = _configuration.Value.CourseRoot;
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

                            long courseId = course.Id;

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
                                            if (courseId > 0)
                                            {
                                                systemFileName = course.FileId;
                                                filePath = Path.Combine(uploads, systemFileName.ToString() + Path.GetExtension(course.FileName));
                                                var fileInfo = new System.IO.FileInfo(filePath);
                                                fileInfo.Delete();
                                                course.FileName = file.FileName;
                                                course.Ext = Path.GetExtension(file.FileName);

                                            }
                                            else
                                            {
                                                systemFileName = Guid.NewGuid();
                                                course.FileId = systemFileName.Value;
                                                course.FileName = file.FileName;
                                                course.Ext = Path.GetExtension(file.FileName);
                                            }
                                            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"user-pic-upload", fileName);
                                            filePath = Path.Combine(uploads, systemFileName.ToString() + Path.GetExtension(file.FileName));
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await file.CopyToAsync(fileStream);
                                            }
                                        }
                                    }

                                }

                            }
                            await _courseService.AddOrUpdateAsync(course, context);

                            transaction.Commit();
                            return Ok(course);
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

        [Route("DeleteCourse")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCourse(long id)
        {
            try
            {
                var del = await _courseService.RemoveAsync(id);
                if (del.IsSuccess == true)
                {
                    return Ok(ResourceFa.DeleteSuccess);
                }
                else
                {
                    return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ResourceFa.DeleteForChildError, del.childs) });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ex.Message) });
            }
        }

        [Route("ActiveDeactiveCourse")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ActiveDeactiveCourse(long id)
        {
            try
            {
                var enity = await _courseService.FindByIdAsync(id);
                if (enity.IsActive == true)
                    enity.IsActive = false;
                else
                    enity.IsActive = true;
                await _courseService.UpdateAsync(enity);
                return Ok(ResourceFa.ActiveDeactiveSuccess);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = ResourceFa.ActiveDeactiveError.code, msg = string.Format(ex.Message) });
            }

        }

        #endregion Course

        #region StudentCourse

   
        [HttpGet]
        [Route("GetAllStudentCourse")]
        [ProducesResponseType(typeof(List<StudentCourse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllStudentCourse(long personId)
        {
            try
            {
                var data = await _studentCourseService.GetStudentCourseByPersonId(personId);
                var list = _studentCourseService.JsonList(data);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpGet]
        [Route("GetStudentCourseByCourse")]
        [ProducesResponseType(typeof(List<StudentCourse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStudentCourseByCourse(long courseId)
        {
            try
            {
                var data = await _studentCourseService.FindByIdAsync(courseId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpGet]
        [Route("GetStudentCourse")]
        [ProducesResponseType(typeof(StudentCourse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStudentCourse(long id)
        {
            try
            {
                var entity = await _studentCourseService.FindByIdAsync(id);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [Route("AddEditStudentCourse")]
        [ProducesResponseType(typeof(StudentCourse), (int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<IActionResult> AddEditStudentCourse([FromBody] StudentCourse StudentCourse)
        {
            try
            {
                await _studentCourseService.AddOrUpdateAsync(StudentCourse);
                return Ok(StudentCourse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("DeleteStudentCourse")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteStudentCourse(long id)
        {
            try
            {
                var del = await _studentCourseService.RemoveAsync(id);
                if (del.IsSuccess == true)
                {
                    return Ok(ResourceFa.DeleteSuccess);
                }
                else
                {
                    return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ResourceFa.DeleteForChildError, del.childs) });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ex.Message) });
            }

        }

        [Route("ActiveDeactiveStudentCourse")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ActiveDeactiveStudentCourse(long id)
        {
            try
            {
                var enity = await _studentCourseService.FindByIdAsync(id);
                if (enity.IsActive == true)
                    enity.IsActive = false;
                else
                    enity.IsActive = true;
                await _studentCourseService.UpdateAsync(enity);
                return Ok(ResourceFa.ActiveDeactiveSuccess);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = ResourceFa.ActiveDeactiveError.code, msg = string.Format(ex.Message) });
            }

        }

        #endregion StudentCourse

    }


}
