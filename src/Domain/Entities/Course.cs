using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public bool Deleted { get; set; }
        public DateTime Date { get; set; }

    }

    public class Chapter
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public List<string> Content { get; set; }
        public string ContentType { get; set; }
    }
}
