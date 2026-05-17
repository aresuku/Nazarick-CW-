using Microsoft.AspNetCore.Mvc;
using HairSalon.Data;
using Microsoft.EntityFrameworkCore;
using HairSalon.Services;
using HairSalon.DTOs;

namespace HairSalon.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly HairSalonContext _context;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersApiController(
            HairSalonContext context,
            IUserService userService,
            IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()//GET: api/users - Получить всех пользователей
        {
            var usersFromDb = await _context.Users.ToListAsync();
            var formattedUsers = await _userService.GetFormattedUsersAsync(usersFromDb);
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _userService.GetServiceInfo(),
                Data = formattedUsers
            };
            return Ok(response);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetUsersForAdmin()//GET: api/users/admin - Получить всех пользователей (админ)
        {
            var usersFromDb = await _context.Users.ToListAsync();
            var formattedUsers = await _userService.GetFormattedUsersForAdminAsync(usersFromDb);

            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _userService.GetServiceInfo(),
                Data = formattedUsers
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)//GET: api/users/{id} - Получить пользователя по id
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }
            var userDto = await _userService.GetUserByIdAsync(user);
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _userService.GetServiceInfo(),
                Data = userDto
            };
            return Ok(response);
        }

        [HttpGet("{id}/admin")]
        public async Task<IActionResult> GetUserByIdForAdmin(int id)//GET: api/users/{id}/admin - Получить пользователя по id (админ)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }
            var userAdminDto = await _userService.GetUserByIdForAdminAsync(user);
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _userService.GetServiceInfo(),
                Data = userAdminDto
            };
            return Ok(response);
        }

        [HttpGet("by-login/{login}")]
        public async Task<IActionResult> GetUserByLogin(string login)//GET: api/users/by-login/{login} - Получить пользователя по логину
        {
            var userDto = await _userService.GetUserByLoginAsync(login);
            if (userDto == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }
            var response = new
            {
                AppVersion = _configuration["AppSettings:Version"],
                ServiceInfo = _userService.GetServiceInfo(),
                Data = userDto
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)//POST: api/users - Создать нового пользователя
        {
            try
            {
                var newUser = await _userService.CreateUserAsync(createUserDto);

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var createdUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == newUser.UserId);

                var userDto = await _userService.GetUserByIdAsync(createdUser);

                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, new
                {
                    message = "Пользователь успешно создан",
                    user = userDto
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
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)//PUT: api/users/{id} - Обновить пользователя
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }
                await _userService.UpdateUserAsync(user, updateUserDto);
                await _context.SaveChangesAsync();
                var updatedUser = await _context.Users.FindAsync(id);
                var userDto = await _userService.GetUserByIdAsync(updatedUser);
                return Ok(new
                {
                    message = "Пользователь успешно обновлен",
                    user = userDto
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

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)// POST: api/users/{id}/change-password - Сменить пароль
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }
                await _userService.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Пароль успешно изменен"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)//POST: api/users/login - Вход в систему
        {
            try
            {
                var user = await _userService.AuthenticateAsync(loginDto.Login, loginDto.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Неверный логин или пароль" });
                }
                var userDto = await _userService.GetUserByIdAsync(user);
                return Ok(new LoginResponseDto
                {
                    Success = true,
                    Message = "Вход выполнен успешно",
                    User = userDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)//DELETE: api/users/{id} - Удалить пользователя
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }
                var userName = user.Login;
                await _userService.DeleteUserAsync(user);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Пользователь успешно удален",
                    deletedUserId = id,
                    deletedUserLogin = userName
                });
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
    }
}