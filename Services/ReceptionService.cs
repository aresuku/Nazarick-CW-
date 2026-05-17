using HairSalon.Data;
using HairSalon.DTOs;
using HairSalon.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HairSalon.Services
{
    public interface IReceptionService
    {        
        Task<IEnumerable<ReceptionDto>> GetFormattedReceptionsAsync(IEnumerable<Reception> receptions);//Получение всех записей  (публичный)
        Task<IEnumerable<AdminReceptionDto>> GetFormattedReceptionsForAdminAsync(IEnumerable<Reception> receptions);//Получение всех записей (админ)               
        Task<ReceptionDto> GetReceptionByIdAsync(Reception reception);//Получение одной записи (по id) (публичный)
        Task<AdminReceptionDto> GetReceptionByIdForAdminAsync(Reception reception);//Получение одной записи (по id) (админ)                     
        Task<IEnumerable<ReceptionDto>> GetReceptionsByMasterAsync(IEnumerable<Reception> receptions);//Получение записей по мастеру (публичный)
        Task<IEnumerable<AdminReceptionDto>> GetReceptionsByMasterForAdminAsync(IEnumerable<Reception> receptions);//Получение записей по мастеру (админ)         
        Task<IEnumerable<ReceptionDto>> GetReceptionsByClientAsync(IEnumerable<Reception> receptions);//Получение записей по клиенту (публичный)
        Task<IEnumerable<AdminReceptionDto>> GetReceptionsByClientForAdminAsync(IEnumerable<Reception> receptions);//Получение записей по клиенту (админ)         
        Task<Reception> CreateReceptionAsync(CreateReceptionDto createDto, int clientId);//Создание записи (от пользователя)        
        Task<Reception> CreateReceptionByMasterAsync(CreateReceptionByMasterDto createDto, int masterId);//Создание записи (от мастера)        
        Task<ReceptionDto> UpdateReceptionAsync(Reception existingReception, UpdateReceptionDto updateDto);//Обновление записи(публичный)
        Task<AdminReceptionDto> UpdateReceptionForAdminAsync(Reception existingReception, AdminUpdateReceptionForAdminDto updateDto);//Обновление записи(админ)         
        Task<bool> CancelReceptionAsync(Reception reception);//Удаление записи 
        string GetServiceInfo();//Инфа о сервисе
    }

    public class ReceptionService : IReceptionService
    {
        private readonly IConfiguration _configuration;
        private readonly HairSalonContext _context;
        private readonly IMasterService _masterService;
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;

        public ReceptionService(IConfiguration configuration, HairSalonContext context, IMasterService masterService, IServiceService serviceService, IUserService userService)
        {
            _configuration = configuration;
            _context = context;
            _masterService = masterService;
            _serviceService = serviceService;
            _userService = userService;
        }
        public Task<IEnumerable<ReceptionDto>> GetFormattedReceptionsAsync(IEnumerable<Reception> receptions)//Получение всех записей с форматированием  (публичный)
        {
            var MaxItems = _configuration.GetValue<int>("AppSettings:MaxItems");

            var formattedReceptions = receptions
                .OrderBy(r => r.Id)
                .Take(MaxItems)
                .Select(r => MapToReceptionDto(r));
            return Task.FromResult(formattedReceptions);
        }
        public Task<IEnumerable<AdminReceptionDto>> GetFormattedReceptionsForAdminAsync(IEnumerable<Reception> receptions)//Получение всех записей(админ)
        {
            var formattedReceptions = receptions
                .OrderBy(r => r.Id)
                .Select(r => MapToAdminReceptionDto(r));
            return Task.FromResult(formattedReceptions);
        }
        public Task<ReceptionDto> GetReceptionByIdAsync(Reception reception)//Получение одной записи (по id) (публичный)
        {
            if (reception == null)
                return Task.FromResult<ReceptionDto>(null);
            return Task.FromResult(MapToReceptionDto(reception));
        }
        public Task<AdminReceptionDto> GetReceptionByIdForAdminAsync(Reception reception)//Получение одной записи (по id) (админ) 
        {
            if (reception == null)
                return Task.FromResult<AdminReceptionDto>(null);
            return Task.FromResult(MapToAdminReceptionDto(reception));
        }
        public Task<IEnumerable<ReceptionDto>> GetReceptionsByMasterAsync(IEnumerable<Reception> receptions)//Получение записей по мастеру (публичный)
        {            
            var formattedReceptions = receptions
                .OrderBy(r => r.Time)
                .Select(r => MapToReceptionDto(r));
            return Task.FromResult(formattedReceptions);
        }
        public Task<IEnumerable<AdminReceptionDto>> GetReceptionsByMasterForAdminAsync(IEnumerable<Reception> receptions)//Получение записей по мастеру для админа
        {
            var formattedReceptions = receptions
                .OrderBy(r => r.Time)
                .Select(r => MapToAdminReceptionDto(r));
            return Task.FromResult(formattedReceptions);
        }
        public Task<IEnumerable<ReceptionDto>> GetReceptionsByClientAsync(IEnumerable<Reception> receptions)//Получение записей по клиенту (публичный)
        {
            var formattedReceptions = receptions
                .OrderBy(r => r.Time)
                .Select(r => MapToReceptionDto(r));
            return Task.FromResult(formattedReceptions);
        }
        public Task<IEnumerable<AdminReceptionDto>> GetReceptionsByClientForAdminAsync(IEnumerable<Reception> receptions)//Получение записей по клиенту (админ) 
        {
            var formattedReceptions = receptions
                .OrderBy(r => r.Time)
                .Select(r => MapToAdminReceptionDto(r));
            return Task.FromResult(formattedReceptions);
        }
        public async Task<Reception> CreateReceptionAsync(CreateReceptionDto createDto, int clientId)//Создание записи (от пользователя) 
        {
            //Валидация
            //Времени
            if (createDto.Time == default)
                throw new ArgumentException("Время записи обязательно");

            if (createDto.Time < DateTime.Now)
                throw new ArgumentException("Нельзя записаться на прошедшее время");

            //Проверка рабочего времени (10:00 - 20:00)
            var hour = createDto.Time.Hour;
            if (hour < 10 || hour >= 20)
                throw new ArgumentException("Запись возможна только с 10:00 до 20:00");

            //Проверка кратности времени (30 минутам)
            if (createDto.Time.Minute % 30 != 0)
                throw new ArgumentException("Запись возможна с интервалом 30 минут");

            //Проверка существования клиента            
            var client = await _context.Users.FindAsync(clientId);
            if (client == null)
                throw new InvalidOperationException("Клиент не найден");

            //Проверка существования мастера 
            var master = await _context.Masters
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MasterId == createDto.MasterId);

            if (master == null)
                throw new InvalidOperationException("Указанный мастер не существует");

            // Проверка существования услуги               
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == createDto.ServiceId);

            if (service == null)
                throw new InvalidOperationException("Указанная услуга не существует");

            // Проверка, что мастер не занят
            var existingReception = await _context.Receptions
                .FirstOrDefaultAsync(r => r.MasterId == createDto.MasterId && r.Time == createDto.Time);

            if (existingReception != null)
                throw new InvalidOperationException("Мастер уже занят в это время");                        
                        
            var reception = new Reception
            {
                Time = createDto.Time,
                MasterId = createDto.MasterId,
                ServiceId = createDto.ServiceId,
                ClientId = clientId
            };
            return reception;
        }
        public async Task<Reception> CreateReceptionByMasterAsync(CreateReceptionByMasterDto createDto, int masterId)//Создание записи (от мастера) 
        {
            //Здесь MasterId берется из контекста 
            //Если в будущем делать приложение, то нужно получить id мастера из токена авторизации

            //Валидация 
            //Времени
            if (createDto.Time == default)
                throw new ArgumentException("Время записи обязательно");

            if (createDto.Time < DateTime.Now)
                throw new ArgumentException("Нельзя записаться на прошедшее время");

            //Проверка рабочего времени (10:00 - 20:00)
            var hour = createDto.Time.Hour;
            if (hour < 10 || hour >= 20)
                throw new ArgumentException("Запись возможна только с 10:00 до 20:00");

            //Проверка кратности времени (30 минутам)
            if (createDto.Time.Minute % 30 != 0)
                throw new ArgumentException("Запись возможна с интервалом 30 минут");

            //Проверка существования мастера
            var master = await _context.Masters
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MasterId == masterId);
            if (master == null)
                throw new InvalidOperationException("Мастер не найден");

            //Проверка существования клиента
            var client = await _context.Users.FindAsync(createDto.ClientId);
            if (client == null)
                throw new InvalidOperationException("Клиент не найден");

            // Проверка существования услуги     
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == createDto.ServiceId);

            if (service == null)
                throw new InvalidOperationException("Указанная услуга не существует");

            // Проверка, что мастер не занят
            var existingReception = await _context.Receptions
                .FirstOrDefaultAsync(r => r.MasterId == masterId && r.Time == createDto.Time);

            if (existingReception != null)
                throw new InvalidOperationException("Мастер уже занят в это время");
                        
            var reception = new Reception
            {
                Time = createDto.Time,
                MasterId = masterId,
                ServiceId = createDto.ServiceId,
                ClientId = createDto.ClientId
            };

            return reception;
        }
        public Task<AdminReceptionDto> UpdateReceptionForAdminAsync(Reception existingReception, AdminUpdateReceptionForAdminDto updateDto)//Обновление записи(админ)
        {
            if (updateDto.Time.HasValue)
            {
                if (updateDto.Time.Value < DateTime.Now)
                    throw new ArgumentException("Нельзя перенести запись на прошедшее время");

                var hour = updateDto.Time.Value.Hour;
                if (hour < 10 || hour >= 20)
                    throw new ArgumentException("Запись возможна только с 10:00 до 20:00");

                if (updateDto.Time.Value.Minute % 30 != 0)
                    throw new ArgumentException("Запись возможна с интервалом 30 минут");

                existingReception.Time = updateDto.Time.Value;
            }
            if (updateDto.MasterId.HasValue)
                existingReception.MasterId = updateDto.MasterId.Value;

            if (updateDto.ServiceId.HasValue)
                existingReception.ServiceId = updateDto.ServiceId.Value;

            if (updateDto.ClientId.HasValue)
                existingReception.ClientId = updateDto.ClientId.Value;
            return Task.FromResult(MapToAdminReceptionDto(existingReception));
        }
        public Task<ReceptionDto> UpdateReceptionAsync(Reception existingReception, UpdateReceptionDto updateDto)//Обновление записи(публичный)
        {
            if (updateDto.Time.HasValue)
            {
                if (updateDto.Time.Value < DateTime.Now)
                    throw new ArgumentException("Нельзя перенести запись на прошедшее время");

                var hour = updateDto.Time.Value.Hour;
                if (hour < 10 || hour >= 20)
                    throw new ArgumentException("Запись возможна только с 10:00 до 20:00");

                if (updateDto.Time.Value.Minute % 30 != 0)
                    throw new ArgumentException("Запись возможна с интервалом 30 минут");

                existingReception.Time = updateDto.Time.Value;
            }

            return Task.FromResult(MapToReceptionDto(existingReception));
        }
        public Task<bool> CancelReceptionAsync(Reception reception)//Отмена записи(валидация) 
        {
            if (reception == null)
                throw new ArgumentNullException(nameof(reception));

            //Проверка что запись не в прошлом
            if (reception.Time < DateTime.Now)
                throw new InvalidOperationException("Нельзя отменить прошедшую запись");

            //Проверка что до записи больше часа
            if (reception.Time < DateTime.Now.AddHours(1))
                throw new InvalidOperationException("Отмена возможна не позднее чем за час до записи");
            return Task.FromResult(true);
        }
        public ReceptionDto MapToReceptionDto(Reception reception)//Маппинг Reception -> ReceptionDto (публичный)
        {
            if (reception == null) return null;

            return new ReceptionDto
            {
                Id = reception.Id,
                Time = reception.Time,
                Master = _masterService.MapToMasterDto(reception.Master),
                Service = _serviceService.MapToServiceDto(reception.Service),
                DisplayInfo = $"{reception.Id}|{reception.Time:dd.MM.yyyy HH:mm} [Master: {reception.Master?.FirstName} {reception.Master?.LastName}, Service: {reception.Service?.Name}]"
            };
        }
        public AdminReceptionDto MapToAdminReceptionDto(Reception reception)// Админский маппинг Reception -> AdminReceptionDto
        {
            if (reception == null) return null;            

            return new AdminReceptionDto
            {
                Id = reception.Id,
                Time = reception.Time,
                Master = _masterService.MapToMasterAdminDto(reception.Master),
                Service = _serviceService.MapToServiceDto(reception.Service),                
                Client = _userService.MapToAdminUserDto(reception.Client),
                DisplayInfo = $"{reception.Id}|{reception.Time:dd.MM.yyyy HH:mm} [Master: {reception.Master?.FirstName} {reception.Master?.LastName}, Service: {reception.Service?.Name}]",
                ClientDisplayInfo = reception.Client != null
                    ? $"Клиент: {reception.Client.Username} (ID: {reception.Client.UserId})"
                    : "Запись создана мастером"
            };
        }
        public string GetServiceInfo()//Инфа о сервисе
        {
            return $"ReceptionService is running. Processed by: {_configuration["AppSettings:AppName"]}";
        }
    }
}