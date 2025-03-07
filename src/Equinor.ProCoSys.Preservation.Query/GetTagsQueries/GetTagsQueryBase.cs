﻿using System;
using System.Linq;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;
using PreservationAction = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries
{
    public abstract class GetTagsQueryBase
    {
        protected IQueryable<TagForQueryDto> CreateQueryableWithFilter(IReadOnlyContext context, string projectName, Filter filter, DateTime utcNow)
        {
            var startOfThisWeekUtc = DateTime.MinValue;
            var startOfNextWeekUtc = DateTime.MinValue;
            var startOfTwoWeeksUtc = DateTime.MinValue;
            var startOfThreeWeeksUtc = DateTime.MinValue;
            var startOfFourWeeksUtc = DateTime.MinValue;
            if (filter.DueFilters.Any())
            {
                startOfThisWeekUtc = utcNow.StartOfWeek();
                startOfNextWeekUtc = startOfThisWeekUtc.AddWeeks(1);
                startOfTwoWeeksUtc = startOfThisWeekUtc.AddWeeks(2);
                startOfThreeWeeksUtc = startOfThisWeekUtc.AddWeeks(3);
                startOfFourWeeksUtc = startOfThisWeekUtc.AddWeeks(4);
            }

            // No .Include() here. EF do not support .Include together with selecting a projection (dto).
            // If the select-statement select tag so queryable has been of type IQueryable<Tag>, .Include(t => t.Requirements) work fine
            var queryable = from tag in context.QuerySet<Tag>()
                join project in context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals project.Id
                join step in context.QuerySet<Step>() on tag.StepId equals step.Id
                join journey in context.QuerySet<Journey>() on EF.Property<int>(step, "JourneyId") equals journey.Id
                join mode in context.QuerySet<Mode>() on step.ModeId equals mode.Id
                join responsible in context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                let anyReqTypeFiltered = (from req in context.QuerySet<TagRequirement>()
                    join reqDef in context.QuerySet<RequirementDefinition>() on req.RequirementDefinitionId equals reqDef.Id
                    join reqType in context.QuerySet<RequirementType>() on EF.Property<int>(reqDef, "RequirementTypeId") equals reqType.Id
                    where
                        !req.IsVoided &&
                        EF.Property<int>(req, "TagId") == tag.Id &&
                        filter.RequirementTypeIds.Contains(reqType.Id)
                    select reqType.Id).Any()
                let anyOverdueActions = (from a in context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && !a.ClosedAtUtc.HasValue && a.DueTimeUtc < utcNow
                    select a.Id).Any()
                let anyOpenActions = (from a in context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && !a.ClosedAtUtc.HasValue
                    select a.Id).Any()
                let anyClosedActions = (from a in context.QuerySet<PreservationAction>()
                    where EF.Property<int>(a, "TagId") == tag.Id && a.ClosedAtUtc.HasValue
                    select a.Id).Any()
                where project.Name == projectName &&
                      (filter.VoidedFilter == VoidedFilterType.All ||
                       (filter.VoidedFilter == VoidedFilterType.NotVoided && !tag.IsVoided) ||
                       (filter.VoidedFilter == VoidedFilterType.Voided && tag.IsVoided)) &&
                      (!filter.DueFilters.Any() || 
                           (filter.DueFilters.Contains(DueFilterType.Overdue) && tag.NextDueTimeUtc < startOfThisWeekUtc) ||
                           (filter.DueFilters.Contains(DueFilterType.ThisWeek) && tag.NextDueTimeUtc >= startOfThisWeekUtc && tag.NextDueTimeUtc < startOfNextWeekUtc) ||
                           (filter.DueFilters.Contains(DueFilterType.NextWeek) && tag.NextDueTimeUtc >= startOfNextWeekUtc && tag.NextDueTimeUtc < startOfTwoWeeksUtc) ||
                           (filter.DueFilters.Contains(DueFilterType.WeekPlusTwo) && tag.NextDueTimeUtc >= startOfTwoWeeksUtc && tag.NextDueTimeUtc < startOfThreeWeeksUtc) ||
                           (filter.DueFilters.Contains(DueFilterType.WeekPlusThree) && tag.NextDueTimeUtc >= startOfThreeWeeksUtc && tag.NextDueTimeUtc < startOfFourWeeksUtc)) &&
                      (!filter.ActionStatus.Any() || 
                           (filter.ActionStatus.Contains(ActionStatus.HasOpen) && anyOpenActions) ||
                           (filter.ActionStatus.Contains(ActionStatus.HasClosed) && anyClosedActions) ||
                           (filter.ActionStatus.Contains(ActionStatus.HasOverdue) && anyOverdueActions)) &&
                      (!filter.PreservationStatus.Any() || 
                            filter.PreservationStatus.Contains(tag.Status)) &&
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
                select new TagForQueryDto
                {
                    AreaCode = tag.AreaCode,
                    AnyOverdueActions = anyOverdueActions,
                    AnyOpenActions = anyOpenActions,
                    AnyClosedActions = anyClosedActions,
                    CalloffNo = tag.Calloff,
                    CommPkgNo = tag.CommPkgNo,
                    Description = tag.Description,
                    DisciplineCode = tag.DisciplineCode,
                    IsVoided = tag.IsVoided,
                    JourneyId = journey.Id,
                    ModeTitle = mode.Title,
                    McPkgNo = tag.McPkgNo,
                    NextDueTimeUtc = tag.NextDueTimeUtc,
                    PurchaseOrderNo = tag.PurchaseOrderNo,
                    Remark = tag.Remark,
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

        protected IQueryable<TagForQueryDto> AddSorting(Sorting sorting, IQueryable<TagForQueryDto> queryable)
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
                            queryable = queryable.OrderBy(dto => dto.CalloffNo);
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
                            queryable = queryable.OrderByDescending(dto => dto.CalloffNo);
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
