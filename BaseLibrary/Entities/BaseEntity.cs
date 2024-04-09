
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
