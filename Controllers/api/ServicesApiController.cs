using Microsoft.AspNetCore.Mvc;
using HairSalon.Data;
using Microsoft.EntityFrameworkCore;
using HairSalon.Services;
using HairSalon.DTOs;
namespace HairSalon.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesApiController : ControllerBase
    {
        private readonly HairSalonContext _context;
        private readonly IServiceService _serviceService;
        private readonly IConfiguration _configuration;
        public ServicesApiController(HairSalonContext context,
            IServiceService serviceService,
            IConfiguration configuration)
        {
            _context = context;
            _serviceService = serviceService;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetServices()//GET: api/services - Получить все услуги
        {
            var servicesFromDb = await _context.Services.ToListAsync();
            var formattedServices = await _serviceService.GetFormattedServicesAsync(servicesFromDb);
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _serviceService.GetServiceInfo(),
                Data = formattedServices
            };
            return Ok(response);            
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)//GET: api/services/{id} - Получить услугу по ID
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) 
            { 
                return NotFound(new { message = "Услуга не найдена" });
            }
            var serviceDto = await _serviceService.GetServiceByIdAsync(service);
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _serviceService.GetServiceInfo(),
                Data = serviceDto
            };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto createServiceDto)//POST: api/services - Создать новую услугу
        {
            try
            {
                var newService = await _serviceService.CreateServiceAsync(createServiceDto);
                _context.Services.Add(newService);
                await _context.SaveChangesAsync();

                var createdService = await _context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == newService.ServiceId);
                var serviceDto = await _serviceService.GetServiceByIdAsync(createdService);
                return CreatedAtAction(nameof(GetServiceById), new { id = newService.ServiceId }, new
                {
                    message = "Услуга успешно создана",
                    service = serviceDto
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new{message = ex.Message});
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new{message = ex.Message});
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new{message="Ошибка при сохранении в базу данных", detail = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new{message="Внутренняя ошибка сервера", detail = ex.Message});
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceDto updateServiceDto)// PUT: api/services/{id} - Обновить услугу
        {
            try
            {
                var service = await _context.Services.FindAsync(id);

                if (service == null)
                {
                    return NotFound(new { message = "Услуга не найдена" });
                }
                                
                await _serviceService.UpdateServiceAsync(service, updateServiceDto);
                await _context.SaveChangesAsync();

                var updatedService = await _context.Services.FindAsync(id);
                var serviceDto = await _serviceService.GetServiceByIdAsync(updatedService);

                return Ok(new
                {
                    message = "Услуга успешно обновлена",
                    service = serviceDto
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Ошибка при сохранении в базу данных", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });
            }
        }
    }
}