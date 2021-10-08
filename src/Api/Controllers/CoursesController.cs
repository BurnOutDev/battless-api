using Application;
using AutoMapper;
using Domain.Entities;
using Domain.Models.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(
            ICourseService courseService,
            IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        public ActionResult<CourseResponse> Create(CreateCourseRequest model)
        {
            var course = _courseService.Create(model);
            return Ok(course);
        }

        [Authorize(Role.Admin)]
        [HttpPut("{id:guid}")]
        public ActionResult<CourseResponse> Update(Guid id, UpdateCourseRequest model)
        {
            var course = _courseService.Update(id, model);
            return Ok(course);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            _courseService.Delete(id);
            return Ok(new { message = "Course deleted successfully" });
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        public ActionResult<IEnumerable<CourseResponse>> GetAll()
        {
            var courses = _courseService.GetAll();
            return Ok(courses);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public ActionResult<CourseResponse> GetById(Guid id)
        {
            var course = _courseService.GetById(id);
            return Ok(course);
        }
    }
}
