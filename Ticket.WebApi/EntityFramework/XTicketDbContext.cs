using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.Entity;
using Xtremax.Base.EntityFramework;

namespace Ticket.WebApi.EntityFramework
{
    public class XTicketDbContext : AppDbContext
    {
        public DbSet<TicketEntity> Ticket { get; set; }

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketEntity>()
                .Property(t => t.TicketId)
                .IsRequired();

            modelBuilder.Entity<TicketEntity>(typeBuilder =>
            {
                // Unique Key, we must enforce filter to '_DeletedFlag = 0' otherwise EF Generator tends to add Filter 'WHERE ScopeId <> NULL AND ScopeItemId <> NULL'
                typeBuilder.HasIndex(t => t.TicketId).IsUnique().HasFilter("_DeletedFlag = 0");
                typeBuilder.HasIndex(t => t.TicketSummary);
                typeBuilder.HasIndex(t => t.TicketPriority);
                typeBuilder.HasIndex(t => t.TicketStatus);
                typeBuilder.HasIndex(t => t.UserId);
                typeBuilder.HasIndex(t => t.CreatedDate);
            });
        }
        #endregion

        public DbSet<AttachmentEntity> Attachment { get; set; }

        public XTicketDbContext(DbContextOptions<XTicketDbContext> options) : base(options)
        {
        }
    }
}
