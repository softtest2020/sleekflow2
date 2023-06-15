using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ticket.WebApi.Dto;
using Xtremax.Base.Entity;

namespace Ticket.WebApi.Entity
{
    public class AttachmentEntity : AuditableEntity
    {
        public string FileName { get; set; }

        public string MimeType { get; set; }

        public byte[] BinaryData { get; set; }

        [JsonIgnore]
        public Guid TicketId { get; set; }
        
        [JsonIgnore]
        public TicketEntity Ticket { get; set; }

        public override string AuditValue => $"Audit: FileName {FileName}, MimeType {MimeType}";

        public AttachmentEntity(AttachmentDto attachmentDto)
        {
            FileName = attachmentDto.FileName;
            MimeType = attachmentDto.MimeType;
            BinaryData = attachmentDto.BinaryData;
            TicketId = attachmentDto.TicketId;
        }

        public AttachmentEntity()
        {
        }
    }
}
