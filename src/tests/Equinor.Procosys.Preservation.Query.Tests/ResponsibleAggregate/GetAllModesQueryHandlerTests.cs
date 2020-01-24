using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.ResponsibleAggregate
{
    [TestClass]
    public class GetAllResponsiblesQueryHandlerTests
    {
        private Mock<IResponsibleRepository> _responsibleRepoMock;
        private Responsible _responsible;

        private GetAllResponsiblesQueryHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            _responsible = new Responsible("S", "Responsible");

            _responsibleRepoMock = new Mock<IResponsibleRepository>();
            _responsibleRepoMock.Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult(new List<Responsible>
                {
                    _responsible,
                    new Mock<Responsible>().Object,
                    new Mock<Responsible>().Object
                }));
            
            _dut = new GetAllResponsiblesQueryHandler(_responsibleRepoMock.Object);
        }

        [TestMethod]
        public async Task HandleGetAllResponsiblesQueryHandler_ShouldReturnResponsibles()
        {
            var result = await _dut.Handle(new GetAllResponsiblesQuery(), default);

            var responsibles = result.Data.ToList();
            
            Assert.AreEqual(3, responsibles.Count);
            Assert.AreEqual(_responsible.Name, responsibles.First().Name);
        }
    }
}
