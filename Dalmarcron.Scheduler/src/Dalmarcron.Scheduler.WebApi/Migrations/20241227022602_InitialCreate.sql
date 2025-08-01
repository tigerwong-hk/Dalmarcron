﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "ApiLogs" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "ActionName" text,
    "EventType" text NOT NULL,
    "ResponseStatusCode" integer NOT NULL,
    "Url" text NOT NULL,
    "UserIp" text,
    "CreatedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "DurationMsec" integer NOT NULL,
    "LogDetail" jsonb NOT NULL,
    "ModifiedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "Status" boolean NOT NULL,
    "TraceId" text NOT NULL,
    "UserId" text,
    CONSTRAINT "PK_ApiLogs" PRIMARY KEY ("Id")
);

CREATE TABLE "AuditLogs" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "AuditScopeId" uuid NOT NULL,
    "Action" text NOT NULL,
    "ChangedValues" jsonb,
    "Error" text,
    "PrimaryKey" text NOT NULL,
    "Table" text NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "DurationMsec" integer NOT NULL,
    "LogDetail" jsonb NOT NULL,
    "ModifiedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "Status" boolean NOT NULL,
    "TraceId" text NOT NULL,
    "UserId" text,
    CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id")
);

CREATE TABLE "ScheduledJobs" (
    "ScheduledJobId" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "ApiMethod" character varying(20) NOT NULL,
    "ApiType" character varying(20) NOT NULL,
    "ApiUrl" text NOT NULL,
    "CronExpression" text NOT NULL,
    "JobName" text NOT NULL,
    "PublicationState" character varying(20) NOT NULL,
    "ApiHeaders" text,
    "ApiIdempotencyKey" text,
    "ApiJsonBody" text,
    "Oauth2BaseUri" text,
    "Oauth2ClientId" text,
    "Oauth2ClientScopes" jsonb,
    "Oauth2ClientSecret" text,
    "ClientId" text NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreateRequestId" text NOT NULL,
    "CreatorId" text NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "ModifiedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "ModifierId" text NOT NULL,
    CONSTRAINT "PK_ScheduledJobs" PRIMARY KEY ("ScheduledJobId")
);

CREATE TABLE "JobPublishedTransactions" (
    "JobPublishedTransactionId" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "ApiMethod" character varying(20) NOT NULL,
    "ApiType" character varying(20) NOT NULL,
    "ApiUrl" text NOT NULL,
    "CronExpression" text NOT NULL,
    "JobName" text NOT NULL,
    "LambdaFunctionArn" text NOT NULL,
    "LambdaPermissionStatement" text NOT NULL,
    "LambdaTriggerArn" text NOT NULL,
    "ApiHeaders" text,
    "ApiIdempotencyKey" text,
    "ApiJsonBody" text,
    "Oauth2BaseUri" text,
    "Oauth2ClientId" text,
    "Oauth2ClientScopes" jsonb,
    "Oauth2ClientSecret" text,
    "ScheduledJobId" uuid NOT NULL,
    "ClientId" text NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreateRequestId" text NOT NULL,
    "CreatorId" text NOT NULL,
    CONSTRAINT "PK_JobPublishedTransactions" PRIMARY KEY ("JobPublishedTransactionId"),
    CONSTRAINT "FK_JobPublishedTransactions_ScheduledJobs_ScheduledJobId" FOREIGN KEY ("ScheduledJobId") REFERENCES "ScheduledJobs" ("ScheduledJobId") ON DELETE CASCADE
);

CREATE TABLE "JobUnpublishedTransactions" (
    "JobUnpublishedTransactionId" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "ApiMethod" character varying(20) NOT NULL,
    "ApiType" character varying(20) NOT NULL,
    "ApiUrl" text NOT NULL,
    "CronExpression" text NOT NULL,
    "JobName" text NOT NULL,
    "ApiHeaders" text NOT NULL,
    "ApiIdempotencyKey" text,
    "ApiJsonBody" text,
    "Oauth2BaseUri" text,
    "Oauth2ClientId" text,
    "Oauth2ClientScopes" jsonb,
    "Oauth2ClientSecret" text,
    "ScheduledJobId" uuid NOT NULL,
    "ClientId" text NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "CreateRequestId" text NOT NULL,
    "CreatorId" text NOT NULL,
    CONSTRAINT "PK_JobUnpublishedTransactions" PRIMARY KEY ("JobUnpublishedTransactionId"),
    CONSTRAINT "FK_JobUnpublishedTransactions_ScheduledJobs_ScheduledJobId" FOREIGN KEY ("ScheduledJobId") REFERENCES "ScheduledJobs" ("ScheduledJobId") ON DELETE CASCADE
);

