using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;
using PreservationAction = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries
{
    public abstract class GetTagsQueryBase
    {
        protected IQueryable<TaqForQueryDto> CreateQueryableWithFilter(IReadOnlyContext _context, string projectName, Filter filter)
        {
            var nowUtc = TimeService.UtcNow;
            var startOfThisWeekUtc = DateTime.MinValue;
            var startOfNextWeekUtc = DateTime.MinValue;
            var startOfTwoWeeksUtc = DateTime.MinValue;
            if (filter.DueFilters.Any())
            {
                startOfThisWeekUtc = nowUtc.StartOfWeek();
                startOfNextWeekUtc = startOfThisWeekUtc.AddWeeks(1);
                startOfTwoWeeksUtc = startOfThisWeekUtc.AddWeeks(2);

            }
            // No .Include() here. EF do not support .Include together with selecting a projection (dto).
            // If the select-statement select tag so queryable has been of type IQueryable<Tag>, .Include(t => t.Requirements) work fine
            var queryable = from tag in _context.QuerySet<Tag>()
                join project in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals project.Id
                join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                join journey in _context.QuerySet<Journey>() on EF.Property<int>(step, "JourneyId") equals journey.Id
                join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                join responsible in _context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                let anyReqTypeFiltered = (from req in _context.QuerySet<TagRequirement>()
                    join reqDef in _context.QuerySet<RequirementDefinition>() on req.RequirementDefinitionId equals reqDef.Id
                    join reqType in _context.QuerySet<RequirementType>() on EF.Property<int>(reqDef, "RequirementTypeId") equals reqType.Id
                    where EF.Property<int>(req, "TagId") == tag.Id && filter.RequirementTypeIds.Contains(reqType.Id)
                    select reqType.Id).Any()
                let anyOverDueActions = (from a in _context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && !a.ClosedAtUtc.HasValue && a.DueTimeUtc < nowUtc
                    select a.Id).Any()
                let anyOpenActions = (from a in _context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && !a.ClosedAtUtc.HasValue
                    select a.Id).Any()
                let anyClosedActions = (from a in _context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && a.ClosedAtUtc.HasValue
                    select a.Id).Any()
                where project.Name == projectName &&
                      (filter.VoidedFilter == VoidedFilterType.All ||
                       (filter.VoidedFilter == VoidedFilterType.NotVoided && !tag.IsVoided) ||
                       (filter.VoidedFilter == VoidedFilterType.Voided && tag.IsVoided)) &&
                      (!filter.DueFilters.Any() || 
                           (filter.DueFilters.Contains(DueFilterType.OverDue) && tag.NextDueTimeUtc < startOfThisWeekUtc) ||
                           (filter.DueFilters.Contains(DueFilterType.ThisWeek) && tag.NextDueTimeUtc >= startOfThisWeekUtc && tag.NextDueTimeUtc < startOfNextWeekUtc) ||
                           (filter.DueFilters.Contains(DueFilterType.NextWeek) && tag.NextDueTimeUtc >= startOfNextWeekUtc && tag.NextDueTimeUtc < startOfTwoWeeksUtc)) &&
                      (!filter.ActionStatus.HasValue || 
                           (filter.ActionStatus == ActionStatus.HasOpen && anyOpenActions) ||
                           (filter.ActionStatus == ActionStatus.HasClosed && anyClosedActions) ||
                           (filter.ActionStatus == ActionStatus.HasOverDue && anyOverDueActions)) &&
                      (!filter.PreservationStatus.HasValue || 
                            tag.Status == filter.PreservationStatus.Value) &&
                      (string.IsNullOrEmpty(filter.TagNoStartsWith) ||
                            tag.TagNo.StartsWith(filter.TagNoStartsWith)) &&
                      (string.IsNullOrEmpty(filter.CommPkgNoStartsWith) ||
                            tag.CommPkgNo.StartsWith(filter.CommPkgNoStartsWith)) &&
                      (string.IsNullOrEmpty(filter.McPkgNoStartsWith) ||
                            tag.McPkgNo.StartsWith(filter.McPkgNoStartsWith)) &&
                      (string.IsNullOrEmpty(filter.PurchaseOrderNoStartsWith) ||
                            tag.PurchaseOrderNo.StartsWith(filter.PurchaseOrderNoStartsWith)) &&
                      (string.IsNullOrEmpty(filter.StorageAreaStartsWith) ||
                            tag.StorageArea.StartsWith(filter.StorageAreaStartsWith)) &&
                      (string.IsNullOrEmpty(filter.CallOffStartsWith) ||
                            tag.Calloff.StartsWith(filter.CallOffStartsWith)) &&
                      (!filter.RequirementTypeIds.Any() || 
                            anyReqTypeFiltered) &&
                      (!filter.TagFunctionCodes.Any() ||
                            filter.TagFunctionCodes.Contains(tag.TagFunctionCode)) &&
                      (!filter.AreaCodes.Any() || 
                            filter.AreaCodes.Contains(tag.AreaCode)) &&
                      (!filter.DisciplineCodes.Any() || 
                            filter.DisciplineCodes.Contains(tag.DisciplineCode)) &&
                      (!filter.ResponsibleIds.Any() || 
                            filter.ResponsibleIds.Contains(responsible.Id)) &&
                      (!filter.JourneyIds.Any() || 
                            filter.JourneyIds.Contains(journey.Id)) &&
                      (!filter.ModeIds.Any() || 
                            filter.ModeIds.Contains(mode.Id)) &&
                      (!filter.StepIds.Any() || 
                            filter.StepIds.Contains(step.Id))
                select new TaqForQueryDto
                {
                    AreaCode = tag.AreaCode,
                    AnyOverDueActions = anyOverDueActions,
                    AnyOpenActions = anyOpenActions,
                    AnyClosedActions = anyClosedActions,
                    Calloff = tag.Calloff,
                    CommPkgNo = tag.CommPkgNo,
                    Description = tag.Description,
                    DisciplineCode = tag.DisciplineCode,
                    IsVoided = tag.IsVoided,
                    JourneyId = journey.Id,
                    ModeTitle = mode.Title,
                    McPkgNo = tag.McPkgNo,
                    NextDueTimeUtc = tag.NextDueTimeUtc,
                    PurchaseOrderNo = tag.PurchaseOrderNo,
                    ResponsibleCode = responsible.Code,
                    ResponsibleDescription = responsible.Description,
                    Status = tag.Status,
                    StepId = step.Id,
                    StorageArea = tag.StorageArea,
                    TagFunctionCode = tag.TagFunctionCode,
                    TagId = tag.Id,
                    TagNo = tag.TagNo,
                    TagType = tag.TagType,
                    RowVersion = tag.RowVersion
                };
            return queryable;
        }

        protected IQueryable<TaqForQueryDto> AddSorting(Sorting sorting, IQueryable<TaqForQueryDto> queryable)
        {
            switch (sorting.Direction)
            {
                default:
                    switch (sorting.Property)
                    {
                        case SortingProperty.Due:
                            queryable = queryable.OrderBy(dto => dto.Status).ThenBy(dto => dto.NextDueTimeUtc);
                            break;
                        case SortingProperty.Status:
                            queryable = queryable.OrderBy(dto => dto.Status);
                            break;
                        case SortingProperty.TagNo:
                            queryable = queryable.OrderBy(dto => dto.TagNo);
                            break;
                        case SortingProperty.Description:
                            queryable = queryable.OrderBy(dto => dto.Description);
                            break;
                        case SortingProperty.Responsible:
                            queryable = queryable.OrderBy(dto => dto.ResponsibleCode);
                            break;
                        case SortingProperty.Mode:
                            queryable = queryable.OrderBy(dto => dto.ModeTitle);
                            break;
                        case SortingProperty.PO:
                            queryable = queryable.OrderBy(dto => dto.Calloff);
                            break;
                        case SortingProperty.Area:
                            queryable = queryable.OrderBy(dto => dto.AreaCode);
                            break;
                        case SortingProperty.Discipline:
                            queryable = queryable.OrderBy(dto => dto.DisciplineCode);
                            break;
                        default:
                            queryable = queryable.OrderBy(dto => dto.TagId);
                            break;
                    }

                    break;
                case SortingDirection.Desc:
                    switch (sorting.Property)
                    {
                        case SortingProperty.Due:
                            queryable = queryable.OrderByDescending(dto => dto.Status).ThenByDescending(dto => dto.NextDueTimeUtc);
                            break;
                        case SortingProperty.Status:
                            queryable = queryable.OrderByDescending(dto => dto.Status);
                            break;
                        case SortingProperty.TagNo:
                            queryable = queryable.OrderByDescending(dto => dto.TagNo);
                            break;
                        case SortingProperty.Description:
                            queryable = queryable.OrderByDescending(dto => dto.Description);
                            break;
                        case SortingProperty.Responsible:
                            queryable = queryable.OrderByDescending(dto => dto.ResponsibleCode);
                            break;
                        case SortingProperty.Mode:
                            queryable = queryable.OrderByDescending(dto => dto.ModeTitle);
                            break;
                        case SortingProperty.PO:
                            queryable = queryable.OrderByDescending(dto => dto.Calloff);
                            break;
                        case SortingProperty.Area:
                            queryable = queryable.OrderByDescending(dto => dto.AreaCode);
                            break;
                        case SortingProperty.Discipline:
                            queryable = queryable.OrderByDescending(dto => dto.DisciplineCode);
                            break;
                        default:
                            queryable = queryable.OrderByDescending(dto => dto.TagId);
                            break;
                    }
                    break;
            }

            return queryable;
        }
    }
}
