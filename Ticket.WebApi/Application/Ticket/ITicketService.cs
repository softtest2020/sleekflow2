using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.Dto;
using Ticket.WebApi.Entity;
using Xtremax.Base;

namespace Ticket.WebApi.Application.Ticket
{
    public interface ITicketServices : IApplicationService
    {
        IQueryable<TicketEntity> Tickets { get; }
        IQueryable<AttachmentEntity> Attachments { get; }
        void Create(TicketDto ticketDto);
        TicketDto Get(string ticketId);
        void Update(TicketDto ticketDto);
        void Delete(string ticketId);
        //List<TicketDto> GetAll(string TicketId, string Summary, string Priority, string Status, string UserId, DateTime CreatedDT, int Page, int PageSize);
        //IQueryable<TicketEntity> GetAll(Func<IQueryable<TicketEntity>, IQueryable<TicketEntity>> setupQuery = null);

        IEnumerable<AttachmentDto> GetAttachment(Guid attachmentId);

        AttachmentDto GetAttachments(Guid attachmentId);

        public IQueryable<TicketEntity> GetAll(Func<IQueryable<TicketEntity>, IQueryable<TicketEntity>> setupQuery = null)
        {
            return GetAll(setupQuery);
        }
    }
}
