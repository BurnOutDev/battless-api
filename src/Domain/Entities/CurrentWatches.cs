using System;
namespace Domain.Entities
{
    public class CurrentWatches
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Certificate { get; set; }
        public int CurrentChapter { get; set; }
        public bool IsTestPassed { get; set; }
        public DateTime? LastWhatchDate { get; set; }
    }
}
