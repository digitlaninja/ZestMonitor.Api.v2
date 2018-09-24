﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZestMonitor.Api.Data.Contexts;

namespace ZestMonitor.Api.Migrations
{
    [DbContext(typeof(ZestContext))]
    partial class ZestContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<bool>("IsFunded");

                    b.Property<bool>("IsValid");

                    b.Property<string>("IsValidReason");

                    b.Property<string>("Name");

                    b.Property<int>("Nays");

                    b.Property<double>("Ratio");

                    b.Property<DateTime?>("Time");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Url");

                    b.Property<int>("Yeas");

                    b.HasKey("Id");

                    b.ToTable("BlockchainProposal");
                });

            modelBuilder.Entity("ZestMonitor.Api.Data.Entities.MasternodeCount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("Enabled");

                    b.Property<int>("IPv4");

                    b.Property<int>("IPv6");

                    b.Property<int>("InQueue");

                    b.Property<int>("ObfCompat");

                    b.Property<int>("Onion");

                    b.Property<int>("Stable");

                    b.Property<int>("Total");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("MasternodeCount");
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
