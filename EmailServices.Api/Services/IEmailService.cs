namespace EmailServices.Api.Services
{
    public interface IEmailService
    {
        void SendEmail(string mail, string bodyString);
    }
}
