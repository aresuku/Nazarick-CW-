using Microsoft.AspNetCore.Mvc;
using HairSalon.Data;
using Microsoft.EntityFrameworkCore;
using HairSalon.Services;
using HairSalon.DTOs;

namespace HairSalon.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionsApiController : ControllerBase
    {
        private readonly HairSalonContext _context;
        private readonly IReceptionService _receptionService;
        private readonly IConfiguration _configuration;

        public ReceptionsApiController(
            HairSalonContext context,
            IReceptionService receptionService,
            IConfiguration configuration)
        {
            _context = context;
            _receptionService = receptionService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetReceptions()// GET: api/receptions - Получить все записи (публичный)
        {
            var receptionsFromDb = await _context.Receptions
                .Include(r => r.Master)
                    .ThenInclude(m => m.User)
                .Include(r => r.Service)
                .ToListAsync();

            var formattedReceptions = await _receptionService.GetFormattedReceptionsAsync(receptionsFromDb);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = formattedReceptions
            };

            return Ok(response);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetReceptionsForAdmin()// GET: api/receptions/admin - Получить все записи (админ)
        {
            var receptionsFromDb = await _context.Receptions
                .Include(r => r.Master)
                    .ThenInclude(m => m.User)
                .Include(r => r.Service)
                .Include(r => r.Client)
                .ToListAsync();

            var formattedReceptions = await _receptionService.GetFormattedReceptionsForAdminAsync(receptionsFromDb);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = formattedReceptions
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceptionById(int id)// GET: api/receptions/{id} - Получить запись по ID (публичный)
        {
            var reception = await _context.Receptions
                .Include(r => r.Master)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reception == null)
            {
                return NotFound(new { message = "Запись не найдена" });
            }

            var receptionDto = await _receptionService.GetReceptionByIdAsync(reception);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = receptionDto
            };

            return Ok(response);
        }

        [HttpGet("{id}/admin")]
        public async Task<IActionResult> GetReceptionByIdForAdmin(int id)// GET: api/receptions/{id}/admin - Получить запись по ID (админ)
        {
            var reception = await _context.Receptions
                .Include(r => r.Master)
                    .ThenInclude(m => m.User)
                .Include(r => r.Service)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reception == null)
            {
                return NotFound(new { message = "Запись не найдена" });
            }

            var adminReceptionDto = await _receptionService.GetReceptionByIdForAdminAsync(reception);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = adminReceptionDto
            };

            return Ok(response);
        }

        [HttpGet("by-master/{masterId}")]
        public async Task<IActionResult> GetReceptionsByMaster(int masterId)// GET: api/receptions/by-master/{masterId} - Получить записи мастера (публичный)
        {
            var receptions = await _context.Receptions
                .Include(r => r.Master)
                .Include(r => r.Service)
                .Where(r => r.MasterId == masterId)
                .ToListAsync();

            var formattedReceptions = await _receptionService.GetReceptionsByMasterAsync(receptions);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = formattedReceptions
            };

            return Ok(response);
        }

        [HttpGet("by-master/{masterId}/admin")]
        public async Task<IActionResult> GetReceptionsByMasterForAdmin(int masterId)// GET: api/receptions/by-master/{masterId}/admin - Получить записи мастера (админ)
        {
            var receptions = await _context.Receptions
                .Include(r => r.Master)
                    .ThenInclude(m => m.User)
                .Include(r => r.Service)
                .Include(r => r.Client)
                .Where(r => r.MasterId == masterId)
                .ToListAsync();

            var formattedReceptions = await _receptionService.GetReceptionsByMasterForAdminAsync(receptions);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = formattedReceptions
            };

            return Ok(response);
        }

        [HttpGet("by-client/{clientId}")]
        public async Task<IActionResult> GetReceptionsByClient(int clientId)// GET: api/receptions/by-client/{clientId} - Получить записи клиента (публичный)
        {
            var receptions = await _context.Receptions
                .Include(r => r.Master)
                .Include(r => r.Service)
                .Where(r => r.ClientId == clientId)
                .ToListAsync();

            var formattedReceptions = await _receptionService.GetReceptionsByClientAsync(receptions);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = formattedReceptions
            };

            return Ok(response);
        }

        [HttpGet("by-client/{clientId}/admin")]
        public async Task<IActionResult> GetReceptionsByClientForAdmin(int clientId)// GET: api/receptions/by-client/{clientId}/admin - Получить записи клиента (админ)
        {
            var receptions = await _context.Receptions
                .Include(r => r.Master)
                    .ThenInclude(m => m.User)
                .Include(r => r.Service)
                .Include(r => r.Client)
                .Where(r => r.ClientId == clientId)
                .ToListAsync();

            var formattedReceptions = await _receptionService.GetReceptionsByClientForAdminAsync(receptions);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _receptionService.GetServiceInfo(),
                Data = formattedReceptions
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReception([FromBody] CreateReceptionDto createReceptionDto)// POST: api/receptions - Создать новую запись (от пользователя)
        {
            try
            {
                //При написании приложение clientId получается из токена авторизации
                //Временно используется заглушка
                var clientId = 100;

                var newReception = await _receptionService.CreateReceptionAsync(createReceptionDto, clientId);

                _context.Receptions.Add(newReception);
                await _context.SaveChangesAsync();

                var createdReception = await _context.Receptions
                    .Include(r => r.Master)
                    .Include(r => r.Service)
                    .FirstOrDefaultAsync(r => r.Id == newReception.Id);

                var receptionDto = await _receptionService.GetReceptionByIdAsync(createdReception);

                return CreatedAtAction(nameof(GetReceptionById), new { id = newReception.Id }, new
                {
                    message = "Запись успешно создана",
                    reception = receptionDto
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

        [HttpPost("by-master")]
        public async Task<IActionResult> CreateReceptionByMaster([FromBody] CreateReceptionByMasterDto createReceptionByMasterDto)// POST: api/receptions/by-master - Создать запись от мастера
        {
            try
            {
                //При написании приложение masterId получается из токена авторизации
                //Временно используется заглушка
                var masterId = 101;

                var newReception = await _receptionService.CreateReceptionByMasterAsync(createReceptionByMasterDto, masterId);

                _context.Receptions.Add(newReception);
                await _context.SaveChangesAsync();

                var createdReception = await _context.Receptions
                    .Include(r => r.Master)
                        .ThenInclude(m => m.User)
                    .Include(r => r.Service)
                    .Include(r => r.Client)
                    .FirstOrDefaultAsync(r => r.Id == newReception.Id);

                var adminReceptionDto = await _receptionService.GetReceptionByIdForAdminAsync(createdReception);

                return CreatedAtAction(nameof(GetReceptionByIdForAdmin), new { id = newReception.Id }, new
                {
                    message = "Запись успешно создана мастером",
                    reception = adminReceptionDto
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

        [HttpPut("{id}/admin")]
        public async Task<IActionResult> UpdateReceptionForAdmin(int id, [FromBody] AdminUpdateReceptionForAdminDto AdminUpdateReceptionForAdminDto)// PUT: api/receptions/{id}/admin" - Обновить запись (админ)
        {
            try
            {
                var reception = await _context.Receptions
                    .Include(r => r.Master)
                        .ThenInclude(m => m.User)
                    .Include(r => r.Service)
                    .Include(r => r.Client)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reception == null)
                {
                    return NotFound(new { message = "Запись не найдена" });
                }

                await _receptionService.UpdateReceptionForAdminAsync(reception, AdminUpdateReceptionForAdminDto);
                await _context.SaveChangesAsync();

                var updatedReception = await _context.Receptions
                    .Include(r => r.Master)
                        .ThenInclude(m => m.User)
                    .Include(r => r.Service)
                    .Include(r => r.Client)
                    .FirstOrDefaultAsync(r => r.Id == id);

                var adminReceptionDto = await _receptionService.GetReceptionByIdForAdminAsync(updatedReception);

                return Ok(new
                {
                    message = "Запись успешно обновлена",
                    reception = adminReceptionDto
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReception(int id, [FromBody] UpdateReceptionDto UpdateReceptionDto)// PUT: api/receptions/{id} - Обновить запись (публичный)
        {
            try
            {
                var reception = await _context.Receptions
                    .Include(r => r.Master)
                    .Include(r => r.Service)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reception == null)
                {
                    return NotFound(new { message = "Запись не найдена" });
                }

                await _receptionService.UpdateReceptionAsync(reception, UpdateReceptionDto);
                await _context.SaveChangesAsync();

                var updatedReception = await _context.Receptions
                    .Include(r => r.Master)
                    .Include(r => r.Service)
                    .FirstOrDefaultAsync(r => r.Id == id);

                var receptionDto = await _receptionService.GetReceptionByIdAsync(updatedReception);

                return Ok(new
                {
                    message = "Время записи успешно обновлено",
                    reception = receptionDto
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

        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelReception(int id)// DELETE: api/receptions/{id}/cancel - Отменить запись(публичный)
        {
            try
            {
                var reception = await _context.Receptions
                    .Include(r => r.Master)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reception == null)
                {
                    return NotFound(new { message = "Запись не найдена" });
                }

                await _receptionService.CancelReceptionAsync(reception);
                _context.Receptions.Remove(reception);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Запись успешно отменена",
                    cancelledReceptionId = id,
                    masterName = $"{reception.Master?.FirstName} {reception.Master?.LastName}",
                    cancelledTime = reception.Time.ToString("dd.MM.yyyy HH:mm")
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReception(int id)// DELETE: api/receptions/{id} - Удалить запись (админ)
        {
            try
            {
                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reception == null)
                {
                    return NotFound(new { message = "Запись не найдена" });
                }

                _context.Receptions.Remove(reception);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Запись успешно удалена",
                    deletedReceptionId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });
            }
        }    
    }   
}