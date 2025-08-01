﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Dalmarcron.Scheduler.EntityFrameworkCore.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dalmarcron.Scheduler.WebApi.Migrations
{
    [DbContext(typeof(DalmarcronSchedulerDbContext))]
    partial class DalmarcronSchedulerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.JobPublishedTransaction", b =>
                {
                    b.Property<Guid>("JobPublishedTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("ApiHeaders")
                        .HasColumnType("text");

                    b.Property<string>("ApiIdempotencyKey")
                        .HasColumnType("text");

                    b.Property<string>("ApiJsonBody")
                        .HasColumnType("text");

                    b.Property<string>("ApiMethod")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ApiType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ApiUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreateRequestId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CronExpression")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LambdaFunctionArn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LambdaPermissionStatement")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LambdaTriggerArn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Oauth2BaseUri")
                        .HasColumnType("text");

                    b.Property<string>("Oauth2ClientId")
                        .HasColumnType("text");

                    b.Property<List<string>>("Oauth2ClientScopes")
                        .HasColumnType("jsonb");

                    b.Property<string>("Oauth2ClientSecret")
                        .HasColumnType("text");

                    b.Property<Guid>("ScheduledJobId")
                        .HasColumnType("uuid");

                    b.HasKey("JobPublishedTransactionId");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("CreatorId");

                    b.HasIndex("ScheduledJobId");

                    b.HasIndex("CreateRequestId", "ClientId")
                        .IsUnique();

                    b.ToTable("JobPublishedTransactions");
                });

            modelBuilder.Entity("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.JobUnpublishedTransaction", b =>
                {
                    b.Property<Guid>("JobUnpublishedTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("ApiHeaders")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ApiIdempotencyKey")
                        .HasColumnType("text");

                    b.Property<string>("ApiJsonBody")
                        .HasColumnType("text");

                    b.Property<string>("ApiMethod")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ApiType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ApiUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreateRequestId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CronExpression")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Oauth2BaseUri")
                        .HasColumnType("text");

                    b.Property<string>("Oauth2ClientId")
                        .HasColumnType("text");

                    b.Property<List<string>>("Oauth2ClientScopes")
                        .HasColumnType("jsonb");

                    b.Property<string>("Oauth2ClientSecret")
                        .HasColumnType("text");

                    b.Property<Guid>("ScheduledJobId")
                        .HasColumnType("uuid");

                    b.HasKey("JobUnpublishedTransactionId");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("CreatorId");

                    b.HasIndex("ScheduledJobId");

                    b.HasIndex("CreateRequestId", "ClientId")
                        .IsUnique();

                    b.ToTable("JobUnpublishedTransactions");
                });

            modelBuilder.Entity("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.ScheduledJob", b =>
                {
                    b.Property<Guid>("ScheduledJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("ApiHeaders")
                        .HasColumnType("text");

                    b.Property<string>("ApiIdempotencyKey")
                        .HasColumnType("text");

                    b.Property<string>("ApiJsonBody")
                        .HasColumnType("text");

                    b.Property<string>("ApiMethod")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ApiType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ApiUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreateRequestId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CronExpression")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("ModifierId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Oauth2BaseUri")
                        .HasColumnType("text");

                    b.Property<string>("Oauth2ClientId")
                        .HasColumnType("text");

                    b.Property<List<string>>("Oauth2ClientScopes")
                        .HasColumnType("jsonb");

                    b.Property<string>("Oauth2ClientSecret")
                        .HasColumnType("text");

                    b.Property<string>("PublicationState")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("ScheduledJobId");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("CreatorId");

                    b.HasIndex("JobName")
                        .IsUnique()
                        .HasFilter("\"IsDeleted\" = false");

                    b.HasIndex("ModifiedOn");

                    b.HasIndex("ModifierId");

                    b.HasIndex("CreateRequestId", "ClientId")
                        .IsUnique();

                    b.ToTable("ScheduledJobs");
                });

            modelBuilder.Entity("Dalmarkit.Common.AuditTrail.ApiLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ActionName")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<int>("DurationMsec")
                        .HasColumnType("integer");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LogDetail")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("ModifiedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<int>("ResponseStatusCode")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<string>("TraceId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("UserIp")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ActionName");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("EventType");

                    b.HasIndex("ModifiedOn");

                    b.HasIndex("ResponseStatusCode");

                    b.HasIndex("TraceId");

                    b.HasIndex("Url");

                    b.HasIndex("UserId");

                    b.HasIndex("UserIp");

                    b.ToTable("ApiLogs");
                });

            modelBuilder.Entity("Dalmarkit.Common.AuditTrail.AuditLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("AuditScopeId")
                        .HasColumnType("uuid");

                    b.Property<string>("ChangedValues")
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<int>("DurationMsec")
                        .HasColumnType("integer");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<string>("LogDetail")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("ModifiedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("PrimaryKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<string>("Table")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TraceId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChangedValues");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("ChangedValues"), "gin");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("ModifiedOn");

                    b.HasIndex("PrimaryKey");

                    b.HasIndex("TraceId");

                    b.HasIndex("UserId");

                    b.HasIndex("Table", "PrimaryKey");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.JobPublishedTransaction", b =>
                {
                    b.HasOne("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.ScheduledJob", "ScheduledJob")
                        .WithMany()
                        .HasForeignKey("ScheduledJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduledJob");
                });

            modelBuilder.Entity("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.JobUnpublishedTransaction", b =>
                {
                    b.HasOne("Dalmarcron.Scheduler.EntityFrameworkCore.Entities.ScheduledJob", "ScheduledJob")
                        .WithMany()
                        .HasForeignKey("ScheduledJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduledJob");
                });
#pragma warning restore 612, 618
        }
    }
}
