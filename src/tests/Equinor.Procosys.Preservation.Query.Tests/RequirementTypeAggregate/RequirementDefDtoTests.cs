using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementDefDtoTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var rd = new RequirementDefinitionDto(1, "TitleA", true, 4, 10, null);

            Assert.AreEqual(1, rd.Id);
            Assert.AreEqual("TitleA", rd.Title);
            Assert.AreEqual(4, rd.DefaultInterval);
            Assert.AreEqual(10, rd.SortKey);
            Assert.IsTrue(rd.IsVoided);
            Assert.IsNull(rd.Fields);
        }
    }
}
