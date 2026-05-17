using HairSalon.DTOs;
using HairSalon.Models;
using Microsoft.EntityFrameworkCore;
using HairSalon.Data;
using System.Security.Cryptography;
using System.Text;
namespace HairSalon.Services
{
    public interface IUserService
    {        
        Task<IEnumerable<UserDto>> GetFormattedUsersAsync(IEnumerable<User> users);// Получение всех пользователей
        Task<IEnumerable<AdminUserDto>> GetFormattedUsersForAdminAsync(IEnumerable<User> users);// Получение всех пользователей(админ)               
        Task<UserDto> GetUserByIdAsync(User user);// Получение одного пользователя
        Task<AdminUserDto> GetUserByIdForAdminAsync(User user);
        Task<UserDto> GetUserByLoginAsync(string login);
        Task<User> CreateUserAsync(CreateUserDto createDto);// Создание пользователя
        Task<UserDto> UpdateUserAsync(User existingUser, UpdateUserDto updateDto);// Обновление пользователя
        Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword);// Смена пароля
        Task<bool> DeleteUserAsync(User user);// Удаление пользователя
        Task<User> AuthenticateAsync(string login, string password);// Аутентификация
        string HashPassword(string password);// Хэширование пароля
        bool VerifyPassword(string password, string hash);//верификация пароля
        public UserDto MapToUserDto(User user);//Маппинг User -> UserDto
        public AdminUserDto MapToAdminUserDto(User user);//Админский маппинг User -> AdminUserDto
        string GetServiceInfo();//инфа о сервисе
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly HairSalonContext _context;
        public UserService(IConfiguration configuration, HairSalonContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public Task<IEnumerable<UserDto>> GetFormattedUsersAsync(IEnumerable<User> users)
        {
            var MaxItems = _configuration.GetValue<int>("AppSettings:MaxItems");
            var formattedUsers = users
                .OrderBy(u => u.UserId)
                .Take(MaxItems)
                .Select(u => MapToUserDto(u));
            return Task.FromResult(formattedUsers);
        }                
        public Task<IEnumerable<AdminUserDto>> GetFormattedUsersForAdminAsync(IEnumerable<User> users)
        {
            var formattedUsers = users
                .OrderBy(u => u.UserId)                
                .Select(u => MapToAdminUserDto(u));
            return Task.FromResult(formattedUsers);
        }
        public Task<UserDto> GetUserByIdAsync(User user)//Получение одного пользователя по объекту(публичный)
        {
            if (user == null)
                return Task.FromResult<UserDto>(null);

            return Task.FromResult(MapToUserDto(user));
        }
        public Task<AdminUserDto> GetUserByIdForAdminAsync(User user)//Получение одного пользователя для админа
        {
            if (user == null)
                return Task.FromResult<AdminUserDto>(null);

            return Task.FromResult(MapToAdminUserDto(user));
        }
        public async Task<UserDto> GetUserByLoginAsync(string login)//Получение пользователя по логину
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Логин обязателен");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
                return null;

            return MapToUserDto(user);
        }
        public async Task<User> CreateUserAsync(CreateUserDto createDto)//Создание пользователя
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(createDto.Login))
                throw new ArgumentException("Логин обязателен");

            if (string.IsNullOrWhiteSpace(createDto.Password))
                throw new ArgumentException("Пароль обязателен");

            if (createDto.Password.Length < 6)
                throw new ArgumentException("Пароль должен быть не менее 6 символов");

            if (string.IsNullOrWhiteSpace(createDto.Username))
                throw new ArgumentException("Имя пользователя обязательно");

            // Проверка уникальности логина
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == createDto.Login);

            if (existingUser != null)
                throw new InvalidOperationException($"Пользователь с логином '{createDto.Login}' уже существует");

            // Проверка уникальности email (если указан)
            if (!string.IsNullOrWhiteSpace(createDto.Email))
            {
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == createDto.Email);

                if (existingEmail != null)
                    throw new InvalidOperationException($"Пользователь с email '{createDto.Email}' уже существует");
            }
            // Создание пользователя
            var user = new User
            {
                Login = createDto.Login,
                PasswordHash = HashPassword(createDto.Password),
                Username = createDto.Username,
                Email = createDto.Email,
                Role = createDto.Role,
                IsActive = true
            };

            return user;
        }
        public async Task<UserDto> UpdateUserAsync(User existingUser, UpdateUserDto updateDto)//Обновление пользователя
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Login))
            {
                //Проверка уникальности нового логина
                var existingLogin = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == updateDto.Login && u.UserId != existingUser.UserId);

                if (existingLogin != null)
                    throw new InvalidOperationException($"Пользователь с логином '{updateDto.Login}' уже существует");

                existingUser.Login = updateDto.Login;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Username))
                existingUser.Username = updateDto.Username;

            if (updateDto.Email != null)
            {
                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                {
                    var existingEmail = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == updateDto.Email && u.UserId != existingUser.UserId);

                    if (existingEmail != null)
                        throw new InvalidOperationException($"Пользователь с email '{updateDto.Email}' уже существует");
                }
                existingUser.Email = updateDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Role))
            {
                if (updateDto.Role != "User" && updateDto.Role != "Master" && updateDto.Role != "Admin")
                    throw new ArgumentException("Роль должна быть 'User', 'Master' или 'Admin'");
                existingUser.Role = updateDto.Role;
            }

            if (updateDto.IsActive.HasValue)
                existingUser.IsActive = updateDto.IsActive.Value;

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                if (updateDto.Password.Length < 6)
                    throw new ArgumentException("Пароль должен быть не менее 6 символов");
                existingUser.PasswordHash = HashPassword(updateDto.Password);
            }

            return MapToUserDto(existingUser);
        }
        public async Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword)// Смена пароля
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!VerifyPassword(currentPassword, user.PasswordHash))
                throw new InvalidOperationException("Текущий пароль указан неверно");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Новый пароль не может быть пустым");

            if (newPassword.Length < 6)
                throw new ArgumentException("Новый пароль должен быть не менее 6 символов");

            user.PasswordHash = HashPassword(newPassword);
            return await Task.FromResult(true); ;
        }
        public async Task<bool> DeleteUserAsync(User user)// Удаление пользователя
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //Проверка связан ли пользователь с мастером
            var isMaster = await _context.Masters
                .AnyAsync(m => m.UserId == user.UserId);

            if (isMaster)
            {
                throw new InvalidOperationException(
                    $"Невозможно удалить пользователя '{user.Login}', так как он связан с мастером. " +
                    "Сначала удалите или переназначьте мастера.");
            }
            return true;
        }
        public async Task<User> AuthenticateAsync(string login, string password)// Аутентификация пользователя
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            if (!user.IsActive)
                return null;

            return user;
        }
        public string HashPassword(string password)// Хэширование пароля
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public bool VerifyPassword(string password, string hash)// Проверка пароля
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }
        public UserDto MapToUserDto(User user)//Маппинг User -> UserDto
        {
            if (user == null) return null;

            return new UserDto
            {
                Id = user.UserId,
                Login = user.Login,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
                DisplayInfo = $"{user.UserId}|{user.Login} [{user.Role}] - {user.Username}"
            };
        }
        public AdminUserDto MapToAdminUserDto(User user)//Админский маппинг User -> AdminUserDto 
        {
            if (user == null) return null;

            return new AdminUserDto
            {
                Id = user.UserId,
                Login = user.Login,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                DisplayInfo = $"{user.UserId}|{user.Login} [{user.Role}] - {user.Username}"
            };
        }
        public string GetServiceInfo()//инфа про серсис
        {
            return $"UserService is running. Processed by: {_configuration["AppSettings:AppName"]}";
        }
    }
}