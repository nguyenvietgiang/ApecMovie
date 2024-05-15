namespace ApecMoviePortal.Services.TicketServices
{
    public interface ITicketService
    {
        Task<bool> ConfirmTicketAsync(Guid ticketId, string token);
    }
}
