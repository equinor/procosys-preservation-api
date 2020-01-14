﻿using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class GetAllRequirementTypesQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var q = new GetAllRequirementTypesQuery(true);
            Assert.IsTrue(q.IncludeVoided);
        }
    }
}
