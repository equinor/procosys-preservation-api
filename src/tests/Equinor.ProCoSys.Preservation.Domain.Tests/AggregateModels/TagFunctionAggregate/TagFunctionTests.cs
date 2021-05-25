using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.TagFunctionAggregate
{
    [TestClass]
    public class TagFunctionTests
    {
        private const string TestPlant = "PlantA";
        const string NewCode = "ANewCode";
        const string NewRegisterCode = "ANewRegisterCode";

        private readonly TagFunction _dut = new TagFunction(TestPlant, "CodeA", "DescA", "CodeR");

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
        public void RenameTagFunction_ShouldUpdateCodeAndRegisterCode()
        {
            _dut.RenameTagFunction(NewCode, NewRegisterCode);

            Assert.AreEqual(NewCode, _dut.Code);
            Assert.AreEqual(NewRegisterCode, _dut.RegisterCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameTagFunction_ShouldFailOnEmptyCode() =>
            _dut.RenameTagFunction(" ", NewRegisterCode);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameTagFunction_ShouldFailOnNullCode() =>
            _dut.RenameTagFunction(null, NewRegisterCode);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameTagFunction_ShouldFailOnEmptyRegisterCode() =>
            _dut.RenameTagFunction(NewCode, " ");

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameTagFunction_ShouldFailOnNullRegisterCode() =>
            _dut.RenameTagFunction(NewCode, null);
    }
}
