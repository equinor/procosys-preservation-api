using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Oid = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 64, nullable: false),
                    LastName = table.Column<string>(maxLength: 64, nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Journeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journeys_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Journeys_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    ForSupplier = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modes_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Modes_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PreservationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    ObjectGuid = table.Column<Guid>(nullable: false),
                    PreservedAtUtc = table.Column<DateTime>(nullable: false),
                    PreservedById = table.Column<int>(nullable: false),
                    BulkPreserved = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreservationRecords_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PreservationRecords_Persons_PreservedById",
                        column: x => x.PreservedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    IsClosed = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequirementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Code = table.Column<string>(maxLength: 32, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    SortKey = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequirementTypes_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequirementTypes_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Responsibles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Code = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responsibles_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Responsibles_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TagFunctions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Code = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    RegisterCode = table.Column<string>(maxLength: 255, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagFunctions_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctions_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ObjectGuid = table.Column<Guid>(nullable: false),
                    PreservationRecordId = table.Column<int>(nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    ObjectType = table.Column<int>(nullable: false),
                    DueInWeeks = table.Column<int>(nullable: true),
                    PreservationRecordGuid = table.Column<Guid>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.CheckConstraint("constraint_history_check_valid_event_type", "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','TagDeleted','PreservationStarted','PreservationCompleted','IntervalChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed')");
                    table.ForeignKey(
                        name: "FK_History_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_History_PreservationRecords_PreservationRecordId",
                        column: x => x.PreservationRecordId,
                        principalTable: "PreservationRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequirementDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    DefaultIntervalWeeks = table.Column<int>(nullable: false),
                    Usage = table.Column<string>(maxLength: 32, nullable: false, defaultValue: "ForAll"),
                    SortKey = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RequirementTypeId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementDefinitions", x => x.Id);
                    table.CheckConstraint("constraint_reqdef_check_valid_usage", "Usage in ('ForAll','ForSuppliersOnly','ForOtherThanSuppliers')");
                    table.ForeignKey(
                        name: "FK_RequirementDefinitions_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequirementDefinitions_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                        column: x => x.RequirementTypeId,
                        principalTable: "RequirementTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    ModeId = table.Column<int>(nullable: false),
                    IsSupplierStep = table.Column<bool>(nullable: false),
                    ResponsibleId = table.Column<int>(nullable: false),
                    SortKey = table.Column<int>(nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    JourneyId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Steps_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Steps_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Steps_Modes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Modes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Steps_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Steps_Responsibles_ResponsibleId",
                        column: x => x.ResponsibleId,
                        principalTable: "Responsibles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Label = table.Column<string>(maxLength: 255, nullable: false),
                    Unit = table.Column<string>(maxLength: 32, nullable: true),
                    IsVoided = table.Column<bool>(nullable: false),
                    ShowPrevious = table.Column<bool>(nullable: true),
                    SortKey = table.Column<int>(nullable: false),
                    FieldType = table.Column<string>(nullable: false, defaultValue: "Info"),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RequirementDefinitionId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.CheckConstraint("constraint_field_check_valid_fieldtype", "FieldType in ('Info','Number','CheckBox','Attachment')");
                    table.ForeignKey(
                        name: "FK_Fields_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fields_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                        column: x => x.RequirementDefinitionId,
                        principalTable: "RequirementDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagFunctionRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    IntervalWeeks = table.Column<int>(nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    RequirementDefinitionId = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TagFunctionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFunctionRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_RequirementDefinitions_RequirementDefinitionId",
                        column: x => x.RequirementDefinitionId,
                        principalTable: "RequirementDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_TagFunctions_TagFunctionId",
                        column: x => x.TagFunctionId,
                        principalTable: "TagFunctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    ObjectGuid = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false, defaultValue: 0),
                    AreaCode = table.Column<string>(maxLength: 255, nullable: true),
                    AreaDescription = table.Column<string>(maxLength: 255, nullable: true),
                    Calloff = table.Column<string>(maxLength: 30, nullable: true),
                    CommPkgNo = table.Column<string>(maxLength: 30, nullable: true),
                    DisciplineCode = table.Column<string>(maxLength: 255, nullable: true),
                    DisciplineDescription = table.Column<string>(maxLength: 255, nullable: true),
                    TagType = table.Column<string>(nullable: false, defaultValue: "Standard"),
                    McPkgNo = table.Column<string>(maxLength: 30, nullable: true),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    PurchaseOrderNo = table.Column<string>(maxLength: 20, nullable: true),
                    Remark = table.Column<string>(maxLength: 255, nullable: true),
                    StorageArea = table.Column<string>(maxLength: 255, nullable: true),
                    StepId = table.Column<int>(nullable: false),
                    IsInSupplierStep = table.Column<bool>(nullable: false),
                    TagFunctionCode = table.Column<string>(maxLength: 255, nullable: true),
                    TagNo = table.Column<string>(maxLength: 255, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    NextDueTimeUtc = table.Column<DateTime>(nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.CheckConstraint("constraint_tag_check_valid_statusenum", "Status in (0,1,2)");
                    table.CheckConstraint("constraint_tag_check_valid_tag_type", "TagType in ('Standard','PreArea','SiteArea','PoArea')");
                    table.ForeignKey(
                        name: "FK_Tags_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 4096, nullable: false),
                    DueTimeUtc = table.Column<DateTime>(nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(nullable: true),
                    ClosedById = table.Column<int>(nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_Persons_ClosedById",
                        column: x => x.ClosedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Actions_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Actions_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Actions_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TagRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    IntervalWeeks = table.Column<int>(nullable: false),
                    Usage = table.Column<string>(maxLength: 32, nullable: false, defaultValue: "ForAll"),
                    NextDueTimeUtc = table.Column<DateTime>(nullable: true),
                    IsVoided = table.Column<bool>(nullable: false),
                    RequirementDefinitionId = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TagId = table.Column<int>(nullable: false),
                    _initialPreservationPeriodStatus = table.Column<string>(maxLength: 64, nullable: false, defaultValue: "NeedsUserInput")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagRequirements", x => x.Id);
                    table.CheckConstraint("constraint_requirement_check_valid_initial_status", "_initialPreservationPeriodStatus in ('NeedsUserInput','ReadyToBePreserved')");
                    table.CheckConstraint("constraint_tagreq_check_valid_usage", "Usage in ('ForAll','ForSuppliersOnly','ForOtherThanSuppliers')");
                    table.ForeignKey(
                        name: "FK_TagRequirements_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagRequirements_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagRequirements_RequirementDefinitions_RequirementDefinitionId",
                        column: x => x.RequirementDefinitionId,
                        principalTable: "RequirementDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagRequirements_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    FileName = table.Column<string>(maxLength: 255, nullable: false),
                    BlobPath = table.Column<string>(maxLength: 1024, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    AttachmentType = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ActionId = table.Column<int>(nullable: true),
                    TagId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PreservationPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Status = table.Column<string>(nullable: false, defaultValue: "NeedsUserInput"),
                    DueTimeUtc = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    PreservationRecordId = table.Column<int>(nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TagRequirementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationPeriods", x => x.Id);
                    table.CheckConstraint("constraint_period_check_valid_status", "Status in ('NeedsUserInput','ReadyToBePreserved','Preserved')");
                    table.ForeignKey(
                        name: "FK_PreservationPeriods_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PreservationPeriods_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PreservationPeriods_PreservationRecords_PreservationRecordId",
                        column: x => x.PreservationRecordId,
                        principalTable: "PreservationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                        column: x => x.TagRequirementId,
                        principalTable: "TagRequirements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    FieldId = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    FieldValueAttachmentId = table.Column<int>(nullable: true),
                    FieldType = table.Column<string>(nullable: false),
                    PreservationPeriodId = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Value = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldValues_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FieldValues_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FieldValues_Attachments_FieldValueAttachmentId",
                        column: x => x.FieldValueAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldValues_PreservationPeriods_PreservationPeriodId",
                        column: x => x.PreservationPeriodId,
                        principalTable: "PreservationPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ClosedById",
                table: "Actions",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_CreatedById",
                table: "Actions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ModifiedById",
                table: "Actions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_TagId",
                table: "Actions",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ActionId",
                table: "Attachments",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TagId",
                table: "Attachments",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CreatedById",
                table: "Attachments",
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
                name: "IX_Fields_RequirementDefinitionId",
                table: "Fields",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_CreatedById",
                table: "FieldValues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_FieldId",
                table: "FieldValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_FieldValueAttachmentId",
                table: "FieldValues",
                column: "FieldValueAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_PreservationPeriodId",
                table: "FieldValues",
                column: "PreservationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_History_CreatedById",
                table: "History",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_History_ObjectGuid_ASC",
                table: "History",
                column: "ObjectGuid");

            migrationBuilder.CreateIndex(
                name: "IX_History_PreservationRecordId",
                table: "History",
                column: "PreservationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_CreatedById",
                table: "Journeys",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_ModifiedById",
                table: "Journeys",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_Plant_ASC",
                table: "Journeys",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Modes_CreatedById",
                table: "Modes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_ModifiedById",
                table: "Modes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_Plant_ASC",
                table: "Modes",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ModifiedById",
                table: "Persons",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Oid",
                table: "Persons",
                column: "Oid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_CreatedById",
                table: "PreservationPeriods",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_ModifiedById",
                table: "PreservationPeriods",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_Plant_ASC",
                table: "PreservationPeriods",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Comment", "CreatedAtUtc", "DueTimeUtc", "ModifiedAtUtc", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_PreservationRecordId",
                table: "PreservationPeriods",
                column: "PreservationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_TagRequirementId",
                table: "PreservationPeriods",
                column: "TagRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_CreatedById",
                table: "PreservationRecords",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_PreservedById",
                table: "PreservationRecords",
                column: "PreservedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ModifiedById",
                table: "Projects",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name_ASC",
                table: "Projects",
                column: "Name")
                .Annotation("SqlServer:Include", new[] { "Plant" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Plant_ASC",
                table: "Projects",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Name", "IsClosed", "CreatedAtUtc", "ModifiedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_CreatedById",
                table: "RequirementDefinitions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_ModifiedById",
                table: "RequirementDefinitions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_Plant_ASC",
                table: "RequirementDefinitions",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "IsVoided", "CreatedAtUtc", "ModifiedAtUtc", "SortKey", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_RequirementTypeId",
                table: "RequirementDefinitions",
                column: "RequirementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTypes_CreatedById",
                table: "RequirementTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTypes_ModifiedById",
                table: "RequirementTypes",
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
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_Steps_CreatedById",
                table: "Steps",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_JourneyId",
                table: "Steps",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_ModeId",
                table: "Steps",
                column: "ModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_ModifiedById",
                table: "Steps",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_Plant_ASC",
                table: "Steps",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "SortKey", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Steps_ResponsibleId",
                table: "Steps",
                column: "ResponsibleId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_CreatedById",
                table: "TagFunctionRequirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_ModifiedById",
                table: "TagFunctionRequirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_Plant_ASC",
                table: "TagFunctionRequirements",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_RequirementDefinitionId",
                table: "TagFunctionRequirements",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_TagFunctionId",
                table: "TagFunctionRequirements",
                column: "TagFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_CreatedById",
                table: "TagFunctions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_ModifiedById",
                table: "TagFunctions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_Plant_ASC",
                table: "TagFunctions",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Code", "RegisterCode" });

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_CreatedById",
                table: "TagRequirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_ModifiedById",
                table: "TagRequirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_RequirementDefinitionId",
                table: "TagRequirements",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_TagId",
                table: "TagRequirements",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Calloff_ASC",
                table: "Tags",
                column: "Calloff")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CommPkgNo_ASC",
                table: "Tags",
                column: "CommPkgNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "Description", "CreatedAtUtc", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_McPkgNo_ASC",
                table: "Tags",
                column: "McPkgNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "Description", "CommPkgNo", "CreatedAtUtc", "DisciplineCode", "IsVoided", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedById",
                table: "Tags",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Plant_ASC",
                table: "Tags",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "TagNo" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ProjectId",
                table: "Tags",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_PurchaseOrderNo_ASC",
                table: "Tags",
                column: "PurchaseOrderNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_StepId",
                table: "Tags",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_StorageArea_ASC",
                table: "Tags",
                column: "StorageArea")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagNo_ASC",
                table: "Tags",
                column: "TagNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "Description", "CreatedAtUtc", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldValues");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "TagFunctionRequirements");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "PreservationPeriods");

            migrationBuilder.DropTable(
                name: "TagFunctions");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "PreservationRecords");

            migrationBuilder.DropTable(
                name: "TagRequirements");

            migrationBuilder.DropTable(
                name: "RequirementDefinitions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "RequirementTypes");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "Journeys");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropTable(
                name: "Responsibles");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
