using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xtremax.Base.Core.Models;
using Ticket.WebApi.Utility;

namespace Ticket.WebApi.Models
{
    public class TicketIndexRequestModel : BaseFilterRequestModel
    {
        public string TicketId { get; set; }
        public string Summary { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Assignee { get; set; }
        public DateTime? CreatedStartDateTime { get; set; }
        public DateTime? CreatedEndDateTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var ticketWebApiOptions = validationContext.GetService(typeof(TicketWebApiOptions)) as TicketWebApiOptions;

            if (ticketWebApiOptions == null)
            {
                throw new System.Exception("Cannot retrieve auditTrailWebApiOptions implementation");
            }

            //sort mapping for createdate
            if (string.Compare(Sort, "createdDateTime") == 0)
            {
                Sort = "createdDate";
            }

            //Date Validation
            if (CreatedStartDateTime != null && CreatedEndDateTime == null)
            {
                DateTime createdStartDateTime = (DateTime)CreatedStartDateTime;
                CreatedEndDateTime = createdStartDateTime.AddDays(ticketWebApiOptions.DaysLimit);
            }
            else if (CreatedStartDateTime == null && CreatedEndDateTime != null)
            {
                DateTime createdEndDateTime = (DateTime)CreatedEndDateTime;
                CreatedStartDateTime = createdEndDateTime.AddDays(ticketWebApiOptions.DaysLimit * -1);
            }
            else if (CreatedEndDateTime == null && CreatedStartDateTime == null)
            {
                CreatedStartDateTime = DateTime.Now.AddDays(ticketWebApiOptions.DaysLimit * -1);
                CreatedEndDateTime = DateTime.Now;
            }
            else
            {
                DateTime createdStartDateTime = (DateTime)CreatedStartDateTime;
                DateTime createdEndDateTime = (DateTime)CreatedEndDateTime;
                var dateRangeValidation = (createdEndDateTime - createdStartDateTime).TotalDays < ticketWebApiOptions.DaysLimit ? true : false;

                if (dateRangeValidation == false)
                {
                    yield return new ValidationResult($"Filter with CreatedStartDateTime: {CreatedStartDateTime}, CreatedEndDateTime: {CreatedEndDateTime} is Over Limit {ticketWebApiOptions.DaysLimit} Days", new List<string> { "CreatedStartDateTime", "CreatedEndDateTime" });
                }
            }
        }
    }
}
