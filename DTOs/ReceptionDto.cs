namespace HairSalon.DTOs
{
    public class ReceptionDto//для всех пользователей
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public MasterDto? Master { get; set; }
        public ServiceDto? Service { get; set; }
        public string DisplayInfo { get; set; }
    }
    public class AdminReceptionDto//Админский
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public AdminMasterDto? Master { get; set; }
        public ServiceDto? Service { get; set; }
        public AdminUserDto? Client { get; set; }  //Клиент, который записался
        public string DisplayInfo { get; set; }
        public string ClientDisplayInfo { get; set; }
    }
    public class CreateReceptionDto//DTO для создания записи
    {
        public DateTime Time { get; set; }
        public int MasterId { get; set; }
        public int ServiceId { get; set; }
    }
    public class CreateReceptionByMasterDto//DTO для создания записи от лица мастера
    {
        public DateTime Time { get; set; }
        public int ClientId { get; set; }  
        public int ServiceId { get; set; }
    }
    public class AdminUpdateReceptionForAdminDto// DTO для обновления записи (админ)
    {
        public DateTime? Time { get; set; }
        public int? MasterId { get; set; }
        public int? ServiceId { get; set; }
        public int? ClientId { get; set; }
    }
    public class UpdateReceptionDto// DTO для обновления записи (публичный)
    {
        public DateTime? Time { get; set; }
    }

}