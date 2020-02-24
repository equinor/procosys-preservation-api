using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class TagValidatorTests : ReadOnlyTestsBase
    {
        private const string ProjectName = "P";
        private const string TagNo = "PA-13";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var p = AddProject(context, ProjectName, "Project description");
                var j = AddJourneyWithStep(context, "J", AddMode(context, "M"), AddResponsible(context, "R"));
                var rt = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D");
                var r = new Requirement(_schema, 4, rt.RequirementDefinitions.First());
                AddTag(context, p, TagNo, "Tag description", j.Steps.First(), new List<Requirement>{r});
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ExistsAsync(TagNo, ProjectName, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ExistsAsync("X", ProjectName, default);
                Assert.IsFalse(result);
            }
        }
    }
}
