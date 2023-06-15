namespace Ticket.WebApi.Utility
{
    public class TicketWebApiOptions
    {
        public int CountLimit { get; set; } = 1000;
        public int DaysLimit { get; set; } = 30;
    }
}
