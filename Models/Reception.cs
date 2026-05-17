using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairSalon.Models
{
    [Table("Receptions")]
    public class Reception
    {
        [Key]        
        public int Id { get; set; }

        [Required]        
        [Column("Время записи")]
        public DateTime Time { get; set; }
        public int MasterId { get; set; }
        public int ServiceId { get; set; }
        public int? ClientId { get; set; }
        public Master Master { get; set; }
        public Service Service { get; set; }

        [ForeignKey("ClientId")]
        public virtual User? Client { get; set; }  // Клиент, который записался
    }
}
