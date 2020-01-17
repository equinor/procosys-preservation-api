using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class TagRepositoryTests
    {
        private List<Tag> _tags;
        private Mock<DbSet<Tag>> _dbSetMock;
        private ContextHelper _contextHelper;

        [TestInitialize]
        public void Setup()
        {
            var step = new Mock<Step>().Object;
            var requirements = new List<Requirement>
            {
                new Mock<Requirement>().Object,
                new Mock<Requirement>().Object,
                new Mock<Requirement>().Object,
            };

            _tags = new List<Tag>
            {
                new Tag("PCS$TESTPLANT", "TagNo1", "ProjectNo1", "Area1", "CallOffNo1", "DisciplineCode1", "McPkgNo1", "CommPkgNo1", "PoNo1", "TagFunctionCode1", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo2", "ProjectNo1", "Area1", "CallOffNo1", "DisciplineCode1", "McPkgNo1", "CommPkgNo1", "PoNo1", "TagFunctionCode1", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo3", "ProjectNo1", "Area1", "CallOffNo1", "DisciplineCode1", "McPkgNo1", "CommPkgNo1", "PoNo1", "TagFunctionCode1", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo3", "ProjectNo2", "Area2", "CallOffNo2", "DisciplineCode2", "McPkgNo2", "CommPkgNo2", "PoNo2", "TagFunctionCode2", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo4", "ProjectNo3", "Area3", "CallOffNo3", "DisciplineCode3", "McPkgNo3", "CommPkgNo3", "PoNo3", "TagFunctionCode3", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo5", "ProjectNo4", "Area4", "CallOffNo4", "DisciplineCode4", "McPkgNo4", "CommPkgNo4", "PoNo4", "TagFunctionCode4", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo6", "ProjectNo5", "Area5", "CallOffNo5", "DisciplineCode5", "McPkgNo5", "CommPkgNo5", "PoNo5", "TagFunctionCode5", step, requirements),
            };
            
            _dbSetMock = _tags.AsQueryable().BuildMockDbSet();

            _contextHelper = new ContextHelper();
            _contextHelper
                .ContextMock
                .Setup(x => x.Tags)
                .Returns(_dbSetMock.Object);
        }

        [TestMethod]
        public async Task GetAllByProjectNo_Returns3TagsWithinProject_Async()
        {
            var dut = new TagRepository(_contextHelper.ContextMock.Object);

            var result = await dut.GetAllByProjectNoAsync("ProjectNo1");

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetAllByProjectNo_Returns0TagsWithinProject_Async()
        {
            var dut = new TagRepository(_contextHelper.ContextMock.Object);

            var result = await dut.GetAllByProjectNoAsync("ProjectNoNotInDbSet");

            Assert.AreEqual(0, result.Count);
        }

        // TODO: Write tests for all methods
    }
}
