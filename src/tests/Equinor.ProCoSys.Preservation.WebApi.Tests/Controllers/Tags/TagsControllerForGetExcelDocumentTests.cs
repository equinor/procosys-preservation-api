using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags;
using Equinor.ProCoSys.Preservation.WebApi.Excel;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class TagsControllerForGetExcelDocumentTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private Mock<IExcelConverter> _excelConverterMock;
        private readonly FilterDto _filterDto = new FilterDto{ProjectName = "PX"};
        private readonly SortingDto _sortingDto = new SortingDto();
        private readonly ExportDto _exportDto = new ExportDto(null, null);
        private TagsController _dut;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetTagsForExportQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<ExportDto>(_exportDto) as Result<ExportDto>));
            _excelConverterMock = new Mock<IExcelConverter>();
            _excelConverterMock.Setup(x => x.Convert(_exportDto)).Returns(new MemoryStream());
            _dut = new TagsController(_mediatorMock.Object, _excelConverterMock.Object);
        }

        [TestMethod]
        public async Task GetExcelDocument_ShouldSendCommand()
        {
            await _dut.ExportTagsWithHistoryToExcel("", true, _filterDto, _sortingDto);
            _mediatorMock.Verify(x => x.Send(It.IsAny<GetTagsForExportQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetExcelDocument_ShouldCreateCorrectQuery()
        {
            GetTagsForExportQuery _createdQuery = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetTagsForExportQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<ExportDto>(_exportDto) as Result<ExportDto>))
                .Callback<IRequest<Result<ExportDto>>, CancellationToken>((request, cancellationToken) =>
                {
                    _createdQuery = request as GetTagsForExportQuery;
                });

            _filterDto.ProjectName = "ProjectName";
            _filterDto.ActionStatus = ActionStatus.HasClosed;
            _filterDto.AreaCodes = new List<string>{"A"};
            _filterDto.CallOffStartsWith = "B";
            _filterDto.CommPkgNoStartsWith = "C";
            _filterDto.DisciplineCodes = new List<string>{"D"};
            _filterDto.DueFilters = new List<DueFilterType>{DueFilterType.NextWeek};
            _filterDto.JourneyIds = new List<int>{1};
            _filterDto.McPkgNoStartsWith = "E";
            _filterDto.ModeIds = new List<int>{2};
            _filterDto.PreservationStatus = new List<PreservationStatus>{PreservationStatus.Active};
            _filterDto.PurchaseOrderNoStartsWith = "F";
            _filterDto.RequirementTypeIds = new List<int>{3};
            _filterDto.ResponsibleIds = new List<int>{4};
            _filterDto.StepIds = new List<int>{5};
            _filterDto.StorageAreaStartsWith = "G";
            _filterDto.TagFunctionCodes = new List<string>{"H"};
            _filterDto.TagNoStartsWith = "I";
            _filterDto.VoidedFilter = VoidedFilterType.Voided;

            _sortingDto.Direction = SortingDirection.Desc;
            _sortingDto.Property = SortingProperty.Responsible;

            await _dut.ExportTagsWithHistoryToExcel("", true, _filterDto, _sortingDto);

            Assert.AreEqual(_filterDto.ProjectName, _createdQuery.ProjectName);
            Assert.IsNotNull(_createdQuery.Filter);
            Assert.IsNotNull(_createdQuery.Sorting);
            Assert.AreEqual(_filterDto.ActionStatus, _createdQuery.Filter.ActionStatus);
            Assert.AreEqual(1, _createdQuery.Filter.AreaCodes.Count);
            Assert.AreEqual(_filterDto.AreaCodes.ElementAt(0), _createdQuery.Filter.AreaCodes.ElementAt(0));
            Assert.AreEqual(_filterDto.CallOffStartsWith, _createdQuery.Filter.CallOffStartsWith);
            Assert.AreEqual(_filterDto.CommPkgNoStartsWith, _createdQuery.Filter.CommPkgNoStartsWith);
            Assert.AreEqual(1, _createdQuery.Filter.DisciplineCodes.Count);
            Assert.AreEqual(_filterDto.DisciplineCodes.ElementAt(0), _createdQuery.Filter.DisciplineCodes.ElementAt(0));
            Assert.AreEqual(1, _createdQuery.Filter.DueFilters.Count);
            Assert.AreEqual(_filterDto.DueFilters.ElementAt(0), _createdQuery.Filter.DueFilters.ElementAt(0));
            Assert.AreEqual(1, _createdQuery.Filter.JourneyIds.Count);
            Assert.AreEqual(_filterDto.JourneyIds.ElementAt(0), _createdQuery.Filter.JourneyIds.ElementAt(0));
            Assert.AreEqual(_filterDto.McPkgNoStartsWith, _createdQuery.Filter.McPkgNoStartsWith);
            Assert.AreEqual(1, _createdQuery.Filter.ModeIds.Count);
            Assert.AreEqual(_filterDto.ModeIds.ElementAt(0), _createdQuery.Filter.ModeIds.ElementAt(0));
            Assert.AreEqual(_filterDto.PreservationStatus.ElementAt(0), _createdQuery.Filter.PreservationStatus.ElementAt(0));
            Assert.AreEqual(_filterDto.PurchaseOrderNoStartsWith, _createdQuery.Filter.PurchaseOrderNoStartsWith);
            Assert.AreEqual(1, _createdQuery.Filter.RequirementTypeIds.Count);
            Assert.AreEqual(_filterDto.RequirementTypeIds.ElementAt(0), _createdQuery.Filter.RequirementTypeIds.ElementAt(0));
            Assert.AreEqual(1, _createdQuery.Filter.ResponsibleIds.Count);
            Assert.AreEqual(_filterDto.ResponsibleIds.ElementAt(0), _createdQuery.Filter.ResponsibleIds.ElementAt(0));
            Assert.AreEqual(1, _createdQuery.Filter.StepIds.Count);
            Assert.AreEqual(_filterDto.StepIds.ElementAt(0), _createdQuery.Filter.StepIds.ElementAt(0));
            Assert.AreEqual(_filterDto.StorageAreaStartsWith, _createdQuery.Filter.StorageAreaStartsWith);
            Assert.AreEqual(1, _createdQuery.Filter.TagFunctionCodes.Count);
            Assert.AreEqual(_filterDto.TagFunctionCodes.ElementAt(0), _createdQuery.Filter.TagFunctionCodes.ElementAt(0));
            Assert.AreEqual(_filterDto.TagNoStartsWith, _createdQuery.Filter.TagNoStartsWith);
            Assert.AreEqual(_filterDto.VoidedFilter, _createdQuery.Filter.VoidedFilter);
            
            Assert.AreEqual(_sortingDto.Direction, _createdQuery.Sorting.Direction);
            Assert.AreEqual(_sortingDto.Property, _createdQuery.Sorting.Property);
        }
    }
}
