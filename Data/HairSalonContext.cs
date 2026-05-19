using Microsoft.EntityFrameworkCore;
using HairSalon.Models;

namespace HairSalon.Data
{
    public class HairSalonContext : DbContext
    {
        public DbSet<Master> Masters { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Reception> Receptions { get; set; }
        public DbSet<User> Users { get; set; }

        public HairSalonContext()
        {
        }

        public HairSalonContext(DbContextOptions<HairSalonContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                                 
            modelBuilder.Entity<Reception>()// Запрет на создание двух записей для одного мастера в одно время
                .HasIndex(r => new { r.MasterId, r.Time })
                .IsUnique()
                .HasDatabaseName("Receptions_MasterId_Time");

            
            modelBuilder.Entity<Master>()// Уникальный email у мастеров
                .HasIndex(m => m.Email)
                .IsUnique()
                .HasDatabaseName("Masters_Email");
            
            modelBuilder.Entity<User>()// Уникальный логин у пользователей
                .HasIndex(u => u.Login)
                .IsUnique()
                .HasDatabaseName("Users_Login");
            
            modelBuilder.Entity<User>()// Уникальный email у пользователей
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("Users_Email");
            
            modelBuilder.Entity<Service>()// Уникальное имя услуги
                .HasIndex(s => s.Name)
                .IsUnique()
                .HasDatabaseName("Services_Name");
           
            modelBuilder.Entity<Reception>() // Reception -> Master (1:N)
                .HasOne(r => r.Master)
                .WithMany(m => m.Receptions)
                .HasForeignKey(r => r.MasterId)
                .OnDelete(DeleteBehavior.Restrict);  // Запрет каскадного удаления
                        
            modelBuilder.Entity<Reception>()// Reception -> Service (1:N)
                .HasOne(r => r.Service)
                .WithMany(s => s.Receptions)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
                        
            modelBuilder.Entity<Reception>()// Reception -> Client (User) (1:N)
                .HasOne(r => r.Client)
                .WithMany(u => u.Receptions)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.SetNull);  // При удалении пользователя - ClientId = NULL

            
            modelBuilder.Entity<Master>()// Master -> User (1:1)
                .HasOne(m => m.User)
                .WithOne()
                .HasForeignKey<Master>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // При удалении пользователя - удаляется мастер

                       

            modelBuilder.Entity<Service>()// Настройка decimal
                .Property(s => s.Price)
                .HasPrecision(18, 2);
                        


            DbSeeder.Seed(modelBuilder);
        }
    }
}