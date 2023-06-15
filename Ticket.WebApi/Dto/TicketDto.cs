using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.Entity;
using Xtremax.Base.Dto;

namespace Ticket.WebApi.Dto
{
    public class TicketDto : EntityDto
    {
        private TicketEntity ticket;

        public TicketDto()
        {

        }

        public TicketDto(TicketEntity ticket)
        {
            this.TicketId = ticket.TicketId;
            this.TicketSummary = ticket.TicketSummary;
            this.TicketDescription = ticket.TicketDescription;
            this.TicketPriority = ticket.TicketPriority;
            this.TicketStatus = ticket.TicketStatus;
            this.UserId = ticket.UserId;
            this.Id = ticket.Id;
            this.CreatedDate = ticket.CreatedDate;
            this.AttachmentDtos = ticket.Attachments?.Select(a => new AttachmentDto(a)) ?? Enumerable.Empty<AttachmentDto>();
            this.DueDate = ticket.DueDate;
        }

        public string TicketId { get; set; }
        public string TicketSummary { get; set; }
        public string TicketDescription { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        //public List<AttachmentDto> AttachmentDtos { get; set; }
        public IEnumerable<AttachmentDto> AttachmentDtos { get; set; }
    }
}
