﻿// <auto-generated />
using System;
using DataAccess_Layer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess_Layer.Data.Migrations
{
    [DbContext(typeof(UploadDbContext))]
    [Migration("20240625035639_Init-DBUploadFile")]
    partial class InitDBUploadFile
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataAccess_Layer.Models.AttachmentFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileNameToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("AttachmentFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
