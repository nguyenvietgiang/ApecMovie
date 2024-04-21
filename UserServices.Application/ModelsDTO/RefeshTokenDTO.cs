namespace UserServices.Application.ModelsDTO
{
    public class RefeshTokenDTO
    {
        public string RefeshToken { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
