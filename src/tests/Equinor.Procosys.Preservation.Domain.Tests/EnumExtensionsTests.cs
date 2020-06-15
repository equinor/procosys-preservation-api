using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class EnumExtensionsTests
    {
        private enum TestEnums
        {
            [System.ComponentModel.Description("Some enum description")]
            SomeEnum
        }

        [TestMethod]
        public void GetDescription_ShouldReturnDescription()
        {
            var expected = "Some enum description";
            var actual = TestEnums.SomeEnum.GetDescription();
            Assert.AreEqual(expected, actual);
        }
    }
}
