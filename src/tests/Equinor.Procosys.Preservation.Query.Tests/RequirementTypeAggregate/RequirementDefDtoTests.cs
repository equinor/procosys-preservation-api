using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementDefDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetsProperties()
        {
            var rd = new RequirementDefinitionDto(1, "TitleA", true, 4, 10, new List<FieldDto>());

            Assert.AreEqual(1, rd.Id);
            Assert.AreEqual("TitleA", rd.Title);
            Assert.AreEqual(4, rd.DefaultInterval);
            Assert.AreEqual(10, rd.SortKey);
            Assert.IsTrue(rd.IsVoided);
            Assert.AreEqual(0, rd.Fields.Count());
        }

        [TestMethod]
        public void Constructor_ShouldThrowsException_WhenModeNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new RequirementDefinitionDto(1, "TitleA", true, 4, 10, null)
            );

        [TestMethod]
        public void ConstructorWithFields_ShouldCreateDtoWithFieldsSortedBySortKey()
        {
            var rd = new RequirementDefinitionDto(1, "TitleA", true, 4, 10, new List<FieldDto>
            {
                new FieldDto(1, "", true, FieldType.Info, 1, null, null),
                new FieldDto(2, "", true, FieldType.Info, 90, null, null),
                new FieldDto(3, "", true, FieldType.Info, 5, null, null),
                new FieldDto(4, "", true, FieldType.Info, 10, null, null),
            });

            var dtos = rd.Fields.ToList();
            Assert.AreEqual(4, dtos.Count);
            Assert.AreEqual(1, dtos[0].Id);
            Assert.AreEqual(3, dtos[1].Id);
            Assert.AreEqual(4, dtos[2].Id);
            Assert.AreEqual(2, dtos[3].Id);
        }
    }
}
