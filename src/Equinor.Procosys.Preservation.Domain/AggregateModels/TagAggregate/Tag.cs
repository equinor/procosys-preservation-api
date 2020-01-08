using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Tag : SchemaEntityBase, IAggregateRoot
    {
        public const int DescriptionLengthMax = 1000;
        public const int TagNoLengthMax = 255;
        public const int ProjectNoLengthMax = 255;

        public string AreaCode { get; private set; }
        public string CalloffNumber { get; private set; }
        public string CommPkgNumber { get; private set; }
        public string Description { get; private set; }
        public string DisciplineCode { get; private set; }
        public bool IsAreaTag { get; private set; }
        public string McPkcNumber { get; private set; }
        public DateTime NextDueTime { get; private set; }
        public string ProjectNumber { get; private set; }
        public string PurchaseOrderNumber { get; private set; }
        public int StepId { get; private set; }
        public string TagFunctionCode { get; private set; }
        public string TagNo { get; private set; }

        protected Tag()
            : base(null)
        {
        }

        public Tag(
            string schema,
            string tagNumber,
            string projectNumber,
            string description,
            string areaCode,
            string calloffNumber,
            string disciplineCode,
            string mcPkcNumber,
            string purchaseOrderNumber,
            string tagFunctionCode,
            Step step
            )
            : base(schema)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            TagNo = tagNumber;
            ProjectNumber = projectNumber;
            Description = description;
            AreaCode = areaCode;
            CalloffNumber = calloffNumber;
            DisciplineCode = disciplineCode;
            McPkcNumber = mcPkcNumber;
            PurchaseOrderNumber = purchaseOrderNumber;
            TagFunctionCode = tagFunctionCode;
            StepId = step.Id;
        }

        public void SetStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            StepId = step.Id;
        }
    }
}
