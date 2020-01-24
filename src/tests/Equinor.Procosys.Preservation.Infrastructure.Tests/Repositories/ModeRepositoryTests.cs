using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class ModeRepositoryTests : RepositoryTestBase
    {
        private const string TestMode = "HOOKUP";
        private const int ModeId = 11;
        private List<Mode> _modes;
        private Mock<DbSet<Mode>> _dbSetMock;
        private Mode _mode;

        private ModeRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Id).Returns(ModeId);
            _mode = new Mode(TestPlant, TestMode);
            _modes = new List<Mode>
            {
                _mode,
                new Mode(TestPlant, "M2"),
                modeMock.Object
            };
            
            _dbSetMock = _modes.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Modes)
                .Returns(_dbSetMock.Object);

            _dut = new ModeRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetAll_Returns3()
        {
            var result = await _dut.GetAllAsync();

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetByTitle_KnownTitle_ReturnsMode()
        {
            var result = await _dut.GetByTitleAsync(TestMode);

            Assert.AreEqual(TestMode, result.Title);
        }

        [TestMethod]
        public async Task GetByTitle_UnknownTitle_ReturnsNull()
        {
            var result = await _dut.GetByTitleAsync("XYZ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIds_KnownId_ReturnsMode()
        {
            var result = await _dut.GetByIdsAsync(new List<int>{ModeId});

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ModeId, result.First().Id);
        }

        [TestMethod]
        public async Task GetByIds_UnknownId_ReturnsEmptyList()
        {
            var result = await _dut.GetByIdsAsync(new List<int>{12672});

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task Exists_KnownId_ReturnsTrue()
        {
            var result = await _dut.Exists(ModeId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Exists_UnknownId_ReturnsFalse()
        {
            var result = await _dut.Exists(416);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetById_KnownId_ReturnsMode()
        {
            var result = await _dut.GetByIdAsync(ModeId);

            Assert.AreEqual(ModeId, result.Id);
        }

        [TestMethod]
        public async Task GetById_UnknownId_ReturnsNull()
        {
            var result = await _dut.GetByIdAsync(2423);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_Mode_CallAddForMode()
        {
            _dut.Add(_mode);

            _dbSetMock.Verify(s => s.Add(_mode), Times.Once);
        }

        [TestMethod]
        public void Remove_Mode_CallRemoveForMode()
        {
            _dut.Remove(_mode);

            _dbSetMock.Verify(s => s.Remove(_mode), Times.Once);
        }
    }
}
