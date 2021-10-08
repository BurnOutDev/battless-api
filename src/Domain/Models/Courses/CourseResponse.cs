using System;
using System.Collections.Generic;

namespace Domain.Models.Courses
{
    public class CourseResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public List<ChapterResponse> Chapters { get; set; } = new List<ChapterResponse>();
        public DateTime Date { get; set; }
    }

    public class ChapterResponse
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
}