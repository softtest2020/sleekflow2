using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ticket.WebApi.Models
{
    public class IdModel
    {
        [Required]
        public String TicketId { get; set; }
    }
}
