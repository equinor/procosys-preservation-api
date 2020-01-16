using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.AllAvailableTagsQuery
{
    [TestClass]
    public class GetAllAvailableTagsQueryHandlerTests
    {
        private Mock<ITagRepository> _tagRepositoryMock;
        private Mock<ITagApiService> _tagApiServiceMock;
        private Mock<IPlantProvider> _plantProviderMock;
        private IList<ProcosysTagOverview> _apiTags;
        private List<Tag> _repositoryTags;

        [TestInitialize]
        public void Setup()
        {
            _tagRepositoryMock = new Mock<ITagRepository>();
            _tagApiServiceMock = new Mock<ITagApiService>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns("PCS$TESTPLANT");

            _apiTags = new List<ProcosysTagOverview>
            {
                new ProcosysTagOverview
                {
                    CommPkgNo = "CommPkgNo1",
                    Description = "Desc1",
                    Id = 1,
                    McPkgNo = "McPkgNo1",
                    PurchaseOrderNo = "PoNo1",
                    TagNo = "TagNo1"
                },
                new ProcosysTagOverview
                {
                    CommPkgNo = "CommPkgNo2",
                    Description = "Desc2",
                    Id = 2,
                    McPkgNo = "McPkgNo2",
                    PurchaseOrderNo = "PoNo2",
                    TagNo = "TagNo2"
                },
                new ProcosysTagOverview
                {
                    CommPkgNo = "CommPkgNo3",
                    Description = "Desc3",
                    Id = 3,
                    McPkgNo = "McPkgNo3",
                    PurchaseOrderNo = "PoNo3",
                    TagNo = "TagNo3"
                }
            };

            var stepMock = new Mock<Step>();
            var requirementMock = new Mock<Requirement>();
            _repositoryTags = new List<Tag>
            {
                new Tag("", "TagNo1", "", "", "", "", "", "", "","", stepMock.Object, new List<Requirement> {requirementMock.Object }),
                new Tag("", "TagNoNotInApi1", "", "", "", "", "", "", "","", stepMock.Object, new List<Requirement> {requirementMock.Object }),
                new Tag("", "TagNoNotInApi2", "", "", "", "", "", "", "","", stepMock.Object, new List<Requirement> {requirementMock.Object }),
            };
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            _tagApiServiceMock
                .Setup(x => x.GetTags("PCS$TESTPLANT", "ProjectName", "TagNo"))
                .Returns(Task.FromResult(_apiTags));

            var dut = new GetAllAvailableTagsQueryHandler(_tagRepositoryMock.Object, _tagApiServiceMock.Object, _plantProviderMock.Object);
            var query = new GetAllAvailableTagsQuery("ProjectName", "TagNo");

            var result = await dut.Handle(query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfItems()
        {
            _tagApiServiceMock
                .Setup(x => x.GetTags("PCS$TESTPLANT", "ProjectName", "TagNo"))
                .Returns(Task.FromResult(_apiTags));

            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(_repositoryTags));

            var dut = new GetAllAvailableTagsQueryHandler(_tagRepositoryMock.Object, _tagApiServiceMock.Object, _plantProviderMock.Object);
            var query = new GetAllAvailableTagsQuery("ProjectName", "TagNo");

            var result = await dut.Handle(query, default);

            Assert.AreEqual(3, result.Data.Count);
        }

        [TestMethod]
        public async Task Handle_SetsCorrectIsPreservedStatus()
        {
            _tagApiServiceMock
                .Setup(x => x.GetTags("PCS$TESTPLANT", "ProjectName", "TagNo"))
                .Returns(Task.FromResult(_apiTags));

            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(_repositoryTags));

            var dut = new GetAllAvailableTagsQueryHandler(_tagRepositoryMock.Object, _tagApiServiceMock.Object, _plantProviderMock.Object);
            var query = new GetAllAvailableTagsQuery("ProjectName", "TagNo");

            var result = await dut.Handle(query, default);

            Assert.IsTrue(result.Data[0].IsPreserved);
            Assert.IsFalse(result.Data[1].IsPreserved);
            Assert.IsFalse(result.Data[2].IsPreserved);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_WhenTagApiReturnsNull()
        {
            _tagApiServiceMock
                .Setup(x => x.GetTags("PCS$TESTPLANT", "ProjectName", "TagNo"))
                .Returns(Task.FromResult<IList<ProcosysTagOverview>>(null));

            var dut = new GetAllAvailableTagsQueryHandler(_tagRepositoryMock.Object, _tagApiServiceMock.Object, _plantProviderMock.Object);
            var query = new GetAllAvailableTagsQuery("ProjectName", "TagNo");

            var result = await dut.Handle(query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
            Assert.AreEqual(0, result.Data.Count);
        }

        [TestMethod]
        public async Task Handle_ReturnsApiTags_WhenTagRepositoryReturnsNull()
        {
            _tagApiServiceMock
                .Setup(x => x.GetTags("PCS$TESTPLANT", "ProjectName", "TagNo"))
                .Returns(Task.FromResult(_apiTags));

            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult<List<Tag>>(null));

            var dut = new GetAllAvailableTagsQueryHandler(_tagRepositoryMock.Object, _tagApiServiceMock.Object, _plantProviderMock.Object);
            var query = new GetAllAvailableTagsQuery("ProjectName", "TagNo");

            var result = await dut.Handle(query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
            Assert.AreEqual(3, result.Data.Count);
        }
    }
}
