namespace HairSalon.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Username { get; set; }        
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string DisplayInfo { get; set; }
    }
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string DisplayInfo { get; set; }
    }
    public class CreateUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "User";
    }
    public class UpdateUserDto
    {
        public string? Login { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? Password { get; set; }
    }
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }    
    public class RegisterUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }    
    public class LoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }       
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDto? User { get; set; }
        public string? Token { get; set; }//Если в будущем добавлять приложение
    }
}