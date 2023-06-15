using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ticket.WebApi.Migrations.Ticket
{
    public partial class Test2ToDoTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    _CreatedDate = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    _DeletedFlag = table.Column<bool>(nullable: false),
                    _DeletedDate = table.Column<DateTime>(nullable: true),
                    _DeletedBy = table.Column<string>(maxLength: 256, nullable: true),
                    _LastModifiedDate = table.Column<DateTime>(nullable: true),
                    _LastModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    TicketId = table.Column<string>(maxLength: 50, nullable: false),
                    TicketSummary = table.Column<string>(maxLength: 255, nullable: false),
                    TicketDescription = table.Column<string>(nullable: false),
                    TicketPriority = table.Column<string>(maxLength: 50, nullable: false),
                    TicketStatus = table.Column<string>(maxLength: 50, nullable: false),
                    DueDate = table.Column<DateTime>(maxLength: 50, nullable: false),
                    UserId = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    _CreatedDate = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    _DeletedFlag = table.Column<bool>(nullable: false),
                    _DeletedDate = table.Column<DateTime>(nullable: true),
                    _DeletedBy = table.Column<string>(maxLength: 256, nullable: true),
                    _LastModifiedDate = table.Column<DateTime>(nullable: true),
                    _LastModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    MimeType = table.Column<string>(nullable: true),
                    BinaryData = table.Column<byte[]>(nullable: true),
                    TicketId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TicketId",
                table: "Attachment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket__CreatedDate",
                table: "Ticket",
                column: "_CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketId",
                table: "Ticket",
                column: "TicketId",
                unique: true,
                filter: "_DeletedFlag = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketPriority",
                table: "Ticket",
                column: "TicketPriority");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketStatus",
                table: "Ticket",
                column: "TicketStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketSummary",
                table: "Ticket",
                column: "TicketSummary");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserId",
                table: "Ticket",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "Ticket");
        }
    }
}
