using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.Entity;
using Xtremax.Base.Dto;

namespace Ticket.WebApi.Dto
{
    public class AttachmentDto : EntityDto
    {
        public Guid TicketId { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] BinaryData { get; set; }
        public TicketDto ticketDto { get; set; }
        public AttachmentDto(AttachmentEntity entity)
        {
            TicketId = entity.TicketId;
            FileName = entity.FileName;
            MimeType = entity.MimeType;
            BinaryData = entity.BinaryData;
        }

        public AttachmentDto()
        {

        }
    }
}
