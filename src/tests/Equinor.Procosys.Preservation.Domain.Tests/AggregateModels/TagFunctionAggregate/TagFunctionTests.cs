using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagFunctionAggregate
{
    [TestClass]
    public class TagFunctionTests
    {
        private const string TestPlant = "PlantA";
        private TagFunction _dut = new TagFunction(TestPlant, "CodeA", "DescA", "CodeR");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("CodeA", _dut.Code);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.AreEqual("CodeR", _dut.RegisterCode);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(0, _dut.Requirements.Count);
        }
        
        [TestMethod]
        public void Constructor_ShouldThrowException_WhenCodeNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new TagFunction(TestPlant, null, "DescA", "CodeR")
            );
        
        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRegisterCodeNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new TagFunction(TestPlant, "CodeA", "DescA", null)
            );

        [TestMethod]
        public void AddRequirement_ShouldAddRequirementToRequirementsList()
        {
            var reqMock = new Mock<TagFunctionRequirement>();
            reqMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _dut.AddRequirement(reqMock.Object);

            Assert.AreEqual(1, _dut.Requirements.Count);
            Assert.IsTrue(_dut.Requirements.Contains(reqMock.Object));
        }
        
        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => _dut.AddRequirement(null));

        [TestMethod]
        public void RemoveRequirement_ShouldRemoveRequirementFromRequirementsList()
        {
            var reqMock = new Mock<TagFunctionRequirement>();
            reqMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _dut.AddRequirement(reqMock.Object);

            Assert.AreEqual(1, _dut.Requirements.Count);
            Assert.IsTrue(_dut.Requirements.Contains(reqMock.Object));

            _dut.RemoveRequirement(reqMock.Object);

            Assert.AreEqual(0, _dut.Requirements.Count);
        }

        [TestMethod]
        public void RemoveRequirement_ShouldDoNothingIdWhenRequirementNotExist()
        {
            var reqMock = new Mock<TagFunctionRequirement>();
            reqMock.SetupGet(s => s.Plant).Returns(TestPlant);

            Assert.AreEqual(0, _dut.Requirements.Count);

            _dut.RemoveRequirement(reqMock.Object);
            Assert.AreEqual(0, _dut.Requirements.Count);
        }
        
        [TestMethod]
        public void RemoveRequirement_ShouldThrowException_WhenRequirementNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => _dut.RemoveRequirement(null));

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            Assert.IsFalse(_dut.IsVoided);

            _dut.Void();
            Assert.IsTrue(_dut.IsVoided);

            _dut.UnVoid();
            Assert.IsFalse(_dut.IsVoided);
        }
    }
}
