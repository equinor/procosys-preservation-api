using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Repositories
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
            _mode = new Mode(TestPlant, TestMode, false);
            _modes = new List<Mode>
            {
                _mode,
                new Mode(TestPlant, "M2", false),
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
        public async Task GetAll_ShouldReturnAllItems()
        {
            var result = await _dut.GetAllAsync();

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetByIds_KnownId_ShouldReturnMode()
        {
            var result = await _dut.GetByIdsAsync(new List<int> { ModeId });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ModeId, result.First().Id);
        }

        [TestMethod]
        public async Task GetByIds_UnknownId_ShouldReturnEmptyList()
        {
            var result = await _dut.GetByIdsAsync(new List<int> { 12672 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task Exists_KnownId_ShouldReturnTrue()
        {
            var result = await _dut.Exists(ModeId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Exists_UnknownId_ShouldReturnFalse()
        {
            var result = await _dut.Exists(416);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetById_KnownId_ShouldReturnMode()
        {
            var result = await _dut.GetByIdAsync(ModeId);

            Assert.AreEqual(ModeId, result.Id);
        }

        [TestMethod]
        public async Task GetById_UnknownId_ShouldReturnNull()
        {
            var result = await _dut.GetByIdAsync(2423);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_Mode_ShouldCallAddForMode()
        {
            _dut.Add(_mode);

            _dbSetMock.Verify(s => s.Add(_mode), Times.Once);
        }

        [TestMethod]
        public void Remove_WhenModeIsVoided_ShouldCallRemoveForMode()
        {
            // Arrange
            _mode.IsVoided = true;

            // Act
            _dut.Remove(_mode);

            // Arrange
            _dbSetMock.Verify(s => s.Remove(_mode), Times.Once);
        }

        [TestMethod]
        public void Remove_WhenModeIsNotVoided_ShouldThrowException()
            => Assert.ThrowsException<Exception>(() => _dut.Remove(_mode));
    }
}
