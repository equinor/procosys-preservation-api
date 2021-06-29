using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class UploadAttachmentForceOverwriteDtoValidatorTests
    {
        private UploadAttachmentForceOverwriteDtoValidator _dut;
        private BlobStorageOptions _options;

        [TestInitialize]
        public void Setup()
        {
            var blobStorageOptionsMock = new Mock<IOptionsMonitor<BlobStorageOptions>>();
            _options = new BlobStorageOptions
            {
                MaxSizeMb = 2,
                BlobContainer = "bc",
                BlockedFileSuffixes = new[] {".exe", ".zip"}
            };
            blobStorageOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(_options);
            _dut = new UploadAttachmentForceOverwriteDtoValidator(blobStorageOptionsMock.Object);
        }

        [TestMethod]
        public void Validate_OK()
        {
            var uploadAttachmentDto = new UploadAttachmentForceOverwriteDto
            {
                File = new TestFile("picture.gif", 1000)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_When_FileNotGiven()
        {
            var uploadAttachmentDto = new UploadAttachmentForceOverwriteDto();

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(result.Errors[0].ErrorMessage, "'File' must not be empty.");
        }
        
        [TestMethod]
        public void Fail_WhenFileNameNotExists()
        {
            var uploadAttachmentDto = new UploadAttachmentForceOverwriteDto
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
            var uploadAttachmentDto = new UploadAttachmentForceOverwriteDto
            {
                File = new TestFile(new string('x', Attachment.FileNameLengthMax + 1), 1000)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Filename to long! Max"));
        }

        [TestMethod]
        public void Fail_When_FileToBig()
        {
            var uploadAttachmentDto = new UploadAttachmentForceOverwriteDto
            {
                File = new TestFile("picture.gif", (_options.MaxSizeMb * 1024 * 1024) + 1)
            };

            var result = _dut.Validate(uploadAttachmentDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(result.Errors[0].ErrorMessage, $"Maximum file size is {_options.MaxSizeMb}MB!");
        }

        [TestMethod]
        public void Fail_When_IllegalFileType()
        {
            var uploadAttachmentDto = new UploadAttachmentForceOverwriteDto
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
            public TestFile(string fileName, long lengthInBytes)
            {
                ContentDisposition = null;
                ContentType = null;
                FileName = fileName;
                Headers = null;
                Length = lengthInBytes;
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
