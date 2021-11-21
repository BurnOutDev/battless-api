using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LogoBase64 { get; set; }
        public List<Guid> CourseIds { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
