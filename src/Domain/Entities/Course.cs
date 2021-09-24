using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
  public class Course
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
