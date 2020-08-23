using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.RequirementTypes
{
    [TestClass]
    public class UpdateRequirementDefinitionDtoValidatorTests
    {
        private UpdateRequirementDefinitionDtoValidator _dut;
        private UpdateRequirementDefinitionDto _dto;
        private FieldDto _newField;
        private UpdateFieldDto _updateField;

        [TestInitialize]
        public void Setup()
        {
            _newField = new FieldDto {Label = "New", Unit = "U"};
            _updateField = new UpdateFieldDto {Label = "Upd", Id = 2, Unit = "U"};
            _dut = new UpdateRequirementDefinitionDtoValidator();
            _dto = new UpdateRequirementDefinitionDto
            {
                DefaultIntervalWeeks = 1,
                NewFields = new List<FieldDto>{_newField},
                Title = "T",
                SortKey = 2,
                UpdatedFields = new List<UpdateFieldDto>{_updateField}
            };
        }

        [TestMethod]
        public void Validate_OK()
        {
            var result = _dut.Validate(_dto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenTitleIsTooLong()
        {
            _dto.Title = new string('x', RequirementDefinition.TitleLengthMax + 1); 

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage.StartsWith($"The length of '{nameof(_dto.Title)}' must be {RequirementDefinition.TitleLengthMax} characters or fewer."));
        }

        [TestMethod]
        public void Fail_WhenNewFieldLabelIsNull()
        {
            _newField.Label = null;

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == $"Field label cannot be null and must be maximum {Field.LabelLengthMax}");
        }

        [TestMethod]
        public void Fail_WhenNewFieldLabelIsTooLong()
        {
            _newField.Label = new string('x', Field.LabelLengthMax);

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == $"Field label cannot be null and must be maximum {Field.LabelLengthMax}");
        }

        [TestMethod]
        public void Fail_WhenNewFieldUnitIsTooLong()
        {
            _newField.Unit = new string('x', Field.UnitLengthMax);

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == $"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
        }
        
        [TestMethod]
        public void Fail_WhenUpdateFieldLabelIsNull()
        {
            _updateField.Label = null;

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == $"Field label cannot be null and must be maximum {Field.LabelLengthMax}");
        }

        [TestMethod]
        public void Fail_WhenUpdateFieldLabelIsTooLong()
        {
            _updateField.Label = new string('x', Field.LabelLengthMax);

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == $"Field label cannot be null and must be maximum {Field.LabelLengthMax}");
        }

        [TestMethod]
        public void Fail_WhenUpdateFieldUnitIsTooLong()
        {
            _updateField.Unit = new string('x', Field.UnitLengthMax);

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == $"Field unit must be maximum {nameof(Field.UnitLengthMax)}");
        }

        [TestMethod]
        public void Fail_WhenNewFieldLabelsNotUnique()
        {
            var secondField = new FieldDto
            {
                Label = _newField.Label
            };
            _dto.NewFields.Add(secondField);
            
            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Cannot have duplicate field labels");
        }

        [TestMethod]
        public void Fail_WhenUpdateFieldLabelsNotUnique()
        {
            var secondField = new UpdateFieldDto
            {
                Label = _updateField.Label
            };
            _dto.UpdatedFields.Add(secondField);
            
            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Cannot have duplicate field labels");
        }

        [TestMethod]
        public void Fail_WhenUpdateFieldLabelMatchNewFieldLabel()
        {
            _updateField.Label = _newField.Label;
            
            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Cannot have duplicate field labels");
        }

        [TestMethod]
        public void Fail_WhenNewFieldLabelMatchUpdateFieldLabel()
        {
            _newField.Label = _updateField.Label;
            
            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Cannot have duplicate field labels");
        }

        [TestMethod]
        public void Fail_WhenUpdateFieldIdsNotUnique()
        {
            var secondField = new UpdateFieldDto
            {
                Label = "XXXX",
                Id = _updateField.Id
            };
            _dto.UpdatedFields.Add(secondField);
            
            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Fields to update or delete must be unique");
        }

        [TestMethod]
        public void Fail_SortKeyIsZero()
        {
            _dto.SortKey = 0; 

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Sort key must be positive");
        }

        [TestMethod]
        public void Fail_DefaultIntervalWeeksZero()
        {
            _dto.DefaultIntervalWeeks = 0; 

            var result = _dut.Validate(_dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Single().ErrorMessage == "Week interval must be positive");
        }
    }
}
