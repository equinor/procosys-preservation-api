﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.Preserve
{
    [TestClass]
    public class PreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId = 7;
        private const int RequirementId = 71;
        private const int PreservedById = 81;
        private const int Interval = 2;

        private Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private DateTime _utcNow;
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<IPersonRepository> _personRepoMock;
        private Mock<ICurrentUserProvider> _currentUserProvider;
        private Mock<ITimeService> _timeServiceMock;
        private PreserveCommand _command;
        private Tag _tag;
        private Requirement _requirement;

        private PreserveCommandHandler _dut;
        private PreservationPeriod _initialPreservationPeriod;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            var rdMock = new Mock<RequirementDefinition>();

            var requirementMock = new Mock<Requirement>("", Interval, rdMock.Object);
            requirementMock.SetupGet(r => r.Id).Returns(RequirementId);
            _requirement = requirementMock.Object;

            _tag = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _requirement
            });
            _currentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUserProvider
                .Setup(x => x.GetCurrentUser())
                .Returns(_currentUserOid);
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagByTagIdAsync(TagId)).Returns(Task.FromResult(_tag));
            _personRepoMock = new Mock<IPersonRepository>();
            _personRepoMock
                .Setup(x => x.GetByOidAsync(It.Is<Guid>(x => x == _currentUserOid)))
                .Returns(Task.FromResult(new Person(_currentUserOid, "Test", "User")));
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_utcNow);
            _command = new PreserveCommand(TagId, RequirementId);

            _tag.StartPreservation(_utcNow.AddDays(-1));
            _initialPreservationPeriod = _requirement.PreservationPeriods.Single();

            _dut = new PreserveCommandHandler(_projectRepoMock.Object, _personRepoMock.Object, _timeServiceMock.Object, UnitOfWorkMock.Object, _currentUserProvider.Object);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldPreserveRequirement()
        {
            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(Interval);
            Assert.AreEqual(expectedNextDueTimeUtc, _requirement.NextDueTimeUtc);
            Assert.IsNotNull(_initialPreservationPeriod.PreservationRecord);
        }


        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}
