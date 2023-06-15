using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.Dto;
using Ticket.WebApi.Entity;

namespace Ticket.WebApi.Models
{
    public class TicketDetailsResponseModel
    {
        public string TicketId { get; set; }
        public string TicketSummary { get; set; }
        public string TicketDescription { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        //public List<AttachmentModel> Attachments { get; set; }
        public List<string> Attachments { get; set; }
        public Guid Id { get; set; }

        public TicketDetailsResponseModel(string ticketId, string ticketSummary, string ticketDesc, string ticketPrior, string ticketStat, string userId, DateTime dueDate)
        {
            TicketId = ticketId;
            DueDate = dueDate;
            TicketSummary = ticketSummary;
            TicketDescription = ticketDesc;
            TicketPriority = ticketPrior;
            TicketStatus = ticketStat;
            UserId = userId;
        }

        public TicketDetailsResponseModel(TicketDto tDto)
        {
            Id = tDto.Id;
            TicketId = tDto.TicketId;
            TicketSummary = tDto.TicketSummary;
            TicketDescription = tDto.TicketDescription;
            TicketPriority = tDto.TicketPriority;
            TicketStatus = tDto.TicketStatus;
            UserId = tDto.UserId;
            CreatedDate = tDto.CreatedDate;
            CreatedBy = tDto.CreatedBy;
            ModifiedDate = tDto.ModifiedDate;
            ModifiedBy = tDto.ModifiedBy;
            DueDate = tDto.DueDate;
            //Attachments = tDto.AttachmentDtos.Select(attach => new AttachmentModel
            //{
            //    FileName = attach.FileName,
            //    MimeType = attach.MimeType,
            //    AttachmentID = attach.Id

            //}).ToList();
        }
    }
    
}
