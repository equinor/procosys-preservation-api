using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.GetAllRequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetAllRequirementTypes
{
    [TestClass]
    public class RequirementDefDtoTests
    {
        private const string _rowVersion = "AAAAAAAAABA=";

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RequirementDefinitionDto(1, "TitleA", true, 4, RequirementUsage.ForAll, 10, false, new List<FieldDto>(), _rowVersion);

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(4, dut.DefaultIntervalWeeks);
            Assert.AreEqual(RequirementUsage.ForAll, dut.Usage);
            Assert.AreEqual(10, dut.SortKey);
            Assert.IsTrue(dut.IsVoided);
            Assert.IsFalse(dut.NeedsUserInput);
            Assert.AreEqual(0, dut.Fields.Count());
            Assert.AreEqual(_rowVersion, dut.RowVersion);

        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenModeNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new RequirementDefinitionDto(1, "TitleA", true, 4, RequirementUsage.ForAll, 10, true, null, _rowVersion)
            );

        [TestMethod]
        public void ConstructorWithFields_ShouldCreateDtoWithFieldsSortedBySortKey()
        {
            var dut = new RequirementDefinitionDto(1, "TitleA", true, 4, RequirementUsage.ForAll, 10, true, new List<FieldDto>
            {
                new FieldDto(1, "", true, FieldType.Info, 1, null, null, _rowVersion),
                new FieldDto(2, "", true, FieldType.Info, 90, null, null, _rowVersion),
                new FieldDto(3, "", true, FieldType.Info, 5, null, null, _rowVersion),
                new FieldDto(4, "", true, FieldType.Info, 10, null, null, _rowVersion),
            }, _rowVersion);

            var dtos = dut.Fields.ToList();
            Assert.AreEqual(4, dtos.Count);
            Assert.AreEqual(1, dtos[0].Id);
            Assert.AreEqual(_rowVersion, dtos[0].RowVersion);
            Assert.AreEqual(3, dtos[1].Id);
            Assert.AreEqual(4, dtos[2].Id);
            Assert.AreEqual(2, dtos[3].Id);
        }
    }
}
