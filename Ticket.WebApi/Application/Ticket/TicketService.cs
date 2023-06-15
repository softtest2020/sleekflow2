using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ticket.WebApi.Dto;
using Ticket.WebApi.Entity;
using Ticket.WebApi.Models;
using Xtremax.Base.Core.Models;
using Xtremax.Base.EntityFramework;

namespace Ticket.WebApi.Application.Ticket
{
    public class TicketServices : ITicketServices
    {
        private readonly IAppRepository<TicketEntity> _ticketRepository;
        private readonly IAppRepository<AttachmentEntity> _attachmentRepository;
        private readonly ILogger<TicketServices> _logger;

        public IQueryable<TicketEntity> Tickets => _ticketRepository.GetAll();

        public IQueryable<AttachmentEntity> Attachments => _attachmentRepository.GetAll();

        public TicketServices(IAppRepository<TicketEntity> ticketRepository, IAppRepository<AttachmentEntity> attachmentRepository, ILogger<TicketServices> logger)
        {
            _ticketRepository = ticketRepository;
            _attachmentRepository = attachmentRepository;
            _logger = logger;
        }

        public IQueryable<TicketEntity> GetAll(Func<IQueryable<TicketEntity>, IQueryable<TicketEntity>> setupQuery = null)
        {
            return setupQuery != null
                ? setupQuery(_ticketRepository.GetAll())
                : _ticketRepository.GetAll();
        }

        public void Create(TicketDto ticketDto)
        {
            var id = Guid.NewGuid();
            var ticId = createTicketId();
            _ticketRepository.Insert(new TicketEntity()
            {
                Id = id,
                TicketId = ticId,
                TicketSummary = ticketDto.TicketSummary,
                TicketDescription = ticketDto.TicketDescription,
                TicketPriority = ticketDto.TicketPriority,
                TicketStatus = ticketDto.TicketStatus,
                UserId = ticketDto.UserId,
            });
            _ticketRepository.SaveChanges();


            if (ticketDto.AttachmentDtos != null)
            {
                foreach (var att in ticketDto.AttachmentDtos)
                {
                    _attachmentRepository.Insert(new AttachmentEntity()
                    {
                        FileName = att.FileName,
                        BinaryData = att.BinaryData,
                        MimeType = att.MimeType,
                        TicketId = id,
                    });
                }
                _attachmentRepository.SaveChanges();
            }
            _logger.LogDebug($"Ticket registered with ID {ticketDto.TicketId}");
        }

        public TicketDto Get(String TicketId)
        {
            var tickets = _ticketRepository.GetAll().Include(a => a.Attachments).Where(x => x.TicketId == TicketId);

            var result = tickets.Select(ticket => new TicketDto
            {
                Id = ticket.Id,
                TicketId = ticket.TicketId,
                TicketSummary = ticket.TicketSummary,
                TicketDescription = ticket.TicketDescription,
                TicketPriority = ticket.TicketPriority,
                TicketStatus = ticket.TicketStatus,
                UserId = ticket.UserId,
                CreatedBy = ticket.CreatorUserName,
                CreatedDate = ticket.CreatedDate,
                ModifiedBy = ticket.LastModifierUserName,
                ModifiedDate = ticket.ModifiedDate,
                //AttachmentDtos = ticket.Attachments.Select(Attachments => new AttachmentDto
                //{
                //   Id = Attachments.Id,
                //   FileName = Attachments.FileName,
                //   MimeType = Attachments.MimeType,
                //   BinaryData = Attachments.BinaryData
                //}).ToList()
            });
            return result.FirstOrDefault();
        }

        public IEnumerable<AttachmentDto> GetAttachment(Guid ticketId)
        {
            var attachmentEntities = _attachmentRepository.GetAll().Where(attachment => attachment.TicketId.Equals(ticketId));

            var attachmentDtos = attachmentEntities.Select(attachment => new AttachmentDto
            {
                FileName = attachment.FileName,
                MimeType = attachment.MimeType,
                BinaryData = attachment.BinaryData,
                Id = attachment.Id,
                TicketId = attachment.TicketId
            });

            return attachmentDtos;
        }

        public void Update(TicketDto ticketDto)
        {
            var ticket = _ticketRepository.GetAll().Include(a => a.Attachments).Where(x => x.TicketId == ticketDto.TicketId).SingleOrDefault();
            if (ticket == null)
            {
                throw new Exception($"Ticket with ID: {ticketDto.TicketId} not found");
            }
            ticket.TicketSummary = ticketDto.TicketSummary;
            ticket.TicketDescription = ticketDto.TicketDescription;
            ticket.TicketPriority = ticketDto.TicketPriority;
            ticket.TicketStatus = ticketDto.TicketStatus;
            ticket.UserId = ticketDto.UserId;

            _ticketRepository.Update(ticket);
            _ticketRepository.SaveChanges();

            var attachment = new List<AttachmentDto>();
            var new_attachment = new List<AttachmentDto>();

            if (ticketDto.AttachmentDtos != null)
            {
                var del_attachment = ticket.Attachments.Where(x => !attachment.Exists(y => y.Id == x.Id)).ToList();

                foreach (var del_att in del_attachment)
                {
                    _attachmentRepository.Delete(del_att);
                }

                foreach (var att in ticketDto.AttachmentDtos)
                {
                    if (att.Id != default(Guid))
                    {
                        attachment.Add(att);
                    }
                    else
                    {
                        new_attachment.Add(att);
                    }
                }

                foreach (var att1 in new_attachment)
                {
                    _attachmentRepository.Insert(new AttachmentEntity()
                    {
                        FileName = att1.FileName,
                        BinaryData = att1.BinaryData,
                        MimeType = att1.MimeType,
                        TicketId = ticket.Id
                    });
                }

                _attachmentRepository.SaveChanges();
            }
        }

        public void Delete(String TicketId)
        {
            var ticket = _ticketRepository.GetAll().Include(a => a.Attachments).Where(x => x.TicketId == TicketId).Single();
            
            if (ticket == null)
            {
                throw new Exception($"Ticket with ID: {TicketId} not found");
            }

            foreach(var att in ticket.Attachments)
            {
                _attachmentRepository.Delete(att);
                _attachmentRepository.SaveChanges();
            }
            
            _ticketRepository.Delete(ticket);
            _ticketRepository.SaveChanges();
        }

        //ticketID method
        private string createTicketId()
        {
            //XT-0000
            var preLetter = "XT-";
            string lastId = _ticketRepository.GetAll().OrderByDescending(b => b.TicketId).Select(a => a.TicketId).FirstOrDefault();
            int newNum = 0;
            
            if (lastId == null)
            {
                newNum = 1;
            }
            else
            {
                var last4no = lastId.Substring(lastId.Length - 4);
                Int32.TryParse(last4no, out newNum);
                newNum++;
            }

            var ticNum = "" + newNum;

            while (ticNum.Length < 4)
            {
                ticNum = "0" + ticNum;
            }

            return preLetter + ticNum;
        }

        public AttachmentDto GetAttachments(Guid attachmentId)
        {
            AttachmentEntity entity = _attachmentRepository.Get(attachmentId);
            if (entity == null)
            {
                throw new NullReferenceException("Unable to get Attachment");
            }
            return new AttachmentDto(entity);
        }
    }
}
