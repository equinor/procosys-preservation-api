using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class UploadAttachmentDtoValidatorTests
    {
        private UploadAttachmentDtoValidator _dut;
        private AttachmentOptions _options;

        [TestInitialize]
        public void Setup()
        {
            var attachmentOptionsMock = new Mock<IOptionsMonitor<AttachmentOptions>>();
            _options = new AttachmentOptions
            {
                MaxSizeKb = 2,
                ValidFileSuffixes = ".gif|.jpg"
            };
            attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(_options);
            _dut = new UploadAttachmentDtoValidator(attachmentOptionsMock.Object);
        }

        [TestMethod]
        public void Validate_OK()
        {
            var uploadAttachmentDto = new UploadAttachmentDto
            {
                File = new TestFile("picture.gif", 1000)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_When_FileNotGiven()
        {
            var uploadAttachmentDto = new UploadAttachmentDto();

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(result.Errors[0].ErrorMessage, "'File' must not be empty.");
        }
        
        [TestMethod]
        public void Fail_WhenFileNameNotExists()
        {
            var uploadAttachmentDto = new UploadAttachmentDto
            {
                File = new TestFile(null, 1000)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(result.Errors[0].ErrorMessage, "Filename not given!");
        }
        
        [TestMethod]
        public void Fail_WhenFileNameIsTooLong()
        {
            var uploadAttachmentDto = new UploadAttachmentDto
            {
                File = new TestFile(new string('x', Attachment.FileNameLengthMax + 1), 1000)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Filename to long! Max"));
        }

        [TestMethod]
        public void Fail_When_FileToBig()
        {
            var uploadAttachmentDto = new UploadAttachmentDto
            {
                File = new TestFile("picture.gif", 2500)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(result.Errors[0].ErrorMessage, $"Maximum file size is {_options.MaxSizeKb}kB!");
        }

        [TestMethod]
        public void Fail_When_IllegalFileType()
        {
            var uploadAttachmentDto = new UploadAttachmentDto
            {
                File = new TestFile("picture.exe", 500)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(result.Errors[0].ErrorMessage, $"File {uploadAttachmentDto.File.FileName} is not a valid file for upload!");
        }

        class TestFile : IFormFile
        {
            public TestFile(string fileName, long length)
            {
                ContentDisposition = null;
                ContentType = null;
                FileName = fileName;
                Headers = null;
                Length = length;
                Name = null;
            }

            public void CopyTo(Stream target) => throw new System.NotImplementedException();

            public Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken()) => throw new System.NotImplementedException();

            public Stream OpenReadStream() => throw new System.NotImplementedException();

            public string ContentDisposition { get; }
            public string ContentType { get; }
            public string FileName { get; }
            public IHeaderDictionary Headers { get; }
            public long Length { get; }
            public string Name { get; }
        }
    }
}
