﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Helpers;

namespace WebApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20180919161205_migr2")]
    partial class migr2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApi.Entities.Budget", b =>
                {
                    b.Property<int>("BudgetId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ChangeDateTime");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartingDate");

                    b.Property<int>("UserId");

                    b.HasKey("BudgetId");

                    b.HasIndex("UserId");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("WebApi.Entities.BudgetCategory", b =>
                {
                    b.Property<int>("BudgetCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetId");

                    b.Property<DateTime>("ChangeDateTime");

                    b.Property<string>("Icon")
                        .IsRequired();

                    b.Property<double>("MonthlyAmount");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("BudgetCategoryId");

                    b.HasIndex("BudgetId");

                    b.ToTable("BudgetCategories");
                });

            modelBuilder.Entity("WebApi.Entities.PasswordChange", b =>
                {
                    b.Property<int>("PasswordChangeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ChangeDateTime");

                    b.Property<int>("UserId");

                    b.HasKey("PasswordChangeId");

                    b.HasIndex("UserId");

                    b.ToTable("PasswordChanges");
                });

            modelBuilder.Entity("WebApi.Entities.PasswordReset", b =>
                {
                    b.Property<int>("PasswordResetId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("GenerationDateTime");

                    b.Property<string>("Token")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("PasswordResetId");

                    b.HasIndex("UserId");

                    b.ToTable("PasswordResets");
                });

            modelBuilder.Entity("WebApi.Entities.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount");

                    b.Property<int>("BudgetCategoryId");

                    b.Property<int>("CreatedByUserId");

                    b.Property<DateTime>("CreationDateTime");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTime>("TransactionDateTime");

                    b.HasKey("TransactionId");

                    b.HasIndex("BudgetCategoryId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("WebApi.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(160);

                    b.Property<DateTime?>("EmailVerifiedTime");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(160);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApi.Entities.Budget", b =>
                {
                    b.HasOne("WebApi.Entities.User", "User")
                        .WithMany("Budgets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApi.Entities.BudgetCategory", b =>
                {
                    b.HasOne("WebApi.Entities.Budget", "Budget")
                        .WithMany("BudgetCategories")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApi.Entities.PasswordChange", b =>
                {
                    b.HasOne("WebApi.Entities.User", "User")
                        .WithMany("PasswordChanges")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApi.Entities.PasswordReset", b =>
                {
                    b.HasOne("WebApi.Entities.User", "User")
                        .WithMany("PasswordResets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApi.Entities.Transaction", b =>
                {
                    b.HasOne("WebApi.Entities.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
