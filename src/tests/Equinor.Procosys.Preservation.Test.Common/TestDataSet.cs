using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Test.Common
{
    public class TestDataSet
    {
        public readonly int IntervalWeeks = 2;
        public readonly string StdTagPrefix = "StdTagNo";
        public readonly string SiteTagPrefix = "SiteTagNo";
        public readonly string CallOffPrefix = "CO";
        public readonly string AreaPrefix = "AREA";
        public readonly string DisciplinePrefix = "DI";
        public readonly string McPkgPrefix = "MC";
        public readonly string CommPkgPrefix = "COMM";
        public readonly string PoPrefix = "PO";
        public readonly string TagFunctionPrefix = "TF";

        public Project Project1 { get; set; }
        public Project Project2 { get; set; }
        public Mode Mode1 { get; set; }
        public Responsible Responsible1 { get; set; }
        public Mode Mode2 { get; set; }
        public Responsible Responsible2 { get; set; }
        public Journey Journey1With2Steps { get; set; }
        public Journey Journey2With1Step { get; set; }
        public RequirementType ReqType1 { get; set; }
        public RequirementType ReqType2 { get; set; }
        public Person CurrentUser { get; set; }
    }
}
