using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeDtoTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var rt = new RequirementTypeDto(1, "CodeA", "TitleA", true, 10, null);

            Assert.AreEqual(1, rt.Id);
            Assert.AreEqual("CodeA", rt.Code);
            Assert.AreEqual("TitleA", rt.Title);
            Assert.AreEqual(10, rt.SortKey);
            Assert.IsTrue(rt.IsVoided);
            Assert.IsNull(rt.RequirementDefinitions);
        }
    }
}
