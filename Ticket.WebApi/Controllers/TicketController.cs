using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Ticket.WebApi.Application.Ticket;
using Ticket.WebApi.Dto;
using Ticket.WebApi.Models;
using Xtremax.Base.Core.Application;
using Xtremax.Base.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Ticket.WebApi.Entity;
using System.Data.Entity;
using Xtremax.Base.Core.Attributes;
using Xtremax.Base.Extension;
using System.Linq;
using Xtremax.Base.Application.Authorization;
using Ticket.WebApi.Utility.Constants;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace xticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketController : HostController
    {
        private readonly ITicketServices _ticketServices;
        private ILogger<TicketController> _logger;
        protected override ILogger Logger => _logger ?? (_logger = HttpContext.RequestServices.GetService<ILogger<TicketController>>());


        public TicketController(ITicketServices ticketServices)
        {
            _ticketServices = ticketServices;
        }

        [HttpPost]
        [Route("filter")]
        [ProducesResponseType(typeof(BaseListResponseModel<List<TicketDto>>), (int)HttpStatusCode.OK)]
        [BaseActionFilter]
        [BaseExceptionFilter]
        [RequirePermission(TicketConstant.Permission.READ)]
        public BaseListResponseModel<List<TicketDto>> Index([FromBody] TicketIndexRequestModel requestModel)
        {
            Func<IQueryable<TicketEntity>, IQueryable<TicketEntity>> ticFunc = tickets =>
            {
                if (!string.IsNullOrEmpty(requestModel.QuickSearch))
                {
                    var quickSearch = '%' + requestModel.QuickSearch + '%';
                    tickets = tickets.Where(ticket =>
                        EF.Functions.Like(ticket.TicketSummary, quickSearch));
                }

                if (!string.IsNullOrEmpty(requestModel.TicketId))
                {
                    tickets = tickets.Where(ticket => ticket.TicketId.Contains(requestModel.TicketId));
                }

                if (!string.IsNullOrWhiteSpace(requestModel.Summary))
                {
                    tickets = tickets.Where(ticket => ticket.TicketSummary.Contains(requestModel.Summary));
                }

                if (!string.IsNullOrWhiteSpace(requestModel.Priority))
                {
                    string[] prior = requestModel.Priority.Split(",");
                    int i = prior.Length - 1;
                    if (i == 3)
                    {
                        tickets = tickets.Where(ticket => ticket.TicketPriority.Contains(prior[0]) ||
                        ticket.TicketPriority.Contains(prior[1]) ||
                        ticket.TicketPriority.Contains(prior[2]) ||
                        ticket.TicketPriority.Contains(prior[3]));
                    }
                    else if (i == 2)
                    {
                        tickets = tickets.Where(ticket => ticket.TicketPriority.Contains(prior[0]) ||
                        ticket.TicketPriority.Contains(prior[1]) ||
                        ticket.TicketPriority.Contains(prior[2]));
                    }
                    else if (i == 1)
                    {
                        tickets = tickets.Where(ticket => ticket.TicketPriority.Contains(prior[0]) ||
                        ticket.TicketPriority.Contains(prior[1]));
                    }
                    else
                    {
                        tickets = tickets.Where(ticket => ticket.TicketPriority.Contains(prior[0]));
                    }
                }
                if (!string.IsNullOrWhiteSpace(requestModel.Status))
                {
                    string[] stat = requestModel.Status.Split(",");
                    int i = stat.Length - 1;
                    if (i == 2)
                    {
                        tickets = tickets.Where(ticket => ticket.TicketStatus.Contains(stat[0]) ||
                        ticket.TicketStatus.Contains(stat[1]) ||
                        ticket.TicketStatus.Contains(stat[2]));
                    }
                    else if (i == 1)
                    {
                        tickets = tickets.Where(ticket => ticket.TicketStatus.Contains(stat[0]) ||
                        ticket.TicketStatus.Contains(stat[1]));
                    }
                    else
                    {
                        tickets = tickets.Where(ticket => ticket.TicketStatus.Contains(stat[0]));
                    }
                }
                if (!string.IsNullOrWhiteSpace(requestModel.Assignee))
                {
                    tickets = tickets.Where(ticket => ticket.UserId.Contains(requestModel.Assignee));
                }
                if (requestModel.CreatedStartDateTime.HasValue)
                {
                    tickets = tickets.Where(ticket => ticket.CreatedDate >= requestModel.CreatedStartDateTime.Value);
                }
                if (requestModel.CreatedEndDateTime.HasValue)
                {
                    tickets = tickets.Where(ticket => ticket.CreatedDate <= requestModel.CreatedEndDateTime.Value);
                }

                var isAscendingSort = requestModel.SortAscFlag ?? false;

                if (!string.IsNullOrWhiteSpace(requestModel.Sort))
                {
                    tickets = isAscendingSort
                    ? tickets.OrderBy(requestModel.Sort)
                    : tickets.OrderByDescending(requestModel.Sort);
                }

                return tickets;
            };


            var ticketQueryable = _ticketServices.GetAll(ticFunc);
            var results = ticketQueryable
                .Skip((int)((requestModel.Page - 1) * requestModel.PageSize))
                .Take((int)requestModel.PageSize)
                .Select(ticket => new TicketDto(ticket))
                .ToList();
            var totalRows = ticketQueryable.LongCount();

            return new BaseListResponseModel<List<TicketDto>>(results, totalRows);
        }

        [Route("details")]
        [HttpPost]
        [RequirePermission(TicketConstant.Permission.READ)]
        public BaseResponseModel<TicketDetailsResponseModel> Read([FromBody] IdModel requestModel)
        {
            TicketDto tDto = _ticketServices.Get(requestModel.TicketId);
            var cust = new TicketDetailsResponseModel(tDto);

            var attachments = _ticketServices.GetAttachment(cust.Id);
            cust.Attachments = new List<string>();
            foreach (AttachmentDto attachment in attachments)
            {
                cust.Attachments.Add(Url.Action("downloadattachment", "Ticket", new { id = attachment.Id }));
            }
            return new BaseResponseModel<TicketDetailsResponseModel>(cust, (int)HttpStatusCode.OK);
        }

        [Route("create")]
        [HttpPost]
        [RequirePermission(TicketConstant.Permission.CREATE)]
        public BaseResponseModel<TicketRegisterRequestModel> Create([FromForm] TicketRegisterRequestModel requestModel)
        {
            TicketDto ticketDto = new TicketDto
            {
                TicketSummary = requestModel.TicketSummary,
                TicketDescription = requestModel.TicketDescription,
                TicketPriority = requestModel.TicketPriority,
                TicketStatus = requestModel.TicketStatus,
                UserId = requestModel.UserId,
                DueDate = requestModel.DueDate,
            };

            if (requestModel.attachments.Count > 0)
            {
                ticketDto.AttachmentDtos = requestModel.attachments.Select(formFile => new AttachmentDto
                {
                    FileName = formFile.FileName,
                    MimeType = formFile.ContentType,
                    BinaryData = _StreamToByte(formFile.OpenReadStream())
                });
                ticketDto.AttachmentDtos = ticketDto.AttachmentDtos;
            }
            
            _ticketServices.Create(ticketDto);
            return new BaseResponseModel<TicketRegisterRequestModel>(requestModel, (int)HttpStatusCode.OK);
        }


        private static byte[] _StreamToByte(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [Route("update")]
        [HttpPost]
        [RequirePermission(TicketConstant.Permission.UPDATE)]
        public BaseResponseModel<TicketUpdateRequestModel> Update([FromForm] TicketUpdateRequestModel requestModel)
        {
            TicketDto ticketDto = new TicketDto
            {
                TicketId = requestModel.TicketId,
                TicketSummary = requestModel.TicketSummary,
                TicketDescription = requestModel.TicketDescription,
                TicketPriority = requestModel.TicketPriority,
                TicketStatus = requestModel.TicketStatus,
                UserId = requestModel.UserId,
                DueDate = requestModel.DueDate,
            };

            if (requestModel.attachments.Count > 0)
            {
                ticketDto.AttachmentDtos = requestModel.attachments.Select(formFile => new AttachmentDto
                {
                    FileName = formFile.FileName,
                    MimeType = formFile.ContentType,
                    BinaryData = _StreamToByte(formFile.OpenReadStream())
                });
                ticketDto.AttachmentDtos = ticketDto.AttachmentDtos;
            }

            _ticketServices.Update(ticketDto);
            return new BaseResponseModel<TicketUpdateRequestModel>(requestModel, (int)HttpStatusCode.OK);
        }

        [Route("delete")]
        [HttpPost]
        [RequirePermission(TicketConstant.Permission.DELETE)]
        public BaseResponseModel<string> Delete([FromBody] IdModel requestModel)
        {
            _ticketServices.Delete(requestModel.TicketId);
            var message = "Item has been Deleted";
            return new BaseResponseModel<string>(message, (int)HttpStatusCode.OK);
        }

        [HttpGet]
        [BaseActionFilter]
        [BaseExceptionFilter]
        [Route("downloadattachment/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(File))]
        public IActionResult DownloadAttachment(Guid id)
        {
            var attachment = _ticketServices.GetAttachments(id);
            return File(attachment.BinaryData, attachment.MimeType, attachment.FileName);
        }
    }
}