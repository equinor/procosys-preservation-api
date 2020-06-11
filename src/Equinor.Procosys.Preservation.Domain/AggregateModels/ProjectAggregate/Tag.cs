using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Tag : PlantEntityBase, ICreationAuditable, IModificationAuditable
    {
        private readonly List<TagRequirement> _requirements = new List<TagRequirement>();
        private readonly List<Action> _actions = new List<Action>();
        private readonly List<TagAttachment> _attachments = new List<TagAttachment>();

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
        }

        public PreservationStatus Status { get; private set; }
        public string AreaCode { get; private set; }
        public string AreaDescription { get; private set; }
        public string Calloff { get; set; }
        public string CommPkgNo { get; set; }
        public string DisciplineCode { get; private set; }
        public string DisciplineDescription { get; private set; }
        public TagType TagType { get; private set; }
        public string McPkgNo { get; set; }
        public string Description { get; private set; }
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
        public bool IsVoided { get; private set; }
        public DateTime? NextDueTimeUtc { get; private set;  }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

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
            UpdateNextDueTimeUtc();
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
        }

        public bool IsReadyToBePreserved()
            => Status == PreservationStatus.Active && 
               FirstUpcomingRequirement() != null;

        public void Preserve(Person preservedBy)
            => Preserve(preservedBy, false);
                
        public void Preserve(Person preservedBy, int requirementId)
        {
            var requirement = ActiveRequirementsDueToCurrentStep().Single(r => r.Id == requirementId);
            requirement.Preserve(preservedBy, false);
            UpdateNextDueTimeUtc();
        }

        public void BulkPreserve(Person preservedBy)
            => Preserve(preservedBy, true);

        public IEnumerable<TagRequirement> GetUpComingRequirements()
        {
            var GetUpComingRequirements = OrderedRequirements()
                .Where(r => r.IsReadyAndDueToBePreserved());
            return GetUpComingRequirements;
        }

        public IOrderedEnumerable<TagRequirement> OrderedRequirements()
            => ActiveRequirementsDueToCurrentStep().OrderBy(r => r.NextDueTimeUtc);

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

        public void Transfer(Journey journey)
        {
            if (!IsReadyToBeTransferred(journey))
            {
                throw new Exception($"{nameof(Tag)} {Id} can not be transferred");
            }

            SetStep(journey.GetNextStep(StepId));
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

        private void Preserve(Person preservedBy, bool bulkPreserved)
        {
            if (!IsReadyToBePreserved())
            {
                throw new Exception($"{nameof(Tag)} {Id} is not ready to be preserved");
            }

            foreach (var requirement in GetUpComingRequirements())
            {
                requirement.Preserve(preservedBy, bulkPreserved);
            }
        
            UpdateNextDueTimeUtc();
        }

        public IEnumerable<TagRequirement> ActiveRequirementsDueToCurrentStep()
            => Requirements
                .Where(r => !r.IsVoided)
                .Where(r => r.Usage == RequirementUsage.ForAll || 
                            (IsInSupplierStep && r.Usage == RequirementUsage.ForSuppliersOnly) ||
                            (!IsInSupplierStep && r.Usage == RequirementUsage.ForOtherThanSuppliers));

        private TagRequirement FirstUpcomingRequirement()
            => GetUpComingRequirements().FirstOrDefault();

        private void UpdateNextDueTimeUtc()
            => NextDueTimeUtc = OrderedRequirements().FirstOrDefault()?.NextDueTimeUtc;
    }
}
