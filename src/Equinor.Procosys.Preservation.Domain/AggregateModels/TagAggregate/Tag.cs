using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Tag : SchemaEntityBase, IAggregateRoot
    {
        private readonly List<Requirement> _requirements = new List<Requirement>();

        public const int TagNoLengthMax = 255;
        public const int ProjectNumberLengthMax = 255;

        public string AreaCode { get; private set; }
        public string CalloffNumber { get; private set; }
        public string CommPkgNumber { get; private set; }
        public string DisciplineCode { get; private set; }
        public bool IsAreaTag { get; private set; }
        public string McPkcNumber { get; private set; }
        public string ProjectNumber { get; private set; }
        public string PurchaseOrderNumber { get; private set; }
        public int StepId { get; private set; }
        public string TagFunctionCode { get; private set; }
        public string TagNo { get; private set; }
        public IReadOnlyCollection<Requirement> Requirements => _requirements.AsReadOnly();

        protected Tag()
            : base(null)
        {
        }

        public Tag(
            string schema,
            string tagNo,
            string projectNumber,
            string areaCode,
            string calloffNumber,
            string disciplineCode,
            string mcPkcNumber,
            string commPkgNumber,
            string purchaseOrderNumber,
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

            TagNo = tagNo;
            ProjectNumber = projectNumber;
            AreaCode = areaCode;
            CalloffNumber = calloffNumber;
            DisciplineCode = disciplineCode;
            McPkcNumber = mcPkcNumber;
            CommPkgNumber = commPkgNumber;
            PurchaseOrderNumber = purchaseOrderNumber;
            TagFunctionCode = tagFunctionCode;
            StepId = step.Id;
            _requirements.AddRange(reqList);
        }

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

            _requirements.Add(requirement);
        }
    }
}
