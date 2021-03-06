// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoneyHeist.Models;

namespace MoneyHeist.Migrations
{
    [DbContext(typeof(MoneyHeistContext))]
    [Migration("20211029170755_Added table HeistOutcome")]
    partial class AddedtableHeistOutcome
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MoneyHeist.Models.Heist", b =>
                {
                    b.Property<int>("HeistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("HeistId");

                    b.HasIndex("StatusId");

                    b.ToTable("Heists");
                });

            modelBuilder.Entity("MoneyHeist.Models.HeistMember", b =>
                {
                    b.Property<int>("HeistMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("HeistId")
                        .HasColumnType("int");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.HasKey("HeistMemberId");

                    b.HasIndex("HeistId");

                    b.HasIndex("MemberId");

                    b.ToTable("HeistMembers");
                });

            modelBuilder.Entity("MoneyHeist.Models.HeistOutcome", b =>
                {
                    b.Property<int>("HeistOutcomeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("HeistId")
                        .HasColumnType("int");

                    b.Property<string>("Outcome")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HeistOutcomeId");

                    b.ToTable("HeistOutcome");
                });

            modelBuilder.Entity("MoneyHeist.Models.HeistSkill", b =>
                {
                    b.Property<int>("HeistSkillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("HeistId")
                        .HasColumnType("int");

                    b.Property<int>("HeistMembers")
                        .HasColumnType("int");

                    b.Property<int>("SkillId")
                        .HasColumnType("int");

                    b.Property<int>("SkillLevel")
                        .HasColumnType("int");

                    b.HasKey("HeistSkillId");

                    b.HasIndex("HeistId");

                    b.HasIndex("SkillId");

                    b.ToTable("HeistSkills");
                });

            modelBuilder.Entity("MoneyHeist.Models.HeistStatus", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StatusId");

                    b.ToTable("HeistStatuses");

                    b.HasData(
                        new
                        {
                            StatusId = 1,
                            Name = "PLANNING"
                        },
                        new
                        {
                            StatusId = 2,
                            Name = "READY"
                        },
                        new
                        {
                            StatusId = 3,
                            Name = "IN_PROGRESS"
                        },
                        new
                        {
                            StatusId = 4,
                            Name = "FINISHED"
                        });
                });

            modelBuilder.Entity("MoneyHeist.Models.Member", b =>
                {
                    b.Property<int>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sex")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("MemberId");

                    b.HasIndex("StatusId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("MoneyHeist.Models.MemberSkill", b =>
                {
                    b.Property<int>("MemberSkillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsMain")
                        .HasColumnType("bit");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("SkillId")
                        .HasColumnType("int");

                    b.Property<int>("SkillLevel")
                        .HasColumnType("int");

                    b.HasKey("MemberSkillId");

                    b.HasIndex("MemberId");

                    b.HasIndex("SkillId");

                    b.ToTable("MemberSkills");
                });

            modelBuilder.Entity("MoneyHeist.Models.MemberStatus", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StatusId");

                    b.ToTable("MemberStatuses");

                    b.HasData(
                        new
                        {
                            StatusId = 1,
                            Name = "AVAILABLE"
                        },
                        new
                        {
                            StatusId = 2,
                            Name = "EXPIRED"
                        },
                        new
                        {
                            StatusId = 3,
                            Name = "INCARCERATED"
                        },
                        new
                        {
                            StatusId = 4,
                            Name = "RETIRED"
                        });
                });

            modelBuilder.Entity("MoneyHeist.Models.Skill", b =>
                {
                    b.Property<int>("SkillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SkillId");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("MoneyHeist.Models.Heist", b =>
                {
                    b.HasOne("MoneyHeist.Models.HeistStatus", "HeistStatus")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MoneyHeist.Models.HeistMember", b =>
                {
                    b.HasOne("MoneyHeist.Models.Heist", "Heist")
                        .WithMany()
                        .HasForeignKey("HeistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoneyHeist.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MoneyHeist.Models.HeistSkill", b =>
                {
                    b.HasOne("MoneyHeist.Models.Heist", null)
                        .WithMany("HeistSkills")
                        .HasForeignKey("HeistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoneyHeist.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MoneyHeist.Models.Member", b =>
                {
                    b.HasOne("MoneyHeist.Models.MemberStatus", "MemberStatus")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MoneyHeist.Models.MemberSkill", b =>
                {
                    b.HasOne("MoneyHeist.Models.Member", null)
                        .WithMany("MemberSkills")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoneyHeist.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
