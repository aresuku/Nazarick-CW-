using HairSalon.Data;
using HairSalon.DTOs;
using Microsoft.EntityFrameworkCore;
using HairSalon.Models;
namespace HairSalon.Services
{
    public interface IMasterService
    {
        Task<IEnumerable<MasterDto>> GetFormattedMastersAsync(IEnumerable<Master> master);//получение всех мастеров (форматировано)
        Task<IEnumerable<AdminMasterDto>> GetFormattedMastersForAdminAsync(IEnumerable<Master> masters);//получение всех мастеров (форматировано) (админ)
        Task<MasterDto> GetMasterByIdAsync(Master master);// Получение одного мастера по id
        Task<AdminMasterDto> GetMasterByIdForAdminAsync(Master master);//получение одного мастера(по id) (админ)
        Task<Master> CreateMasterWithUserAsync(CreateMasterDto createDto, IUserService userService);//создание мастера с нуля (с автоматическим созданием пользователя)
        Task<Master> CreateMasterFromUserAsync(CreateMasterFromUserDto createDto);//создание мастера из user
        Task<MasterDto> UpdateMasterAsync(Master existingMaster, UpdateMasterDto updateDto);//обновление информации о мастере
        Task<bool> DeleteMasterAsync(Master master, bool deleteRelatedUser = true);// удаление мастера
        public MasterDto MapToMasterDto(Master master);// маппинга Master -> MasterDto
        public AdminMasterDto MapToMasterAdminDto(Master master);// маппинг для админа
        string GetServiceInfo();//инфа о сервисе
    }   
    public class MasterService: IMasterService
    {
        private readonly IConfiguration _configuration;
        private readonly HairSalonContext _context;
        private readonly IUserService _userService;
        public MasterService(IConfiguration configuration, HairSalonContext context, IUserService userService)
        {
            _configuration = configuration;
            _context = context;
            _userService = userService;
        }        
        public Task<IEnumerable<MasterDto>> GetFormattedMastersAsync(IEnumerable<Master> Master)// Получение всех мастеров с форматированием
        {            
            var MaxItems = _configuration.GetValue<int>("AppSettings:MaxItems");
            var Formattedmasters = Master
                .OrderBy(m => m.MasterId)
                .Take(MaxItems)
                .Select(m => MapToMasterDto(m));                
            return Task.FromResult(Formattedmasters);            
        }
        public Task<MasterDto> GetMasterByIdAsync(Master master)// Получение одного мастера по id
        {
            if (master == null)
                return Task.FromResult<MasterDto>(null);
            return Task.FromResult(MapToMasterDto(master));
        }
        public Task<IEnumerable<AdminMasterDto>> GetFormattedMastersForAdminAsync(IEnumerable<Master> masters)
        {
            var formattedMasters = masters
                .OrderBy(m => m.MasterId) 
                .Select(m => MapToMasterAdminDto(m));
            return Task.FromResult(formattedMasters);
        }//получение всех мастеров (форматировано) (админ)
        public Task<AdminMasterDto> GetMasterByIdForAdminAsync(Master master)//получение одного мастера(по id) (админ)
        {
            if (master == null)
                return Task.FromResult<AdminMasterDto>(null);
            return Task.FromResult(MapToMasterAdminDto(master));
        }
        public async Task<Master> CreateMasterWithUserAsync(CreateMasterDto createDto, IUserService userService)//создание мастера с нуля (с автоматическим созданием пользователя)
        {
            if (string.IsNullOrWhiteSpace(createDto.FirstName))
                throw new ArgumentException("Имя обязательно");

            if (string.IsNullOrWhiteSpace(createDto.LastName))
                throw new ArgumentException("Фамилия обязательна");

            if (string.IsNullOrWhiteSpace(createDto.Experience))
                throw new ArgumentException("Обязательно нужно указать опыт");

            if (!string.IsNullOrWhiteSpace(createDto.Gender))
            {
                if (createDto.Gender != "М" && createDto.Gender != "Ж")
                    throw new ArgumentException("Пол должен быть 'М' или 'Ж'");
            }
            else
            {
                throw new ArgumentException("Пол обязателен");
            }

            if (string.IsNullOrWhiteSpace(createDto.Password))
                throw new ArgumentException("Пароль обязателен для создания мастера");

            if (string.IsNullOrWhiteSpace(createDto.Email))
                throw new ArgumentException("Email обязателен для создания мастера");

            if (!createDto.Email.Contains("@"))
                throw new ArgumentException("неправильный формат email");

            var existingEmailMaster = await _context.Masters.FirstOrDefaultAsync(m => m.Email == createDto.Email);
            if (existingEmailMaster != null)
            {
                throw new InvalidOperationException("Мастер с таким email уже существует");
            }

            var existingUser = await _context.Users //Проверка не занят ли уже этот пользователь(по логину/ email)
                .FirstOrDefaultAsync(u => u.Login == createDto.Email || u.Email == createDto.Email);

            if (existingUser != null)
            {                
                var existingMasterForUser = await _context.Masters//Проверка не связан ли этот пользователь с другим мастером
                    .FirstOrDefaultAsync(m => m.UserId == existingUser.UserId);

                if (existingMasterForUser != null)
                {
                    throw new InvalidOperationException(
                        $"Пользователь '{existingUser.Login}' уже связан с мастером '{existingMasterForUser.FirstName} {existingMasterForUser.LastName}'. " +
                        "Невозможно создать второго мастера для одного пользователя.");
                }
            }

            var user = new User
            {
                Login = createDto.Email,
                PasswordHash = userService.HashPassword(createDto.Password),
                Username = $"{createDto.FirstName} {createDto.LastName}",
                Email = createDto.Email,
                Role = "Master",
                IsActive = true
            };               
            var master = new Master
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Experience = createDto.Experience,
                Gender = createDto.Gender,
                Description = createDto.Description,
                Email = createDto.Email,
                Role = "Master",
                User = user  
            };
            return master;
        }
        public async Task<Master> CreateMasterFromUserAsync(CreateMasterFromUserDto createDto)//создание мастера из user
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(createDto.FirstName))
                throw new ArgumentException("Имя обязательно");

            if (string.IsNullOrWhiteSpace(createDto.LastName))
                throw new ArgumentException("Фамилия обязательна");

            if (string.IsNullOrWhiteSpace(createDto.Experience))
                throw new ArgumentException("Обязательно нужно указать опыт");

            if (!string.IsNullOrWhiteSpace(createDto.Gender))
            {
                if (createDto.Gender != "М" && createDto.Gender != "Ж")
                    throw new ArgumentException("Пол должен быть 'М' или 'Ж'");
            }
            else
            {
                throw new ArgumentException("Пол обязателен");
            }

            // Проверка существования пользователя
            var user = await _context.Users.FindAsync(createDto.UserId);
            if (user == null)
                throw new InvalidOperationException($"Пользователь с ID {createDto.UserId} не найден");

            // Проверка, что пользователь еще не связан с мастером
            var existingMaster = await _context.Masters
                .FirstOrDefaultAsync(m => m.UserId == createDto.UserId);            

            if (existingMaster != null)
                throw new InvalidOperationException($"Пользователь '{user.Login}' уже связан с мастером '{existingMaster.FirstName} {existingMaster.LastName}'");

            // Проверка уникальности email
            var existingEmailMaster = await _context.Masters
                .FirstOrDefaultAsync(m => m.Email == user.Email);

            if (existingEmailMaster != null)
                throw new InvalidOperationException($"Мастер с email '{user.Email}' уже существует");

            user.Role = "Master";
            var master = new Master
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Experience = createDto.Experience,
                Gender = createDto.Gender,
                Description = createDto.Description,
                Email = user.Email,
                Role = "Master",
                UserId = user.UserId,
                User = user
            };

            return master;
        }
        public Task<MasterDto> UpdateMasterAsync(Master existingMaster, UpdateMasterDto updateDto)// Обновление
        {
            
            if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                existingMaster.FirstName = updateDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                existingMaster.LastName = updateDto.LastName;

            if (!string.IsNullOrWhiteSpace(updateDto.Experience))
                existingMaster.Experience = updateDto.Experience;

            if (!string.IsNullOrWhiteSpace(updateDto.Gender))
            {
                if (updateDto.Gender != "М" && updateDto.Gender != "Ж")
                    throw new ArgumentException("Пол должен быть 'М' или 'Ж'");
                existingMaster.Gender = updateDto.Gender;
            }

            if (updateDto.Description != null)
                existingMaster.Description = updateDto.Description;

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
                existingMaster.Email = updateDto.Email;
                        
            if (updateDto.UserId.HasValue)
            { 
                existingMaster.UserId = updateDto.UserId.Value;
                existingMaster.User = null;
            }

            return Task.FromResult(MapToMasterDto(existingMaster));
        }
        public async Task<bool> DeleteMasterAsync(Master master, bool deleteRelatedUser = true)//Удаление мастера
        {
            if (master == null)
                throw new ArgumentNullException(nameof(master), "Мастер не может быть null");

            var hasReceptions = await _context.Receptions//Проверка, есть ли у мастера записи
                .AnyAsync(r => r.MasterId == master.MasterId);
            if (hasReceptions)
            {
                throw new InvalidOperationException(
                    $"Невозможно удалить мастера '{master.FirstName} {master.LastName}', так как у него есть активные записи. " +
                    "Сначала удалите или переназначьте записи.");
            }

            var userId = master.UserId;//сохранение инфы user
            var user = master.User;

            _context.Masters.Remove(master);

            if (deleteRelatedUser && userId > 0)//Если нужно удалить и связанного пользователя
            {
                _context.Users.Remove(user);
            }
            return true;
        }
        public string GetServiceInfo()//инфа о сервисе
        {
            return $"MasterService is running. Processed by: {_configuration["AppSettings:AppName"]}";
        }
        public MasterDto MapToMasterDto(Master master)// маппинга Master -> MasterDto
        {
            if (master == null) return null;

            var masterDto = new MasterDto
            {
                Id = master.MasterId,
                FirstName = master.FirstName,
                LastName = master.LastName,
                Experience = master.Experience,
                Gender = master.Gender,
                Description = master.Description,
                Role = master.Role,
                DisplayInfo = $"{master.MasterId}|{master.FirstName} {master.LastName} [{master.Gender}, {master.Experience}]"
            };

            if (master.User != null)
            {
                masterDto.User = _userService.MapToUserDto(master.User);
                masterDto.UserDisplayInfo = $"Связан с пользователем: {master.User.Login}";
            }
            else
            {
                masterDto.UserDisplayInfo = $"ОШИБКА: У мастера ID={master.MasterId} нет связанного пользователя!";
            }

            return masterDto;
        }
        public AdminMasterDto MapToMasterAdminDto(Master master)// маппинг для админа
        {
            if (master == null) return null;

            var masterAdminDto = new AdminMasterDto
            {
                Id = master.MasterId,
                FirstName = master.FirstName,
                LastName = master.LastName,
                Experience = master.Experience,
                Gender = master.Gender,
                Description = master.Description,
                Email = master.Email,
                Role = master.Role,
                DisplayInfo = $"{master.MasterId}|{master.FirstName} {master.LastName} [{master.Gender}, {master.Experience}]"
            };

            if (master.User != null)
            {
                masterAdminDto.User = _userService.MapToAdminUserDto(master.User);                
                masterAdminDto.UserDisplayInfo = $"Связан с пользователем: {master.User.Login} (ID: {master.User.UserId})";
            }
            else
            {
                masterAdminDto.UserDisplayInfo = $"ОШИБКА: У мастера ID={master.MasterId} нет связанного пользователя!";
            }
            return masterAdminDto;
        }        
    }
}