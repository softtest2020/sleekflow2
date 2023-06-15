using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.Entity;

namespace Ticket.WebApi.Models
{
    public class TicketRegisterResponseModel
    {
        public string TicketSummary { get; set; }
        public string TicketDescription { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }
        //public List<AttachmentEntity> Attachments { get; set; }
        public IFormFileCollection Attachments { get; set; } = new FormFileCollection();

        public TicketRegisterResponseModel(string ticketSummary, string ticketDesc, string ticketPrior, string ticketStat, string userId, DateTime duedate)
        {
            TicketSummary = ticketSummary;
            TicketDescription = ticketDesc;
            TicketPriority = ticketPrior;
            TicketStatus = ticketStat;
            DueDate = duedate;
            UserId = userId;
        }
    }
}
