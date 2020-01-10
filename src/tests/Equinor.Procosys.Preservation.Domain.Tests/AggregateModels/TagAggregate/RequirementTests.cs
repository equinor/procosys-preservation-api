using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagAggregate
{
    [TestClass]
    public class RequirementTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var rd = new Mock<RequirementDefinition>();
            rd.SetupGet(x => x.Id).Returns(3);

            var req = new Requirement("SchemaA", 24, rd.Object);

            Assert.AreEqual("SchemaA", req.Schema);
            Assert.AreEqual(rd.Object.Id, req.RequirementDefinitionId);
            Assert.IsFalse(req.IsVoided);
        }

        [TestMethod]
        public void ConstructorWithNullRequirementDefinitionThrowsExceptionTest()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Requirement("SchemaA", 4, null)
            );
    }
}
