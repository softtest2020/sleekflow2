using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xtremax.Base.Entity;

namespace Ticket.WebApi.Entity
{
    public class TicketEntity : AuditableEntity
    {
        public TicketEntity()
        {
        }

        public TicketEntity(TicketEntity ticket)
        {
            Ticket = ticket;
        }

        [Index(IsUnique = true), Required, MaxLength(50)]
        public string TicketId { get; set; }

        [Required, MaxLength(255)]
        public string TicketSummary { get; set; }

        [Required]
        public string TicketDescription { get; set; }

        [Required, MaxLength(50)]
        public string TicketPriority { get; set; }

        [Required, MaxLength(50)]
        public string TicketStatus { get; set; }

        [Required, MaxLength(50)]
        public DateTime DueDate { get; set; }

        [Required, MaxLength(255)]
        public string UserId { get; set; }

        public override string AuditValue => $"Audit: {TicketId}, Summary {TicketSummary}, Description {TicketDescription}," +
            $"Priority {TicketPriority}, Status {TicketStatus}, User ID {UserId}";
        
        //public List<AttachmentEntity> Attachments { get; set; }

        [JsonIgnore]
        public IEnumerable<AttachmentEntity> Attachments { get; set; }

        public TicketEntity Ticket { get; }
    }
}
