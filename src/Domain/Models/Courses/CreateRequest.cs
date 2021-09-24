using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Domain.Models.Courses
{
  public class CreateRequest
  {
    public string Title { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Duration { get; set; }
    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    public bool Deleted { get; set; }
  }

  public class Chapter
  {
    public string Order { get; set; }
    public string Content { get; set; }
    public string ContentType { get; set; }
  }
}