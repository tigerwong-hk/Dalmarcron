using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dalmarcron.Scheduler.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionName = table.Column<string>(type: "text", nullable: true),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    ResponseStatusCode = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    UserIp = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    DurationMsec = table.Column<int>(type: "integer", nullable: false),
                    LogDetail = table.Column<string>(type: "jsonb", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuditScopeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    ChangedValues = table.Column<string>(type: "jsonb", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: false),
                    Table = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    DurationMsec = table.Column<int>(type: "integer", nullable: false),
                    LogDetail = table.Column<string>(type: "jsonb", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledJobs",
                columns: table => new
                {
                    ScheduledJobId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ApiMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApiType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApiUrl = table.Column<string>(type: "text", nullable: false),
                    CronExpression = table.Column<string>(type: "text", nullable: false),
                    JobName = table.Column<string>(type: "text", nullable: false),
                    ApiHeaders = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    ApiIdempotencyKey = table.Column<string>(type: "text", nullable: true),
                    ApiJsonBody = table.Column<string>(type: "jsonb", nullable: true),
                    Oauth2BaseUri = table.Column<string>(type: "text", nullable: true),
                    Oauth2ClientId = table.Column<string>(type: "text", nullable: true),
                    Oauth2ClientScopes = table.Column<List<string>>(type: "jsonb", nullable: true),
                    Oauth2ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    CreateRequestId = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    ModifierId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledJobs", x => x.ScheduledJobId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_ActionName",
                table: "ApiLogs",
                column: "ActionName");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_CreatedOn",
                table: "ApiLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_EventType",
                table: "ApiLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_ModifiedOn",
                table: "ApiLogs",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_ResponseStatusCode",
                table: "ApiLogs",
                column: "ResponseStatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_TraceId",
                table: "ApiLogs",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_Url",
                table: "ApiLogs",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_UserId",
                table: "ApiLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_UserIp",
                table: "ApiLogs",
                column: "UserIp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedValues",
                table: "AuditLogs",
                column: "ChangedValues")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedOn",
                table: "AuditLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ModifiedOn",
                table: "AuditLogs",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PrimaryKey",
                table: "AuditLogs",
                column: "PrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Table_PrimaryKey",
                table: "AuditLogs",
                columns: new[] { "Table", "PrimaryKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TraceId",
                table: "AuditLogs",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_ClientId",
                table: "ScheduledJobs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_CreatedOn",
                table: "ScheduledJobs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_CreateRequestId_ClientId",
                table: "ScheduledJobs",
                columns: new[] { "CreateRequestId", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_CreatorId",
                table: "ScheduledJobs",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_JobName",
                table: "ScheduledJobs",
                column: "JobName",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_ModifiedOn",
                table: "ScheduledJobs",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobs_ModifierId",
                table: "ScheduledJobs",
                column: "ModifierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogs");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ScheduledJobs");
        }
    }
}
