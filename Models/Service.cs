using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairSalon.Models
{
    [Table("Services")]
    public class Service
    {
        [Key]        
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Название")]        
        public string Name { get; set; }
               
        [Required]        
        [Column("Цена")]        
        public decimal Price { get; set; }

        [MaxLength(1500)]
        [Column("Описание")]        
        public string? Description { get; set; }
        public ICollection<Reception> Receptions { get; set; }
    }
}