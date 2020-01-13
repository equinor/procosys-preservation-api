using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeDtoTests
    {
        private FieldDto _info;
        private FieldDto _number;
        private FieldDto _cb;
        private FieldDto _attachment;


        [TestInitialize]
        public void Setup()
        {
            _info = new FieldDto(1, "", "", true, true, FieldType.Info, 1);
            _number = new FieldDto(1, "", "", true, true, FieldType.Number, 1);
            _cb = new FieldDto(1, "", "", true, true, FieldType.CheckBox, 1);
            _attachment = new FieldDto(1, "", "", true, true, FieldType.Attachment, 1);
        }

        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var rt = new RequirementTypeDto(1, "CodeA", "TitleA", true, 10, new List<RequirementDefinitionDto>());

            Assert.AreEqual(1, rt.Id);
            Assert.AreEqual("CodeA", rt.Code);
            Assert.AreEqual("TitleA", rt.Title);
            Assert.AreEqual(10, rt.SortKey);
            Assert.IsTrue(rt.IsVoided);
            Assert.AreEqual(0, rt.RequirementDefinitions.Count());
        }

        [TestMethod]
        public void ConstructorWithNullModeThrowsExceptionTest()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new RequirementTypeDto(1, "CodeA", "TitleA", true, 10, null)
            );

        [TestMethod]
        public void DefinitionsNotNeedingInputShouldBeSortedBySortKey()
        {
            var rt = new RequirementTypeDto(1, "", "", true, 10, new List<RequirementDefinitionDto>
            {
                new RequirementDefinitionDto(1, "", false, 4, 999, new List<FieldDto>{ _info}),
                new RequirementDefinitionDto(2, "", false, 4, 5, new List<FieldDto>{ _info}),
                new RequirementDefinitionDto(3, "", false, 4, 500, new List<FieldDto>{ _info}),
                new RequirementDefinitionDto(4, "", false, 4, 10, new List<FieldDto>{ _info}),
            });

            var dtos = rt.RequirementDefinitions.ToList();
            Assert.AreEqual(4, dtos.Count);
            Assert.AreEqual(2, dtos[0].Id);
            Assert.AreEqual(4, dtos[1].Id);
            Assert.AreEqual(3, dtos[2].Id);
            Assert.AreEqual(1, dtos[3].Id);
        }

        [TestMethod]
        public void DefinitionsNeedingInputShouldBeSortedBySortKey()
        {
            var rt = new RequirementTypeDto(1, "", "", true, 10, new List<RequirementDefinitionDto>
            {
                new RequirementDefinitionDto(1, "", false, 4, 999, new List<FieldDto>{ _cb}),
                new RequirementDefinitionDto(2, "", false, 4, 5, new List<FieldDto>{ _attachment}),
                new RequirementDefinitionDto(3, "", false, 4, 500, new List<FieldDto>{ _attachment}),
                new RequirementDefinitionDto(4, "", false, 4, 10, new List<FieldDto>{ _number}),
            });

            var dtos = rt.RequirementDefinitions.ToList();
            Assert.AreEqual(4, dtos.Count);
            Assert.AreEqual(2, dtos[0].Id);
            Assert.AreEqual(4, dtos[1].Id);
            Assert.AreEqual(3, dtos[2].Id);
            Assert.AreEqual(1, dtos[3].Id);
        }

        [TestMethod]
        public void DefinitionShouldFirstBeSortedByNotNeedingInputThenBySortKey()
        {
            var rt = new RequirementTypeDto(1, "", "", true, 10, new List<RequirementDefinitionDto>
            {
                new RequirementDefinitionDto(1, "", false, 4, 999, new List<FieldDto>{ _cb}),
                new RequirementDefinitionDto(2, "", false, 4, 5, new List<FieldDto>{ _attachment}),
                new RequirementDefinitionDto(3, "", false, 4, 500, new List<FieldDto>{ _attachment}),
                new RequirementDefinitionDto(4, "", false, 4, 10, new List<FieldDto>{ _number}),
                new RequirementDefinitionDto(5, "", false, 4, 999, new List<FieldDto>{ _info}),
                new RequirementDefinitionDto(6, "", false, 4, 5, new List<FieldDto>{ _info}),
                new RequirementDefinitionDto(7, "", false, 4, 500, new List<FieldDto>{ _info}),
                new RequirementDefinitionDto(8, "", false, 4, 10, new List<FieldDto>{ _info}),
            });

            var dtos = rt.RequirementDefinitions.ToList();
            Assert.AreEqual(8, dtos.Count);
            Assert.AreEqual(6, dtos[0].Id);
            Assert.AreEqual(8, dtos[1].Id);
            Assert.AreEqual(7, dtos[2].Id);
            Assert.AreEqual(5, dtos[3].Id);
            Assert.AreEqual(2, dtos[4].Id);
            Assert.AreEqual(4, dtos[5].Id);
            Assert.AreEqual(3, dtos[6].Id);
            Assert.AreEqual(1, dtos[7].Id);
        }
    }
}
