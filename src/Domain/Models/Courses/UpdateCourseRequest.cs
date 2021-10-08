using System;
using System.Collections.Generic;

namespace Domain.Models.Courses
{
    public class UpdateCourseRequest
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public List<UpdateChapter> Chapters { get; set; } = new List<UpdateChapter>();
    }
    public class UpdateChapter
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
    }
}