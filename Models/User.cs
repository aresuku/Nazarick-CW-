using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairSalon.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Логин")]
        public string Login { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Пароль")]
        public string PasswordHash { get; set; } 

        [Required]
        [MaxLength(50)]
        [Column("Имя пользователя")]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("Роль")]
        public string Role { get; set; } = "User"; 

        [Column("Активен")]
        public bool IsActive { get; set; } = true;
        public ICollection<Reception>? Receptions { get; set; }
    }
}