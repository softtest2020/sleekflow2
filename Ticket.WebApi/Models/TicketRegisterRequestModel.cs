using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ticket.WebApi.Entity;

namespace Ticket.WebApi.Models
{
    public class TicketRegisterRequestModel
    {
        [Required]
        public string TicketSummary { get; set; }

        [Required]
        public string TicketDescription { get; set; }

        [Required]
        public string TicketPriority { get; set; }

        [Required]
        public string TicketStatus { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public IFormFileCollection attachments { get; set; } = new FormFileCollection();

        //public List<AttachmentModel> attachments { get; set; }

    }
}
