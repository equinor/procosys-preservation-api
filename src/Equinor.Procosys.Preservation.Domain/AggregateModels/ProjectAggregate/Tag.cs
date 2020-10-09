using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Tag : PlantEntityBase, ICreationAuditable, IModificationAuditable, IVoidable
    {
        private readonly List<TagRequirement> _requirements = new List<TagRequirement>();
        private readonly List<Action> _actions = new List<Action>();
        private readonly List<TagAttachment> _attachments = new List<TagAttachment>();
        private bool _isVoided;

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

            TagType = tagType;
            Status = PreservationStatus.NotStarted;
            TagNo = tagNo;
            Description = description;
            StepId = step.Id;
            IsInSupplierStep = step.IsSupplierStep;
            _requirements.AddRange(reqList);
            ObjectGuid = Guid.NewGuid();
            AddDomainEvent(new TagCreatedEvent(plant, ObjectGuid));
        }

        public Guid ObjectGuid { get; private set; }
        public PreservationStatus Status { get; private set; }
        public string AreaCode { get; private set; }
        public string AreaDescription { get; private set; }
        public string Calloff { get; set; }
        public string CommPkgNo { get; set; }
        public string DisciplineCode { get; private set; }
        public string DisciplineDescription { get; private set; }
        public TagType TagType { get; private set; }
        public string McPkgNo { get; set; }
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

        public void AddRequirement(TagRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            if (requirement.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {requirement.Plant} to item in {Plant}");
            }
            
            if (_requirements.Any(r => r.RequirementDefinitionId == requirement.RequirementDefinitionId))
            {
                throw new ArgumentException($"{nameof(Tag)} {TagNo} already has a requirement with definition {requirement.RequirementDefinitionId}");
            }

            _requirements.Add(requirement);
            if (Status == PreservationStatus.Active)
            {
                requirement.StartPreservation();
            }
            UpdateNextDueTimeUtc();
            AddDomainEvent(new RequirementAddedEvent(requirement.Plant, ObjectGuid, requirement.RequirementDefinitionId));
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

        public void CloseAction(int actionId, Person closedBy, DateTime closedAtUtc, string rowVersion)
        {
            var action = Actions.Single(a => a.Id == actionId);

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action.Close(closedAtUtc, closedBy);
            action.SetRowVersion(rowVersion);
            AddDomainEvent(new ActionClosedEvent(action.Plant, ObjectGuid));
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
            foreach (var requirement in Requirements.Where(r => !r.IsVoided))
            {
                requirement.StartPreservation();
            }

            Status = PreservationStatus.Active;
            UpdateNextDueTimeUtc();
            AddDomainEvent(new PreservationStartedEvent(Plant, ObjectGuid));
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

            AddDomainEvent(new RequirementPreservedEvent(Plant, ObjectGuid, requirement.RequirementDefinitionId, nextDueInWeeks, preservationRecordGuid));
        }

        public void BulkPreserve(Person preservedBy)
            => Preserve(preservedBy, true);

        public IEnumerable<TagRequirement> GetUpComingRequirements()
        {
            var GetUpComingRequirements = OrderedRequirements()
                .Where(r => r.IsReadyAndDueToBePreserved());
            return GetUpComingRequirements;
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

            return Status == PreservationStatus.Active && FollowsAJourney && journey.GetNextStep(StepId) != null;
        }

        public bool IsReadyToBeCompleted(Journey journey)
        {
            if (journey == null)
            {
                throw new ArgumentNullException(nameof(journey));
            }

            return Status == PreservationStatus.Active && 
                   (!FollowsAJourney || FollowsAJourney && journey.GetNextStep(StepId) == null);
        }
        
        public void UpdateStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
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

        public bool IsReadyToBeStarted()
            => Status == PreservationStatus.NotStarted && Requirements.Any(r => !r.IsVoided);

        public TagAttachment GetAttachmentByFileName(string fileName) => _attachments.SingleOrDefault(a => a.FileName.ToUpper() == fileName.ToUpper());

        public bool FollowsAJourney => TagType == TagType.Standard || TagType == TagType.PreArea;

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
                    AddDomainEvent(new RequirementVoidedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId));
                }
                else
                {
                    tagRequirement.IsVoided = false;
                    AddDomainEvent(new RequirementUnvoidedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId));
                }
            }

            if (tagRequirement.IntervalWeeks != intervalWeeks)
            {
                ChangeInterval(tagRequirement.Id, intervalWeeks);
            }

            tagRequirement.SetRowVersion(requirementRowVersion);
        }

        public void ChangeInterval(int requirementId, int intervalWeeks)
        {
            var tagRequirement = Requirements.Single(r => r.Id == requirementId);

            var fromInterval = tagRequirement.IntervalWeeks;

            tagRequirement.SetUpdatedInterval(intervalWeeks);

            var toInterval = tagRequirement.IntervalWeeks;

            UpdateNextDueTimeUtc();
            AddDomainEvent(new IntervalChangedEvent(Plant, ObjectGuid, tagRequirement.RequirementDefinitionId, fromInterval, toInterval));
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

                AddDomainEvent(new RequirementPreservedEvent(Plant, ObjectGuid, requirement.RequirementDefinitionId, nextDueInWeeks, preservationRecordGuid));
            }
        
            UpdateNextDueTimeUtc();
        }

        private TagRequirement FirstUpcomingRequirement()
            => GetUpComingRequirements().FirstOrDefault();

        private void UpdateNextDueTimeUtc()
            => NextDueTimeUtc = OrderedRequirements().FirstOrDefault()?.NextDueTimeUtc;
    }
}
