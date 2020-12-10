﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OutlookClone.Models;

namespace OutlookClone.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20201209175931_MailModelAddReadField")]
    partial class MailModelAddReadField
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("ContactModelGroupModel", b =>
                {
                    b.Property<int>("GroupMembersId")
                        .HasColumnType("int");

                    b.Property<int>("GroupsId")
                        .HasColumnType("int");

                    b.HasKey("GroupMembersId", "GroupsId");

                    b.HasIndex("GroupsId");

                    b.ToTable("ContactModelGroupModel");
                });

            modelBuilder.Entity("ContactModelMailModel", b =>
                {
                    b.Property<int>("MailsId")
                        .HasColumnType("int");

                    b.Property<int>("ToId")
                        .HasColumnType("int");

                    b.HasKey("MailsId", "ToId");

                    b.HasIndex("ToId");

                    b.ToTable("ContactModelMailModel");
                });

            modelBuilder.Entity("OutlookClone.Models.ContactModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Guid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("OutlookClone.Models.GroupModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("GroupOwnerID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("OutlookClone.Models.MailModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Body")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("FromId")
                        .HasColumnType("int");

                    b.Property<bool>("Read")
                        .HasColumnType("bit");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Mails");
                });

            modelBuilder.Entity("ContactModelGroupModel", b =>
                {
                    b.HasOne("OutlookClone.Models.ContactModel", null)
                        .WithMany()
                        .HasForeignKey("GroupMembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OutlookClone.Models.GroupModel", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContactModelMailModel", b =>
                {
                    b.HasOne("OutlookClone.Models.MailModel", null)
                        .WithMany()
                        .HasForeignKey("MailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OutlookClone.Models.ContactModel", null)
                        .WithMany()
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
