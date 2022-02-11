#nullable disable
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The cover type name is required.")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
