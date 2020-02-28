using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Tag : SchemaEntityBase, ICreationAuditable, IModificationAuditable
    {
        private readonly List<Requirement> _requirements = new List<Requirement>();
        private readonly List<Action> _actions = new List<Action>();

        public const int TagNoLengthMax = 255;
        public const int AreaCodeLengthMax = 255;
        public const int DisciplineCodeLengthMax = 255;
        public const int DescriptionLengthMax = 255;
        public const int RemarkLengthMax = 255;

        protected Tag()
            : base(null)
        {
        }

        public Tag(
            string schema,
            TagType tagType,
            string tagNo,
            string description,
            string areaCode,
            string calloff,
            string disciplineCode,
            string mcPkgNo,
            string commPkgNo,
            string purchaseOrderNo,
            string remark,
            string tagFunctionCode,
            Step step, 
            IEnumerable<Requirement> requirements)
            : base(schema)
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
                        
            if (step.Schema != schema)
            {
                throw new ArgumentException($"Can't relate item in {step.Schema} to item in {schema}");
            }

            var requirement = reqList.FirstOrDefault(r => r.Schema != Schema);
            if (requirement != null)
            {
                throw new ArgumentException($"Can't relate item in {requirement.Schema} to item in {schema}");
            }

            TagType = tagType;
            Status = PreservationStatus.NotStarted;
            TagNo = tagNo;
            Description = description;
            AreaCode = areaCode;
            Calloff = calloff;
            DisciplineCode = disciplineCode;
            McPkgNo = mcPkgNo;
            CommPkgNo = commPkgNo;
            PurchaseOrderNo = purchaseOrderNo;
            Remark = remark;
            TagFunctionCode = tagFunctionCode;
            StepId = step.Id;
            _requirements.AddRange(reqList);
        }

        public PreservationStatus Status { get; private set; }
        public string AreaCode { get; private set; }
        public string Calloff { get; private set; }
        public string CommPkgNo { get; private set; }
        public string DisciplineCode { get; private set; }
        public TagType TagType { get; private set; }
        public string McPkgNo { get; private set; }
        public string Description { get; private set; }
        public string PurchaseOrderNo { get; private set; }
        public string Remark { get; private set; }
        public int StepId { get; private set; }
        public string TagFunctionCode { get; private set; }
        public string TagNo { get; private set; }
        public IReadOnlyCollection<Requirement> Requirements => _requirements.AsReadOnly();
        public IReadOnlyCollection<Action> Actions => _actions.AsReadOnly();
        public bool IsVoided { get; private set; }
        public DateTime? NextDueTimeUtc => OrderedRequirements().FirstOrDefault()?.NextDueTimeUtc;

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void SetStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            StepId = step.Id;
        }

        public void AddRequirement(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            if (requirement.Schema != Schema)
            {
                throw new ArgumentException($"Can't relate item in {requirement.Schema} to item in {Schema}");
            }

            _requirements.Add(requirement);
        }

        public void AddAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            
            if (action.Schema != Schema)
            {
                throw new ArgumentException($"Can't relate item in {action.Schema} to item in {Schema}");
            }

            _actions.Add(action);
        }

        public void StartPreservation(DateTime startedAtUtc)
        {
            if (!IsReadyToBeStarted())
            {
                throw new Exception($"Preservation on {nameof(Tag)} {Id} can not start. Status = {Status}");
            }
            foreach (var requirement in Requirements.Where(r => !r.IsVoided))
            {
                requirement.StartPreservation(startedAtUtc);
            }

            Status = PreservationStatus.Active;
        }

        public bool IsReadyToBePreserved(DateTime currentTimeUtc)
            => Status == PreservationStatus.Active && 
               FirstUpcomingRequirement(currentTimeUtc) != null;

        public void Preserve(DateTime preservedAtUtc, Person preservedBy)
            => Preserve(preservedAtUtc, preservedBy, false);

        public void BulkPreserve(DateTime preservedAtUtc, Person preservedBy)
            => Preserve(preservedAtUtc, preservedBy, true);

        public IEnumerable<Requirement> GetUpComingRequirements(DateTime currentTimeUtc)
        {
            var GetUpComingRequirements = OrderedRequirements()
                .Where(r => r.IsReadyAndDueToBePreserved(currentTimeUtc));
            return GetUpComingRequirements;
        }

        public IOrderedEnumerable<Requirement> OrderedRequirements()
            => Requirements
                .Where(r => !r.IsVoided)
                .OrderBy(r => r.NextDueTimeUtc);

        public bool IsReadyToBeTransferred(Journey journey)
        {
            if (journey == null)
            {
                throw new ArgumentNullException(nameof(journey));
            }

            return Status == PreservationStatus.Active && TagType != TagType.SiteArea && journey.GetNextStep(StepId) != null;
        }

        public void Transfer(Journey journey)
        {
            if (!IsReadyToBeTransferred(journey))
            {
                throw new Exception($"{nameof(Tag)} {Id} can not be transferred");
            }

            SetStep(journey.GetNextStep(StepId));
        }

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} is not UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }

        public void SetModified(DateTime modifiedAtUtc, Person modifiedBy)
        {
            if (modifiedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(modifiedAtUtc)} is not UTC");
            }

            ModifiedAtUtc = modifiedAtUtc;
            ModifiedById = modifiedBy.Id;
        }

        private bool IsReadyToBeStarted()
            => Status == PreservationStatus.NotStarted && Requirements.Any(r => !r.IsVoided);

        private void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved)
        {
            if (!IsReadyToBePreserved(preservedAtUtc))
            {
                throw new Exception($"{nameof(Tag)} {Id} is not ready to be preserved");
            }

            foreach (var requirement in GetUpComingRequirements(preservedAtUtc))
            {
                requirement.Preserve(preservedAtUtc, preservedBy, bulkPreserved);
            }
        }

        private Requirement FirstUpcomingRequirement(DateTime currentTimeUtc)
            => GetUpComingRequirements(currentTimeUtc).FirstOrDefault();
    }
}
