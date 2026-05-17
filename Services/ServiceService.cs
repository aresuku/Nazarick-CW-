using HairSalon.Data;
using HairSalon.DTOs;
using Microsoft.EntityFrameworkCore;
using HairSalon.Models;
namespace HairSalon.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetFormattedServicesAsync(IEnumerable<Service> services);//получение всех услуг(форматировано)
        public Task<IEnumerable<ServiceDto>> GetFormattedServicesForAdminAsync(IEnumerable<Service> services);//получение всех услуг(форматировано)(админ)
        Task<ServiceDto> GetServiceByIdAsync(Service service);//получение услуги по id
        Task<Service> CreateServiceAsync(CreateServiceDto createDto);//создание услуги
        Task<ServiceDto> UpdateServiceAsync(Service existingService, UpdateServiceDto updateDto);//обновление услуги
        Task<bool> DeleteServiceAsync(Service service);//удаление услуги
        public ServiceDto MapToServiceDto(Service service);//маппинга Service -> ServiceDto
        string GetServiceInfo();//инфа о сервисе
    }    
    public class ServiceService : IServiceService
    {
        private readonly IConfiguration _configuration;
        private readonly HairSalonContext _context;
        public ServiceService(IConfiguration configuration, HairSalonContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public Task<IEnumerable<ServiceDto>> GetFormattedServicesAsync(IEnumerable<Service> services)//получение всех услуг (с форматированием)
        {
            var MaxItems = _configuration.GetValue<int>("AppSettings:MaxItems");
            var formattedServices = services
                .OrderBy(s => s.ServiceId)
                .Take(MaxItems)
                .Select(s => MapToServiceDto(s));
            return Task.FromResult(formattedServices);
        }
        public Task<IEnumerable<ServiceDto>> GetFormattedServicesForAdminAsync(IEnumerable<Service> services)//получение всех услуг (с форматированием)(админ)
        {
            var formattedServices = services
                .OrderBy(s => s.ServiceId)                
                .Select(s => MapToServiceDto(s));
            return Task.FromResult(formattedServices);
        }
        public Task<ServiceDto> GetServiceByIdAsync(Service service)//получение одной услуги по объекту
        {
            if (service == null)
                return Task.FromResult<ServiceDto>(null);
            return Task.FromResult(MapToServiceDto(service));
        }
        public async Task<Service> CreateServiceAsync(CreateServiceDto createDto)//создание услуги
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(createDto.Name))
                throw new ArgumentException("Название услуги обязательно");

            if (createDto.Price <= 0)
                throw new ArgumentException("Цена должна быть больше 0");

            // Проверка уникальности названия
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.Name == createDto.Name);

            if (existingService != null)
                throw new InvalidOperationException("Услуга с таким названием уже существует");

            // Создание услуги
            var service = new Service
            {
                Name = createDto.Name,
                Price = createDto.Price,
                Description = createDto.Description
            };

            return service;
        }
        public Task<ServiceDto> UpdateServiceAsync(Service existingService, UpdateServiceDto updateDto)//обновление информации об услуге
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                existingService.Name = updateDto.Name;

            if (updateDto.Price.HasValue)
            {
                if (updateDto.Price.Value <= 0)
                    throw new ArgumentException("Цена должна быть больше 0");
                existingService.Price = updateDto.Price.Value;
            }
            
            if (updateDto.Description != null)
                existingService.Description = updateDto.Description;

            return Task.FromResult(MapToServiceDto(existingService));
        }
        public async Task<bool> DeleteServiceAsync(Service service)//удаление услуги
        {
            if (service == null)
                return false;            
            var hasReceptions = await _context.Receptions
                .AnyAsync(r => r.ServiceId == service.ServiceId);

            if (hasReceptions)
                throw new InvalidOperationException("Нельзя удалить услугу, так как есть записи, связанные с ней");

            return true;
        }
        public string GetServiceInfo()//инфа о сервисе
        {
            return $"ServiceService is running. Processed by: {_configuration["AppSettings:AppName"]}";
        }
        public ServiceDto MapToServiceDto(Service service)//маппинга Service -> ServiceDto
        {
            if (service == null) return null;
            return new ServiceDto
            {
                Id = service.ServiceId,
                Name = service.Name,
                Price = service.Price,
                Description = service.Description,
                DisplayInfo = $"{service.ServiceId}|{service.Name} [{service.Price}₽], {service.Description}"
            };
        }        
    }    
}
