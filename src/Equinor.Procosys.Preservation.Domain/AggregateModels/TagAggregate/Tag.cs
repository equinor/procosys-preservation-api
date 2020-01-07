using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Tag : SchemaEntityBase, IAggregateRoot
    {
        public const int DescriptionLengthMax = 1000;
        public const int TagNoLengthMax = 255;
        public const int ProjectNoLengthMax = 255;

        public string Description { get; private set; }
        public string McPkcNumber { get; private set; }
        public string TagFunctionCode { get; private set; }
        public string AreaCode { get; private set; }
        public string DisciplineCode { get; private set; }
        public string PurchaseOrderNumber { get; private set; }
        public string CalloffNumber { get; private set; }
        public bool IsAreaTag { get; private set; }
        public string ProjectNo { get; private set; }
        public string TagNo { get; private set; }
        public int StepId { get; private set; }
        public int TagFunctionId { get; private set; }
        public DateTime NextDueTime { get; private set; }

        protected Tag()
            : base(null)
        {
        }

        public Tag(
            string schema,
            string tagNumber,
            string projectNumber,
            Step step,
            string description,
            string mcPkcNumber,
            string tagFunctionCode,
            string areaCode,
            string disciplineCode,
            string purchaseOrderNumber,
            string calloffNumber
            )
            : base(schema)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            TagNo = tagNumber;
            ProjectNo = projectNumber;
            StepId = step.Id;
            Description = description;
            McPkcNumber = mcPkcNumber;
            TagFunctionCode = tagFunctionCode;
            AreaCode = areaCode;
            DisciplineCode = disciplineCode;
            PurchaseOrderNumber = purchaseOrderNumber;
            CalloffNumber = calloffNumber;
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
