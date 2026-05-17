using Microsoft.EntityFrameworkCore;
using HairSalon.Models;
namespace HairSalon.Data
{
    public static class DbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Login = "Satoru", PasswordHash = "hash1", Username = "Momonga", Email = "YGGDRASIL@gmail.com", Role = "Master", IsActive = true },
                new User { UserId = 2, Login = "Sorcerer King", PasswordHash = "hash2", Username = "Ainz Ooal Gown", Email = "Overlord@gmail.com", Role = "Admin", IsActive = true },
                new User { UserId = 3, Login = "Tanya", PasswordHash = "hash3", Username = "Gun", Email = "Degurechaff@gmail.com", Role = "Master", IsActive = true },
                new User { UserId = 4, Login = "albedo", PasswordHash = "hash4", Username = "LvAnz", Email = "albedo@gmail.com", Role = "User", IsActive = true },
                new User { UserId = 5, Login = "Garuganchua", PasswordHash = "hash5", Username = "Garuganchua", Email = "Garuganchua@gmail.com", Role = "User", IsActive = true }
            );            
            modelBuilder.Entity<Master>().HasData(
                new Master { MasterId = 1, UserId = 1, FirstName = "Сатору", LastName = "Судзуки", Experience = "100 Лет", Gender = "М", Description = "по умолчанию 1", Email = "YGGDRASIL@gmail.com", Role = "Master" },                
                new Master { MasterId = 2, UserId = 3, FirstName = "Таня", LastName = "Дёгурешафф", Experience = "100 Лет", Gender = "Ж", Description = "по умолчанию 3", Email = "Degurechaff@gmail.com", Role = "Master" }
            );
            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceId = 1, Name = "Стрижка", Price = 1000, Description = "по умолчанию 1" },
                new Service { ServiceId = 2, Name = "Окрашивание", Price = 2000, Description = "по умолчанию 2" },
                new Service { ServiceId = 3, Name = "Восстановление", Price = 3000, Description = "по умолчанию 3" }
            );
            modelBuilder.Entity<Reception>().HasData(
                new Reception { Id = 1, Time = new DateTime(2138, 12, 1, 20, 30, 0), MasterId = 1, ServiceId = 1 },
                new Reception { Id = 2, Time = new DateTime(2138, 12, 2, 19, 30, 0), MasterId = 2, ServiceId = 2 },
                new Reception { Id = 3, Time = new DateTime(2138, 12, 3, 12, 30, 0), MasterId = 2, ClientId = 5, ServiceId = 3 }
            );
        }
    }
}