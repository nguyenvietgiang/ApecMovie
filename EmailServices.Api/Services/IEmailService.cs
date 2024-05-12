namespace EmailServices.Api.Services
{
    public interface IEmailService
    {
        void SendEmail(string toAddress, string subject, string body);

        void VerifyEmail(string toAddress, string subject, string ticketID, string token);
    }
}
