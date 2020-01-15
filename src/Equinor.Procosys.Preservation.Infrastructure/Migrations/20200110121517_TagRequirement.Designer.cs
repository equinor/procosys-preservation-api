﻿// <auto-generated />
using System;
using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    [DbContext(typeof(PreservationContext))]
    [Migration("20200110121517_TagRequirement")]
    partial class TagRequirement
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Journey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Journeys");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("JourneyId")
                        .HasColumnType("int");

                    b.Property<int>("ModeId")
                        .HasColumnType("int");

                    b.Property<int>("ResponsibleId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("JourneyId");

                    b.HasIndex("ModeId");

                    b.ToTable("Step");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate.Mode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Modes");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.Field", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FieldType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Info");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("RequirementDefinitionId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<bool>("ShowPrevious")
                        .HasColumnType("bit");

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("RequirementDefinitionId");

                    b.ToTable("Fields");

                    b.HasCheckConstraint("constraint_field_check_valid_fieldtype", "FieldType in ('Info','Number','CheckBox','Attachment')");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DefaultInterval")
                        .HasColumnType("int");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<int>("RequirementTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("RequirementTypeId");

                    b.ToTable("RequirementDefinitions");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("RequirementTypes");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate.Responsible", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Responsibles");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Requirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Interval")
                        .HasColumnType("int");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<int>("RequirementDefinitionId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.ToTable("Requirements");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<bool>("IsAreaTag")
                        .HasColumnType("bit");

                    b.Property<string>("ProjectNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("StepId")
                        .HasColumnType("int");

                    b.Property<string>("TagNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("StepId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Journey", null)
                        .WithMany("Steps")
                        .HasForeignKey("JourneyId");

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate.Mode", null)
                        .WithMany()
                        .HasForeignKey("ModeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.Field", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", null)
                        .WithMany("Fields")
                        .HasForeignKey("RequirementDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType", null)
                        .WithMany("RequirementDefinitions")
                        .HasForeignKey("RequirementTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Requirement", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Tag", null)
                        .WithMany("Requirements")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Tag", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", null)
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
