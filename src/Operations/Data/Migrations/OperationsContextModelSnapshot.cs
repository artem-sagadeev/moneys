﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Operations.Data;

#nullable disable

namespace Operations.Migrations
{
    [DbContext(typeof(OperationsContext))]
    partial class OperationsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Operations.Entities.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Balance")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Cards", (string)null);
                });

            modelBuilder.Entity("Operations.Entities.IncomeRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("RegularIncomeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("RegularIncomeId");

                    b.ToTable("IncomeRecords", (string)null);
                });

            modelBuilder.Entity("Operations.Entities.PaymentRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("RegularPaymentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("RegularPaymentId");

                    b.ToTable("PaymentRecords", (string)null);
                });

            modelBuilder.Entity("Operations.Entities.RegularIncome", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<int>("Frequency")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("NextExecution")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("RegularIncomes", (string)null);
                });

            modelBuilder.Entity("Operations.Entities.RegularPayment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<int>("Frequency")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("NextExecution")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("RegularPayments", (string)null);
                });

            modelBuilder.Entity("Operations.Entities.IncomeRecord", b =>
                {
                    b.HasOne("Operations.Entities.Card", "Card")
                        .WithMany("IncomeRecords")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Operations.Entities.RegularIncome", "RegularIncome")
                        .WithMany("IncomeRecords")
                        .HasForeignKey("RegularIncomeId");

                    b.Navigation("Card");

                    b.Navigation("RegularIncome");
                });

            modelBuilder.Entity("Operations.Entities.PaymentRecord", b =>
                {
                    b.HasOne("Operations.Entities.Card", "Card")
                        .WithMany("PaymentRecords")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Operations.Entities.RegularPayment", "RegularPayment")
                        .WithMany("PaymentRecords")
                        .HasForeignKey("RegularPaymentId");

                    b.Navigation("Card");

                    b.Navigation("RegularPayment");
                });

            modelBuilder.Entity("Operations.Entities.RegularIncome", b =>
                {
                    b.HasOne("Operations.Entities.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("Operations.Entities.RegularPayment", b =>
                {
                    b.HasOne("Operations.Entities.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("Operations.Entities.Card", b =>
                {
                    b.Navigation("IncomeRecords");

                    b.Navigation("PaymentRecords");
                });

            modelBuilder.Entity("Operations.Entities.RegularIncome", b =>
                {
                    b.Navigation("IncomeRecords");
                });

            modelBuilder.Entity("Operations.Entities.RegularPayment", b =>
                {
                    b.Navigation("PaymentRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
