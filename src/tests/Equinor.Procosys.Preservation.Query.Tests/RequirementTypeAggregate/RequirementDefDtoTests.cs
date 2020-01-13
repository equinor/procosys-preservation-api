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
        public void ConstructorSetsPropertiesTest()
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
        public void ConstructorWithNullModeThrowsExceptionTest()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new RequirementDefinitionDto(1, "TitleA", true, 4, 10, null)
            );

        [TestMethod]
        public void FieldsShouldBeSortedBySortKey()
        {
            var rd = new RequirementDefinitionDto(1, "TitleA", true, 4, 10, new List<FieldDto>
            {
                new FieldDto(1, "", "", true, true, FieldType.Info, 1),
                new FieldDto(2, "", "", true, true, FieldType.Info, 90),
                new FieldDto(3, "", "", true, true, FieldType.Info, 5),
                new FieldDto(4, "", "", true, true, FieldType.Info, 10),
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
