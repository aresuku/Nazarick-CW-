namespace HairSalon.DTOs
{
    public class MasterDto
    {
        public int Id { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Experience { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }                
        public string DisplayInfo { get; set; }
        public string Role { get; set; }
        public UserDto User { get; set; }
        public string UserDisplayInfo { get; set; }
    }
    public class AdminMasterDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Experience { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
        public string Email { get; set; }  // Только для админов
        public string Role { get; set; }
        public string DisplayInfo { get; set; }
        public AdminUserDto User { get; set; }
        public string UserDisplayInfo { get; set; }
    }
    public class CreateMasterDto
    {        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Experience { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
        public string Email { get; set; }        
        public string Password { get; set; }  
    }
    public class CreateMasterFromUserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Experience { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateMasterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Experience { get; set; }
        public string? Gender { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public int? UserId { get; set; }  
    }
}