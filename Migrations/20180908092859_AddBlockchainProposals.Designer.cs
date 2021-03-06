﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZestMonitor.Api.Data.Contexts;

namespace ZestMonitor.Api.Migrations
{
    [DbContext(typeof(ZestContext))]
    [Migration("20180908092859_AddBlockchainProposals")]
    partial class AddBlockchainProposals
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ZestMonitor.Api.Data.Entities.BlockchainProposal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Abstains");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("FValid");

                    b.Property<string>("FeeHash");

                    b.Property<string>("Hash");

                    b.Property<bool>("IsEstablished");

                    b.Property<bool>("IsValid");

                    b.Property<string>("IsValidReason");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Nays");

                    b.Property<double>("Ratio");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Url");

                    b.Property<int>("Yeas");

                    b.HasKey("Id");

                    b.ToTable("BlockchainProposal");
                });

            modelBuilder.Entity("ZestMonitor.Api.Data.Entities.ProposalPayments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("ExpectedPayment");

                    b.Property<string>("Hash")
                        .IsRequired();

                    b.Property<string>("ShortDescription");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("ProposalPayments");
                });

            modelBuilder.Entity("ZestMonitor.Api.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("PasswordSalt")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
