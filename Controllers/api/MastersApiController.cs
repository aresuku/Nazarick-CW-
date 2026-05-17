using HairSalon.Data;
using HairSalon.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HairSalon.Services;
namespace HairSalon.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersApiController : ControllerBase
    {
        private readonly HairSalonContext _context;
        private readonly IMasterService _masterService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public MastersApiController(HairSalonContext context,
            IMasterService masterService,
            IConfiguration configuration,
            IUserService userService)
        {
            _context = context;
            _masterService = masterService;
            _configuration = configuration;
            _userService = userService;            
        }
        [HttpGet]
        public async Task<IActionResult> GetMasters()//GET: api/masters - Получить всех мастеров
        {
            var mastersFromDb = await _context.Masters
                .Include(m => m.User)  
                .ToListAsync();
            var formattedMasters = await _masterService.GetFormattedMastersAsync(mastersFromDb);
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _masterService.GetServiceInfo(),
                Data = formattedMasters
            };
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMasterById(int id)//GET: api/masters/{id} - Получить мастера по ID
        {
            var master = await _context.Masters
                .Include(m => m.User)  
                .FirstOrDefaultAsync(m => m.MasterId == id);
            if (master == null) 
            { 
                return NotFound(new { message = "Мастер не найден" });
            }
            var masterDto = await _masterService.GetMasterByIdAsync(master);            
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _masterService.GetServiceInfo(),
                Data = masterDto
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaster([FromBody] CreateMasterDto createMasterDto)//POST: api/masters - Создать нового мастера
        {
            try
            {
                var newMaster = await _masterService.CreateMasterWithUserAsync(createMasterDto, _userService);
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Users.Add(newMaster.User);
                    await _context.SaveChangesAsync();
                    newMaster.UserId = newMaster.User.UserId;
                    _context.Masters.Add(newMaster);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch { await transaction.RollbackAsync(); throw; }                
                // Возврат успешного ответа
                var createdMaster = await _context.Masters
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.MasterId == newMaster.MasterId);
                var masterDto = await _masterService.GetMasterByIdAsync(createdMaster);
                return CreatedAtAction(nameof(GetMasterById), new { id = newMaster.MasterId }, new
                {
                    message = "Мастер успешно создан",
                    master = masterDto
                });
            }
            catch (ArgumentException ex)
            {
                // Ошибки валидации: пустые поля, неверный пол, неверный email
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Бизнес-ошибки: дубли email или login
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Ошибка при сохранении в базу данных", detail = ex.Message });
            }
            catch (Exception ex)
            {
                // Непредвиденные ошибки
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });

            }
        }

        [HttpPost("from-user")]
        public async Task<IActionResult> CreateMasterFromUser([FromBody] CreateMasterFromUserDto createMasterFromUserDto)//POST: api/masters/from-user - Создать мастера из существующего пользователя
        {
            try
            {
                var newMaster = await _masterService.CreateMasterFromUserAsync(createMasterFromUserDto);

                _context.Masters.Add(newMaster);
                await _context.SaveChangesAsync();

                var createdMaster = await _context.Masters
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.MasterId == newMaster.MasterId);

                var masterDto = await _masterService.GetMasterByIdAsync(createdMaster);

                return CreatedAtAction(nameof(GetMasterById), new { id = newMaster.MasterId }, new
                {
                    message = "Мастер успешно создан из существующего пользователя",
                    master = masterDto
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
        public async Task<IActionResult> UpdateMaster(int id, [FromBody] UpdateMasterDto updateMasterDto)// PUT: api/masters/{id} - Обновить мастера
        {
            try
            {
                var master = await _context.Masters
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.MasterId == id);

                if (master == null)
                {
                    return NotFound(new { message = "Мастер не найден" });
                }

                var oldEmail = master.Email;
                                
                await _masterService.UpdateMasterAsync(master, updateMasterDto);

                // Если email изменился то изменение и в связанном пользователе 
                if (!string.IsNullOrWhiteSpace(updateMasterDto.Email) &&
                    updateMasterDto.Email != oldEmail &&
                    master.User != null)
                {
                    // Проверка уникальности нового email
                    var existingMaster = await _context.Masters
                        .FirstOrDefaultAsync(m => m.Email == updateMasterDto.Email && m.MasterId != id);

                    if (existingMaster != null)
                    {
                        return Conflict(new { message = "Мастер с таким email уже существует" });
                    }

                    master.User.Login = updateMasterDto.Email;
                    master.User.Email = updateMasterDto.Email;
                }

                await _context.SaveChangesAsync();

                var updatedMaster = await _context.Masters
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.MasterId == id);

                var masterDto = await _masterService.GetMasterByIdAsync(updatedMaster);

                return Ok(new
                {
                    message = "Мастер успешно обновлен",
                    master = masterDto
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaster(int id, [FromQuery] bool deleteRelatedUser = true)// DELETE: api/masters/{id} - Удалить мастера
        {
            try
            {
                var master = await _context.Masters
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.MasterId == id);

                if (master == null)
                {
                    return NotFound(new { message = "Мастер не найден" });
                }

                var masterName = $"{master.FirstName} {master.LastName}";
                var hadUser = master.User != null;
                var userLogin = master.User?.Login;

                await _masterService.DeleteMasterAsync(master, deleteRelatedUser);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Мастер успешно удален",
                    deletedMasterId = id,
                    deletedMasterName = masterName,
                    userDeleted = deleteRelatedUser && hadUser,
                    deletedUserLogin = deleteRelatedUser && hadUser ? userLogin : null
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Ошибка при удалении из базы данных", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });
            }
        }
    }
}