CREATE INDEX "IX_ApiLogs_ActionName" ON "ApiLogs" ("ActionName");

CREATE INDEX "IX_ApiLogs_CreatedOn" ON "ApiLogs" ("CreatedOn");

CREATE INDEX "IX_ApiLogs_EventType" ON "ApiLogs" ("EventType");

CREATE INDEX "IX_ApiLogs_ModifiedOn" ON "ApiLogs" ("ModifiedOn");

CREATE INDEX "IX_ApiLogs_ResponseStatusCode" ON "ApiLogs" ("ResponseStatusCode");

CREATE INDEX "IX_ApiLogs_TraceId" ON "ApiLogs" ("TraceId");

CREATE INDEX "IX_ApiLogs_Url" ON "ApiLogs" ("Url");

CREATE INDEX "IX_ApiLogs_UserId" ON "ApiLogs" ("UserId");

CREATE INDEX "IX_ApiLogs_UserIp" ON "ApiLogs" ("UserIp");

CREATE INDEX "IX_AuditLogs_ChangedValues" ON "AuditLogs" USING gin ("ChangedValues");

CREATE INDEX "IX_AuditLogs_CreatedOn" ON "AuditLogs" ("CreatedOn");

CREATE INDEX "IX_AuditLogs_ModifiedOn" ON "AuditLogs" ("ModifiedOn");

CREATE INDEX "IX_AuditLogs_PrimaryKey" ON "AuditLogs" ("PrimaryKey");

CREATE INDEX "IX_AuditLogs_Table_PrimaryKey" ON "AuditLogs" ("Table", "PrimaryKey");

CREATE INDEX "IX_AuditLogs_TraceId" ON "AuditLogs" ("TraceId");

CREATE INDEX "IX_AuditLogs_UserId" ON "AuditLogs" ("UserId");

CREATE INDEX "IX_JobPublishedTransactions_ClientId" ON "JobPublishedTransactions" ("ClientId");

CREATE INDEX "IX_JobPublishedTransactions_CreatedOn" ON "JobPublishedTransactions" ("CreatedOn");

CREATE UNIQUE INDEX "IX_JobPublishedTransactions_CreateRequestId_ClientId" ON "JobPublishedTransactions" ("CreateRequestId", "ClientId");

CREATE INDEX "IX_JobPublishedTransactions_CreatorId" ON "JobPublishedTransactions" ("CreatorId");

CREATE INDEX "IX_JobPublishedTransactions_ScheduledJobId" ON "JobPublishedTransactions" ("ScheduledJobId");

CREATE INDEX "IX_JobUnpublishedTransactions_ClientId" ON "JobUnpublishedTransactions" ("ClientId");

CREATE INDEX "IX_JobUnpublishedTransactions_CreatedOn" ON "JobUnpublishedTransactions" ("CreatedOn");

CREATE UNIQUE INDEX "IX_JobUnpublishedTransactions_CreateRequestId_ClientId" ON "JobUnpublishedTransactions" ("CreateRequestId", "ClientId");

CREATE INDEX "IX_JobUnpublishedTransactions_CreatorId" ON "JobUnpublishedTransactions" ("CreatorId");

CREATE INDEX "IX_JobUnpublishedTransactions_ScheduledJobId" ON "JobUnpublishedTransactions" ("ScheduledJobId");

CREATE INDEX "IX_ScheduledJobs_ClientId" ON "ScheduledJobs" ("ClientId");

CREATE INDEX "IX_ScheduledJobs_CreatedOn" ON "ScheduledJobs" ("CreatedOn");

CREATE UNIQUE INDEX "IX_ScheduledJobs_CreateRequestId_ClientId" ON "ScheduledJobs" ("CreateRequestId", "ClientId");

CREATE INDEX "IX_ScheduledJobs_CreatorId" ON "ScheduledJobs" ("CreatorId");

CREATE UNIQUE INDEX "IX_ScheduledJobs_JobName" ON "ScheduledJobs" ("JobName") WHERE "IsDeleted" = false;

CREATE INDEX "IX_ScheduledJobs_ModifiedOn" ON "ScheduledJobs" ("ModifiedOn");

CREATE INDEX "IX_ScheduledJobs_ModifierId" ON "ScheduledJobs" ("ModifierId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241227022602_InitialCreate', '8.0.11');

COMMIT;

