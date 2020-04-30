﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeDtoTests
    {
        private readonly List<FieldDto> _fieldsDtos = new List<FieldDto>();

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RequirementTypeDto(1, "CodeA", "TitleA", true, 10, new List<RequirementDefinitionDto>(), 12345);

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(10, dut.SortKey);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(0, dut.RequirementDefinitions.Count());
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDefinitionsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new RequirementTypeDto(1, "CodeA", "TitleA", true, 10, null, 12345)
            );

        [TestMethod]
        public void ConstructorWithRequirementDefinitionsNotNeedInput_ShouldSortRequirementDefinitionsBySortKey()
        {
            var dut = new RequirementTypeDto(1, "", "", true, 10, new List<RequirementDefinitionDto>
            {
                new RequirementDefinitionDto(1, "", false, 4, 999, false, _fieldsDtos),
                new RequirementDefinitionDto(2, "", false, 4, 5, false, _fieldsDtos),
                new RequirementDefinitionDto(3, "", false, 4, 500, false, _fieldsDtos),
                new RequirementDefinitionDto(4, "", false, 4, 10, false, _fieldsDtos),
            },
                12345);

            var requirementDefinitions = dut.RequirementDefinitions.ToList();
            Assert.AreEqual(4, requirementDefinitions.Count);
            Assert.AreEqual(2, requirementDefinitions[0].Id);
            Assert.AreEqual(4, requirementDefinitions[1].Id);
            Assert.AreEqual(3, requirementDefinitions[2].Id);
            Assert.AreEqual(1, requirementDefinitions[3].Id);
        }

        [TestMethod]
        public void ConstructorWithRequirementDefinitionsNeedingInput_ShouldSortRequirementDefinitionsBySortKey()
        {
            var dut = new RequirementTypeDto(1, "", "", true, 10, new List<RequirementDefinitionDto>
            {
                new RequirementDefinitionDto(1, "", false, 4, 999, true, _fieldsDtos),
                new RequirementDefinitionDto(2, "", false, 4, 5, true, _fieldsDtos),
                new RequirementDefinitionDto(3, "", false, 4, 500, true, _fieldsDtos),
                new RequirementDefinitionDto(4, "", false, 4, 10, true, _fieldsDtos),
            },
                12346);

            var requirementDefinitions = dut.RequirementDefinitions.ToList();
            Assert.AreEqual(4, requirementDefinitions.Count);
            Assert.AreEqual(2, requirementDefinitions[0].Id);
            Assert.AreEqual(4, requirementDefinitions[1].Id);
            Assert.AreEqual(3, requirementDefinitions[2].Id);
            Assert.AreEqual(1, requirementDefinitions[3].Id);
        }

        [TestMethod]
        public void ConstructorWithRequirementDefinitionsBothNeedInputAndNotNeedInput_ShouldSortRequirementDefinitionsByNeedingInputThenSortKey()
        {
            var dut = new RequirementTypeDto(1, "", "", true, 10, new List<RequirementDefinitionDto>
            {
                new RequirementDefinitionDto(1, "", false, 4, 999, true, _fieldsDtos),
                new RequirementDefinitionDto(2, "", false, 4, 5, true, _fieldsDtos),
                new RequirementDefinitionDto(3, "", false, 4, 500, true, _fieldsDtos),
                new RequirementDefinitionDto(4, "", false, 4, 10, true, _fieldsDtos),
                new RequirementDefinitionDto(5, "", false, 4, 999, false, _fieldsDtos),
                new RequirementDefinitionDto(6, "", false, 4, 5, false, _fieldsDtos),
                new RequirementDefinitionDto(7, "", false, 4, 500, false, _fieldsDtos),
                new RequirementDefinitionDto(8, "", false, 4, 10, false, _fieldsDtos),
            },
                123457);

            var requirementDefinitions = dut.RequirementDefinitions.ToList();
            Assert.AreEqual(8, requirementDefinitions.Count);
            Assert.AreEqual(6, requirementDefinitions[0].Id);
            Assert.AreEqual(8, requirementDefinitions[1].Id);
            Assert.AreEqual(7, requirementDefinitions[2].Id);
            Assert.AreEqual(5, requirementDefinitions[3].Id);
            Assert.AreEqual(2, requirementDefinitions[4].Id);
            Assert.AreEqual(4, requirementDefinitions[5].Id);
            Assert.AreEqual(3, requirementDefinitions[6].Id);
            Assert.AreEqual(1, requirementDefinitions[7].Id);
        }
    }
}
