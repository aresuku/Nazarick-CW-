using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairSalon.Models
{
    [Table("Masters")]
    public class Master
    {
        [Key]        
        public int MasterId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]  
        public virtual User User { get; set; }

        [MaxLength(50)]
        [Column("Роль")]
        public string Role { get; set; } = "Master";

        [Required]
        [MaxLength(20)]
        [Column("Имя")]        
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("Фамилия")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Опыт")]
        public string Experience { get; set; }

        [Required]
        [MaxLength(1)]
        [Column("Пол")]
        public string Gender { get; set; }

        [MaxLength(1500)]
        [Column("Описание")]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Column("Адрес почты")]
        public string Email { get; set; }

        public ICollection<Reception> Receptions { get; set; }
    } 
}
