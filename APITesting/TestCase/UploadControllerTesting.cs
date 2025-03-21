using API.Controllers;
using BusinessObject.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;

namespace APITesting;
#pragma warning disable

public class UploadControllerTesting
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void UploadImage_ReturnsOk_WithValidImage()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Fake image content";
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(stream => ms.CopyTo(stream));

        var uploadDto = new UploadDTO { File = fileMock.Object };

        var envMock = new Mock<IWebHostEnvironment>();
        var contentRootPath = Path.GetTempPath(); // Chỉ test local
        envMock.Setup(e => e.ContentRootPath).Returns(contentRootPath);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Base:Url"]).Returns("http://localhost");

        var controller = new UploadController(null, null, envMock.Object, configMock.Object);

        // Act
        var result = controller.UploadImage(uploadDto) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.That(result.Value.ToString(), Does.Contain("http://localhost/images/"));
    }


    [Test]
    public void UploadList_ReturnsOk_WithMultipleFiles()
    {
        // Arrange
        var fileContent = "Dummy content";
        var fileName1 = "image1.jpg";
        var fileName2 = "image2.jpg";

        var formFile1 = CreateMockFormFile(fileContent, fileName1);
        var formFile2 = CreateMockFormFile(fileContent, fileName2);

        var uploadListDto = new UploadListDTO { Files = new List<IFormFile> { formFile1, formFile2 } };

        var envMock = new Mock<IWebHostEnvironment>();
        var tempFolder = Path.GetTempPath();
        envMock.Setup(e => e.ContentRootPath).Returns(tempFolder);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Base:Url"]).Returns("https://homestaybooking-001-site1.ntempurl.com");

        var controller = new UploadController(null, null, envMock.Object, configMock.Object);

        // Act
        var result = controller.UploadList(uploadListDto) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        // Deserialize anonymous object
        var json = JsonConvert.SerializeObject(result.Value);
        var response = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

        Assert.IsTrue(response.ContainsKey("urls"));
        var urls = response["urls"];

        Assert.AreEqual(2, urls.Count);
        Assert.That(urls[0], Does.StartWith("https://homestaybooking-001-site1.ntempurl.com/images/"));
        Assert.That(urls[1], Does.StartWith("https://homestaybooking-001-site1.ntempurl.com/images/"));
    }



    private IFormFile CreateMockFormFile(string content, string fileName)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }


}
