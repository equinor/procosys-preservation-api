using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class ResponsibleRepositoryTests : RepositoryTestBase
    {
        private const string ResponsibleCode = "A";

        private ResponsibleRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var responsible = new Responsible(TestPlant, ResponsibleCode, "Title1");

            var responsibles = new List<Responsible> { responsible};

            var dbSetMock = responsibles.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Responsibles)
                .Returns(dbSetMock.Object);

            _dut = new ResponsibleRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetByCode_ReturnsResponsible_WhenResponsibleExists()
        {
            var result = await _dut.GetByCodeAsync(ResponsibleCode);

            // Assert
            Assert.AreEqual(ResponsibleCode, result.Code);
        }
    }
}
