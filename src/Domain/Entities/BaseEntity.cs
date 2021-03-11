using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BaseEntity
    {
        [Key]
        public Guid UId { get; set; } = Guid.NewGuid();
    }
}
