using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class CreatedAndModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Tags",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Step",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Step",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Step",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Step",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Responsibles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Responsibles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Responsibles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Responsibles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "RequirementTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "RequirementTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "RequirementTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "RequirementTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Requirements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Requirements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Requirements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Requirements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "RequirementDefinitions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "RequirementDefinitions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "RequirementDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "RequirementDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Projects",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "PreservationRecords",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "PreservationRecords",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "PreservationPeriods",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "PreservationPeriods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Modes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Modes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Modes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Modes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Journeys",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Journeys",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Journeys",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Journeys",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "FieldValues",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "FieldValues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Fields",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Fields",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Fields",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Fields",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Actions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                table: "Actions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedById",
                table: "Tags",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Step_CreatedById",
                table: "Step",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Step_ModifiedById",
                table: "Step",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_CreatedById",
                table: "Responsibles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_ModifiedById",
                table: "Responsibles",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTypes_CreatedById",
                table: "RequirementTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTypes_ModifiedById",
                table: "RequirementTypes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_CreatedById",
                table: "Requirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_ModifiedById",
                table: "Requirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_CreatedById",
                table: "RequirementDefinitions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_ModifiedById",
                table: "RequirementDefinitions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ModifiedById",
                table: "Projects",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_CreatedById",
                table: "PreservationRecords",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_ModifiedById",
                table: "PreservationRecords",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_CreatedById",
                table: "PreservationPeriods",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_ModifiedById",
                table: "PreservationPeriods",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ModifiedById",
                table: "Persons",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_CreatedById",
                table: "Modes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_ModifiedById",
                table: "Modes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_CreatedById",
                table: "Journeys",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_ModifiedById",
                table: "Journeys",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_CreatedById",
                table: "FieldValues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_CreatedById",
                table: "Fields",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ModifiedById",
                table: "Fields",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ModifiedById",
                table: "Actions",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Persons_ModifiedById",
                table: "Actions",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Persons_CreatedById",
                table: "Fields",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Persons_ModifiedById",
                table: "Fields",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValues_Persons_CreatedById",
                table: "FieldValues",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Persons_CreatedById",
                table: "Journeys",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Persons_ModifiedById",
                table: "Journeys",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modes_Persons_CreatedById",
                table: "Modes",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modes_Persons_ModifiedById",
                table: "Modes",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Persons_ModifiedById",
                table: "Persons",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_Persons_CreatedById",
                table: "PreservationPeriods",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_Persons_ModifiedById",
                table: "PreservationPeriods",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_CreatedById",
                table: "PreservationRecords",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_ModifiedById",
                table: "PreservationRecords",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Persons_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Persons_ModifiedById",
                table: "Projects",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_Persons_CreatedById",
                table: "RequirementDefinitions",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_Persons_ModifiedById",
                table: "RequirementDefinitions",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Persons_CreatedById",
                table: "Requirements",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Persons_ModifiedById",
                table: "Requirements",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTypes_Persons_CreatedById",
                table: "RequirementTypes",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTypes_Persons_ModifiedById",
                table: "RequirementTypes",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibles_Persons_CreatedById",
                table: "Responsibles",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibles_Persons_ModifiedById",
                table: "Responsibles",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Persons_CreatedById",
                table: "Step",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Persons_ModifiedById",
                table: "Step",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Persons_CreatedById",
                table: "Tags",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Persons_ModifiedById",
                table: "Tags",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Persons_ModifiedById",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Persons_CreatedById",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Persons_ModifiedById",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldValues_Persons_CreatedById",
                table: "FieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Persons_CreatedById",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Persons_ModifiedById",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_Modes_Persons_CreatedById",
                table: "Modes");

            migrationBuilder.DropForeignKey(
                name: "FK_Modes_Persons_ModifiedById",
                table: "Modes");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Persons_ModifiedById",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_Persons_CreatedById",
                table: "PreservationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_Persons_ModifiedById",
                table: "PreservationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_CreatedById",
                table: "PreservationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_ModifiedById",
                table: "PreservationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Persons_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Persons_ModifiedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_Persons_CreatedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_Persons_ModifiedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Persons_CreatedById",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Persons_ModifiedById",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTypes_Persons_CreatedById",
                table: "RequirementTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTypes_Persons_ModifiedById",
                table: "RequirementTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsibles_Persons_CreatedById",
                table: "Responsibles");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsibles_Persons_ModifiedById",
                table: "Responsibles");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Persons_CreatedById",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Persons_ModifiedById",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Persons_CreatedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Persons_ModifiedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ModifiedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Step_CreatedById",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Step_ModifiedById",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Responsibles_CreatedById",
                table: "Responsibles");

            migrationBuilder.DropIndex(
                name: "IX_Responsibles_ModifiedById",
                table: "Responsibles");

            migrationBuilder.DropIndex(
                name: "IX_RequirementTypes_CreatedById",
                table: "RequirementTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequirementTypes_ModifiedById",
                table: "RequirementTypes");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_CreatedById",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_ModifiedById",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_RequirementDefinitions_CreatedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_RequirementDefinitions_ModifiedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ModifiedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_CreatedById",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_ModifiedById",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_CreatedById",
                table: "PreservationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_ModifiedById",
                table: "PreservationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Persons_ModifiedById",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Modes_CreatedById",
                table: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_Modes_ModifiedById",
                table: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_CreatedById",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_ModifiedById",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_FieldValues_CreatedById",
                table: "FieldValues");

            migrationBuilder.DropIndex(
                name: "IX_Fields_CreatedById",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Fields_ModifiedById",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Actions_ModifiedById",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "RequirementTypes");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "PreservationPeriods");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Actions");
        }
    }
}
