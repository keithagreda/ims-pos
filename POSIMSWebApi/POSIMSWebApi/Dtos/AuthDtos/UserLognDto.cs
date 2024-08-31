namespace POSIMSWebApi.Dtos.AuthDtos
{
    public class UserLognDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserToken { get; set; }
        public string NewRefreshToken { get; set; }
        public List<string> UserRole { get; set; }
    }
}
