using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Tag : PlantEntityBase, ICreationAuditable, IModificationAuditable, IVoidable
    {
        private readonly List<TagRequirement> _requirements = new();
        private readonly List<Action> _actions = new();
        private readonly List<TagAttachment> _attachments = new();
        private bool _isVoided;
        private bool _isVoidedInSource;
        private bool _isDeletedInSource;

        public const int TagNoLengthMax = 255;
        public const int TagFunctionCodeLengthMax = 255;
        public const int AreaCodeLengthMax = 255;
        public const int AreaDescriptionLengthMax = 255;
        public const int DisciplineCodeLengthMax = 255;
        public const int DisciplineDescriptionLengthMax = 255;
        public const int DescriptionLengthMax = 255;
        public const int RemarkLengthMax = 255;
        public const int StorageAreaLengthMax = 255;
        public const int PurchaseOrderNoLengthMax = 20;
        public const int McPkgNoLengthMax = 30;
        public const int CommPkgNoLengthMax = 30;
        public const int CalloffLengthMax = 30;

        protected Tag()
            : base(null)
        {
        }

        public Tag(
            string plant,
            TagType tagType,
            Guid? proCoSysGuid,
            string tagNo,
            string description,
            Step step, 
            IEnumerable<TagRequirement> requirements)
            : base(plant)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }
            var reqList = requirements.ToList();
            if (reqList.Count < 1)
            {
                throw new Exception("Must have at least one requirement");
            }
                        
            if (step.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {step.Plant} to item in {plant}");
            }

            var requirement = reqList.FirstOrDefault(r => r.Plant != Plant);
            if (requirement != null)
            {
                throw new ArgumentException($"Can't relate item in {requirement.Plant} to item in {plant}");
            }

            if (tagType == TagType.Standard && (!proCoSysGuid.HasValue || proCoSysGuid.Value == Guid.Empty))
            {
                throw new ArgumentException($"ProCoSysGuid for {tagType} can't be null or Guid.Empty");
            }

            if (tagType != TagType.Standard && proCoSysGuid.HasValue)
            {
                throw new ArgumentException($"ProCoSysGuid for {tagType} mus't be null");
            }

            TagType = tagType;
            Status = PreservationStatus.NotStarted;
            TagNo = tagNo;
            ProCoSysGuid = proCoSysGuid;
            Description = description;
            StepId = step.Id;
            IsInSupplierStep = step.IsSupplierStep;
            _requirements.AddRange(reqList);
            ObjectGuid = Guid.NewGuid();
            AddDomainEvent(new TagCreatedEvent(plant, ObjectGuid));
        }

        public Guid ObjectGuid { get; private set; }
        public Guid? ProCoSysGuid { get; set; }
        public PreservationStatus Status { get; private set; }
        public string AreaCode { get; private set; }
        public string AreaDescription { get; private set; }
        public string Calloff { get; set; }
        public string CommPkgNo { get; set; }
        public Guid? CommPkgProCoSysGuid { get; set; }
        public string DisciplineCode { get; private set; }
        public string DisciplineDescription { get; private set; }
        public TagType TagType { get; private set; }
        public string McPkgNo { get; set; }
        public Guid? McPkgProCoSysGuid { get; set; }
        public string Description { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
        public int StepId { get; private set; }
        public bool IsInSupplierStep { get; private set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; private set; }
        public IReadOnlyCollection<TagRequirement> Requirements => _requirements.AsReadOnly();
        public IReadOnlyCollection<Action> Actions => _actions.AsReadOnly();
        public IReadOnlyCollection<TagAttachment> Attachments => _attachments.AsReadOnly();

        public bool IsVoided
        {
            get => _isVoided;
            set
            {
                // do nothing if already set
                if (_isVoided == value)
                {
                    return;
                }

                if (_isVoidedInSource && !value)
                {
                    throw new Exception("Can't unvoid when voided in source system!");
                }

                _isVoided = value;
                if (_isVoided)
                {
                    AddDomainEvent(new TagVoidedEvent(Plant, ObjectGuid));
                }
                else
                {
                    AddDomainEvent(new TagUnvoidedEvent(Plant, ObjectGuid));
                }
            }
        }

        public bool IsVoidedInSource
        {
            get => _isVoidedInSource;
            set
            {
                // do nothing if already set
                if (_isVoidedInSource == value)
                {
                    return;
                }
                _isVoidedInSource = value;

                // if tag is (un)voided in source (Main), the preservation tag should be (un)voided too
                IsVoided = value;

                if (_isVoidedInSource)
                {
                    AddDomainEvent(new TagVoidedInSourceEvent(Plant, ObjectGuid));
                }
                else
                {
                    AddDomainEvent(new TagUnvoidedInSourceEvent(Plant, ObjectGuid));
                }
            }
        }

        public bool IsDeletedInSource
        {
            get => _isDeletedInSource;
            set
            {
                if (_isDeletedInSource && !value)
                {
                    // this is an Undelete, which don't make sence
                    throw new Exception("Changing IsDeletedInSource from true to false is not supported!");
                }

                // do nothing if already set
                if (_isDeletedInSource == value)
                {
                    return;
                }

                _isDeletedInSource = value;

                // Make sure to also set IsVoidedInSource when setting _isDeletedInSource
                IsVoidedInSource = value;

                AddDomainEvent(new TagDeletedInSourceEvent(Plant, ObjectGuid));
            }
        }

        public DateTime? NextDueTimeUtc { get; private set;  }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void SetArea(string code, string description)
        {
            AreaCode = code;
            AreaDescription = description;
        }

        public void SetDiscipline(string code, string description)
        {
            DisciplineCode = code;
            DisciplineDescription = description;
        }

        public void SetStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            StepId = step.Id;
            IsInSupplierStep = step.IsSupplierStep;
        }

        public void AddRequirement(TagRequirement tagRequirement)
        {
            if (tagRequirement == null)
            {
                throw new ArgumentNullException(nameof(tagRequirement));
            }
            
            if (tagRequirement.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {tagRequirement.Plant} to item in {Plant}");
            }
            
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            if (_requirements.Any(r => r.RequirementDefinitionId == tagRequirement.RequirementDefinitionId))
            {
                throw new ArgumentException($"{nameof(Tag)} {TagNo} already has a requirement with definition {tagRequirement.RequirementDefinitionId}");
            }

            _requirements.Add(tagRequirement);
            if (Status == PreservationStatus.Active || Status == PreservationStatus.InService)
            {
                tagRequirement.StartPreservation();
            }
            UpdateNextDueTimeUtc();
            AddDomainEvent(new TagRequirementAddedEvent(tagRequirement.Plant, ObjectGuid, tagRequirement.RequirementDefinitionId));
        }
                
        public void RemoveRequirement(int requirementId, string requirementRowVersion)
        {
            var tagRequirement = Requirements.Single(r => r.Id == requirementId);
            
            if (!tagRequirement.IsVoided)
            {
                throw new Exception($"{nameof(TagRequirement)} must be voided before delete");
            }

            if (tagRequirement.IsInUse)
            {
                throw new Exception($"Requirement on {nameof(Tag)} {Id} can not be deleted. Status = {Status}");
            }

            tagRequirement.SetRowVersion(requirementRowVersion);
            
            _requirements.Remove(tagRequirement);
            
            UpdateNextDueTimeUtc();
            AddDomainEvent(new TagRequirementDeletedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId));
        }

        public void AddAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            
            if (action.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {action.Plant} to item in {Plant}");
            }

            _actions.Add(action);
            AddDomainEvent(new ActionAddedEvent(action.Plant, ObjectGuid, action.Title));
        }

        public Action CloseAction(int actionId, Person closedBy, DateTime closedAtUtc, string rowVersion)
        {
            var action = Actions.Single(a => a.Id == actionId);

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action.Close(closedAtUtc, closedBy);
            action.SetRowVersion(rowVersion);
            AddDomainEvent(new ActionClosedEvent(action.Plant, ObjectGuid));

            return action;
        }

        public void AddAttachment(TagAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }
            
            if (attachment.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {attachment.Plant} to item in {Plant}");
            }

            _attachments.Add(attachment);
        }
        
        public void RemoveAttachment(TagAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }
            
            if (attachment.Plant != Plant)
            {
                throw new ArgumentException($"Can't remove item in {attachment.Plant} from item in {Plant}");
            }

            _attachments.Remove(attachment);
        }

        public void StartPreservation()
        {
            if (!IsReadyToBeStarted())
            {
                throw new Exception($"Preservation on {nameof(Tag)} {Id} can not start. Status = {Status}");
            }

            if (Status != PreservationStatus.InService)
            {
                foreach (var requirement in Requirements.Where(r => !r.IsVoided))
                {
                    requirement.StartPreservation();
                }

                UpdateNextDueTimeUtc();
            }
            Status = PreservationStatus.Active;
            AddDomainEvent(new PreservationStartedEvent(Plant, ObjectGuid));
        }

        public void UndoStartPreservation()
        {
            if (!IsReadyToUndoStarted())
            {
                throw new Exception($"Can not undo start preservation on {nameof(Tag)} {Id}. Status = {Status}");
            }
            foreach (var requirement in Requirements)
            {
                requirement.UndoStartPreservation();
            }

            Status = PreservationStatus.NotStarted;
            UpdateNextDueTimeUtc();
            AddDomainEvent(new UndoPreservationStartedEvent(Plant, ObjectGuid));
        }

        public void SetInService()
        {
            if (!IsReadyToBeSetInService())
            {
                throw new Exception($"Can not set in service on {nameof(Tag)} {Id}. Status = {Status}");
            }

            Status = PreservationStatus.InService;
            AddDomainEvent(new PreservationSetInServiceEvent(Plant, ObjectGuid));
        }

        public void CompletePreservation(Journey journey)
        {
            if (!IsReadyToBeCompleted(journey))
            {
                throw new Exception($"Preservation on {nameof(Tag)} {Id} can not be completed. Status = {Status}");
            }
            foreach (var requirement in Requirements.Where(r => !r.IsVoided))
            {
                requirement.CompletePreservation();
            }

            Status = PreservationStatus.Completed;
            NextDueTimeUtc = null;
            AddDomainEvent(new PreservationCompletedEvent(Plant, ObjectGuid));
        }

        public bool IsReadyToBePreserved()
            => Status == PreservationStatus.Active && 
               FirstUpcomingRequirement() != null;

        public bool IsReadyToBeRescheduled()
            => Status == PreservationStatus.Active || Status == PreservationStatus.InService;

        public bool IsReadyToBeEdited()
            => Status != PreservationStatus.Completed;

        public bool IsReadyToBeDuplicated()
            => TagType == TagType.PreArea || TagType == TagType.SiteArea;

        public void Preserve(Person preservedBy)
            => Preserve(preservedBy, false);
                
        public void Preserve(Person preservedBy, int requirementId)
        {
            var requirement = RequirementsDueToCurrentStep().Single(r => r.Id == requirementId);
            var nextDueInWeeks = requirement.GetNextDueInWeeks();
            var activePeriod = requirement.ActivePeriod;

            requirement.Preserve(preservedBy, false);
            UpdateNextDueTimeUtc();

            var preservationRecordGuid = activePeriod.PreservationRecord.ObjectGuid;

            AddDomainEvent(new TagRequirementPreservedEvent(Plant, ObjectGuid, requirement.RequirementDefinitionId, nextDueInWeeks, preservationRecordGuid));
        }

        public void BulkPreserve(Person preservedBy)
            => Preserve(preservedBy, true);

        public IEnumerable<TagRequirement> GetUpComingRequirements()
        {
            var upComingRequirements = OrderedRequirements()
                .Where(r => r.IsReadyAndDueToBePreserved());
            return upComingRequirements;
        }

        public IOrderedEnumerable<TagRequirement> OrderedRequirements(bool includeVoided = false, bool includeAllUsages = false)
            => RequirementsDueToCurrentStep(includeVoided, includeAllUsages)
                .OrderBy(r => r.NextDueTimeUtc);

        public bool IsReadyToBeTransferred(Journey journey)
        {
            if (journey == null)
            {
                throw new ArgumentNullException(nameof(journey));
            }

            return (Status == PreservationStatus.NotStarted || 
                    Status == PreservationStatus.Active || 
                    Status == PreservationStatus.InService)
                   && FollowsAJourney && journey.HasNextStep(StepId);
        }

        public bool IsReadyToBeCompleted(Journey journey)
        {
            if (journey == null)
            {
                throw new ArgumentNullException(nameof(journey));
            }

            return (Status == PreservationStatus.Active || Status == PreservationStatus.InService) && 
                   (!FollowsAJourney || !journey.HasNextStep(StepId));
        }
        
        public void UpdateStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            if (step.Id == StepId)
            {
                return;
            }

            if (TagType == TagType.PoArea && !step.IsSupplierStep)
            {
                throw new Exception($"Step for {TagType.PoArea} tags need to be a step for supplier");
            }

            var fromStepId = StepId;
        
            SetStep(step);
            
            AddDomainEvent(new StepChangedEvent(Plant, ObjectGuid, fromStepId, step.Id));
        }

        public void Transfer(Journey journey)
        {
            if (!IsReadyToBeTransferred(journey))
            {
                throw new Exception($"{nameof(Tag)} {Id} can not be transferred");
            }

            var fromStep = journey.Steps.Single(s => s.Id == StepId);

            SetStep(journey.GetNextStep(StepId));

            var toStep = journey.Steps.Single(s => s.Id == StepId);

            AddDomainEvent(new TransferredManuallyEvent(Plant, ObjectGuid, fromStep.Title, toStep.Title));
        }

        public void AutoTransfer(Journey journey, AutoTransferMethod autoTransferMethod)
        {
            if (!IsReadyToBeTransferred(journey))
            {
                throw new Exception($"{nameof(Tag)} {Id} can not be transferred");
            }

            var fromStep = journey.Steps.Single(s => s.Id == StepId);

            if (fromStep.AutoTransferMethod != autoTransferMethod)
            {
                throw new Exception($"{nameof(Tag)} {Id} can not be auto transferred with method {autoTransferMethod}. Current step {fromStep.Id} has method {fromStep.AutoTransferMethod}");
            }

            SetStep(journey.GetNextStep(StepId));

            var toStep = journey.Steps.Single(s => s.Id == StepId);

            AddDomainEvent(new TransferredAutomaticallyEvent(Plant, ObjectGuid, fromStep.Title, toStep.Title, autoTransferMethod));
        }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }

        public bool IsReadyToBeStarted() => Status != PreservationStatus.Active && Requirements.Any(r => !r.IsVoided);

        public bool IsReadyToUndoStarted() => Status == PreservationStatus.Active;

        public bool IsReadyToBeSetInService() => Status == PreservationStatus.Active;

        public TagAttachment GetAttachmentByFileName(string fileName) => _attachments.SingleOrDefault(a => a.FileName.ToUpper() == fileName.ToUpper());

        public bool FollowsAJourney => TagType.FollowsAJourney();

        public IEnumerable<TagRequirement> RequirementsDueToCurrentStep(bool includeVoided = false, bool includeAllUsages = false)
            => Requirements
                .Where(r => includeVoided || !r.IsVoided)
                .Where(r => includeAllUsages || r.Usage == RequirementUsage.ForAll || 
                            (IsInSupplierStep && r.Usage == RequirementUsage.ForSuppliersOnly) ||
                            (!IsInSupplierStep && r.Usage == RequirementUsage.ForOtherThanSuppliers));
        
        public void UpdateRequirement(int requirementId, bool isVoided, int intervalWeeks, string requirementRowVersion)
        {
            var tagRequirement = Requirements.Single(r => r.Id == requirementId);

            if (tagRequirement.IsVoided != isVoided)
            {
                if (isVoided)
                {
                    tagRequirement.IsVoided = true;
                    AddDomainEvent(new TagRequirementVoidedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId));
                }
                else
                {
                    tagRequirement.IsVoided = false;
                    if (Status == PreservationStatus.Active && !tagRequirement.HasActivePeriod)
                    {
                        tagRequirement.StartPreservation();
                    }

                    AddDomainEvent(new TagRequirementUnvoidedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId));
                }
                UpdateNextDueTimeUtc();
            }

            ChangeInterval(tagRequirement.Id, intervalWeeks);

            tagRequirement.SetRowVersion(requirementRowVersion);
        }

        public void ChangeInterval(int requirementId, int intervalWeeks)
        {
            var tagRequirement = Requirements.Single(r => r.Id == requirementId);
            if (tagRequirement.IntervalWeeks == intervalWeeks)
            {
                return;
            }

            var fromInterval = tagRequirement.IntervalWeeks;

            tagRequirement.UpdateInterval(intervalWeeks);

            var toInterval = tagRequirement.IntervalWeeks;

            UpdateNextDueTimeUtc();
            AddDomainEvent(new IntervalChangedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId, fromInterval, toInterval));
        }
        
        public bool IsAreaTag()
        {
            var areaTagTypes = new List<TagType> {TagType.PoArea, TagType.PreArea, TagType.SiteArea};
            return areaTagTypes.Contains(TagType);
        }
        
        public void Reschedule(int weeks, RescheduledDirection direction, string comment)
        {
            if (!IsReadyToBeRescheduled())
            {
                throw new Exception($"{nameof(Tag)} {Id} is not ready to be rescheduled");
            }
            foreach (var requirement in Requirements.Where(r => !r.IsVoided))
            {
                requirement.Reschedule(weeks, direction);
            }
        
            UpdateNextDueTimeUtc();

            AddDomainEvent(new RescheduledEvent(Plant, ObjectGuid, weeks, direction, comment));
        }

        private void Preserve(Person preservedBy, bool bulkPreserved)
        {
            if (!IsReadyToBePreserved())
            {
                throw new Exception($"{nameof(Tag)} {Id} is not ready to be preserved");
            }

            foreach (var requirement in GetUpComingRequirements())
            {
                var nextDueInWeeks = requirement.GetNextDueInWeeks();
                var activePeriod = requirement.ActivePeriod;
                
                requirement.Preserve(preservedBy, bulkPreserved);

                var preservationRecordGuid = activePeriod.PreservationRecord.ObjectGuid;

                AddDomainEvent(new TagRequirementPreservedEvent(Plant, ObjectGuid, requirement.RequirementDefinitionId, nextDueInWeeks, preservationRecordGuid));
            }
        
            UpdateNextDueTimeUtc();
        }

        private TagRequirement FirstUpcomingRequirement()
            => GetUpComingRequirements().FirstOrDefault();

        private void UpdateNextDueTimeUtc()
            => NextDueTimeUtc = OrderedRequirements().FirstOrDefault()?.NextDueTimeUtc;

        public void Rename(string newTagNo)
        {
            if (string.IsNullOrWhiteSpace(newTagNo))
            {
                throw new ArgumentNullException($"{nameof(newTagNo)}");
            }

            TagNo = newTagNo;
        }
    }
}
