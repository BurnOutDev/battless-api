using AutoMapper;
using Domain.Entities;
using Domain.Models.Courses;
using MongoDB.Driver;
using Persistence;
using System;
using System.Collections.Generic;

namespace Application
{
    public interface ICourseService : BaseService<CreateCourseRequest, UpdateCourseRequest, CourseResponse>
    {

    }

    public class CourseService : ICourseService
    {
        private readonly MongoDbRepository<Course> _context;
        private readonly IMapper _mapper;

        public CourseService(
            MongoDbRepository<Course> context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        public CourseResponse Create(CreateCourseRequest model)
        {
            // map model to new account object
            var course = _mapper.Map<Course>(model);

            course.Date = DateTime.Now;

            // save account
            _context.Collection.InsertOne(course);

            return _mapper.Map<CourseResponse>(course);
        }

        public void Delete(Guid id)
        {
            var course = GetCourse(id);
            _context.Collection.DeleteOne(x => x.Id == course.Id);
        }

        public IEnumerable<CourseResponse> GetAll()
        {
            var courses = _context.Collection.Find(FilterDefinition<Course>.Empty).ToList();
            return _mapper.Map<IList<CourseResponse>>(courses);
        }

        public CourseResponse GetById(Guid id)
        {
            var course = _context.Collection.Find(x => x.Id == id).FirstOrDefault();
            if (course == null) throw new KeyNotFoundException("Course not found");
            return _mapper.Map<CourseResponse>(course);
        }

        public CourseResponse Update(Guid id, UpdateCourseRequest model)
        {
            var course = GetCourse(id);

            // copy model to course and save
            _mapper.Map(model, course);
            _context.Collection.ReplaceOne(x => x.Id == course.Id, course);

            return _mapper.Map<CourseResponse>(course);
        }

        private Course GetCourse(Guid id)
        {
            var course = _context.Collection.Find(x => x.Id == id).FirstOrDefault();
            if (course == null) throw new KeyNotFoundException("Course not found");
            return course;
        }
    }
}
