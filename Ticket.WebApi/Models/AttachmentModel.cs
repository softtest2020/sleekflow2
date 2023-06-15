using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ticket.WebApi.Models
{
    public class AttachmentModel
    {
        public Guid? AttachmentID { get; set; }
        
        public string FileName { get; set; }

        public string MimeType { get; set; }

        //public string BinaryData { get; set; }
    }
}
