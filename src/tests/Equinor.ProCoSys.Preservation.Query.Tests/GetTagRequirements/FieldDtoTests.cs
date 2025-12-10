using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagRequirements;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagRequirements
{
    [TestClass]
    public class FieldDtoTests
    {
        private readonly string _testPlant = "PCS$PLANT";
        private readonly double _number = 1282.91;

        [TestMethod]
        public void Constructor_ShouldSetProperties_ForCheckBox()
        {
            var field = new Field(_testPlant, "", FieldType.CheckBox, 0);

            var dut = new FieldDetailsDto(
                field,
                new CheckBoxChecked(_testPlant, field),
                null);

            var dto = dut.CurrentValue as CheckBoxDetailsDto;
            Assert.IsNotNull(dto);
            Assert.IsTrue(dto.IsChecked);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_ForNumberWithoutCurrent()
        {
            var fieldMock = new Mock<Field>(_testPlant, "Label", FieldType.Number, 0, "mm", true);
            fieldMock.SetupGet(f => f.Id).Returns(12);

            var dut = new FieldDetailsDto(fieldMock.Object, null, null);

            Assert.AreEqual(12, dut.Id);
            Assert.AreEqual(FieldType.Number, dut.FieldType);
            Assert.AreEqual("Label", dut.Label);
            Assert.AreEqual("mm", dut.Unit);
            Assert.IsTrue(dut.ShowPrevious);
            Assert.IsNull(dut.CurrentValue);
            Assert.IsNull(dut.PreviousValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_ForNumberWithCurrentNotShowPrevious()
        {
            var field = new Field(_testPlant, "", FieldType.Number, 0, "mm", false);

            var dut = new FieldDetailsDto(
                field,
                new NumberValue(_testPlant, field, _number),
                new NumberValue(_testPlant, field, _number));

            Assert.IsFalse(dut.ShowPrevious);
            Assert.IsNotNull(dut.CurrentValue);
            Assert.IsNull(dut.PreviousValue);
            AssertNumberDto(dut.CurrentValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_ForNumberWithCurrentShowPrevious()
        {
            var field = new Field(_testPlant, "", FieldType.Number, 0, "mm", true);

            var dut = new FieldDetailsDto(
                field,
                new NumberValue(_testPlant, field, _number),
                new NumberValue(_testPlant, field, _number));

            Assert.IsTrue(dut.ShowPrevious);
            Assert.IsNotNull(dut.CurrentValue);
            Assert.IsNotNull(dut.PreviousValue);
            AssertNumberDto(dut.CurrentValue);
            AssertNumberDto(dut.PreviousValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_ForAttachment()
        {
            var field = new Field(_testPlant, "", FieldType.Attachment, 0);

            var fileName = "FilA.txt";
            var attachment = new FieldValueAttachment(_testPlant, Guid.Empty, fileName);
            attachment.SetProtectedIdForTesting(11);

            var dut = new FieldDetailsDto(
                field,
                new AttachmentValue(_testPlant, field, attachment),
                null);

            var dto = dut.CurrentValue as AttachmentDetailsDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual(11, dto.Id);
            Assert.AreEqual(fileName, dto.FileName);
        }

        private void AssertNumberDto(object dto)
        {
            Assert.IsInstanceOfType(dto, typeof(NumberDetailsDto));

            var numberDto = dto as NumberDetailsDto;
            Assert.IsNotNull(numberDto);
            Assert.IsNotNull(numberDto);
            Assert.IsFalse(numberDto.IsNA);
            Assert.IsFalse(numberDto.IsNA);
            Assert.IsTrue(numberDto.Value.HasValue);
            Assert.AreEqual(_number, numberDto.Value.Value);
        }
    }
}
