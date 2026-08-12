using DeviceTrackerAPI.Controllers;
using DeviceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DeviceTrackerAPI.Tests;

public class DevicesControllerTests
{
    [Fact]
    public void GetAll_ReturnsOkResult()
    {
        var controller = new DevicesController();

        var result = controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Create_WithValidDevice_ReturnsOkAndAddsDevice()
    {
        var controller = new DevicesController();
        var newDevice = new Device { Name = "Test Sensor", Location = "Test Room", Value = 10 };

        var result = controller.Create(newDevice) as OkObjectResult;
        var created = result?.Value as Device;

        Assert.NotNull(result);
        Assert.NotNull(created);
        Assert.Equal("Test Sensor", created!.Name);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public void Create_WithNegativeValue_ReturnsBadRequest()
    {
        var controller = new DevicesController();
        var invalidDevice = new Device { Name = "Bad Sensor", Location = "Test Room", Value = -5 };

        var result = controller.Create(invalidDevice);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsBadRequest()
    {
        var controller = new DevicesController();
        var invalidDevice = new Device { Name = "", Location = "Test Room", Value = 10 };

        var result = controller.Create(invalidDevice);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Create_WithEmptyLocation_ReturnsBadRequest()
    {
        var controller = new DevicesController();
        var invalidDevice = new Device { Name = "Sensor", Location = "", Value = 10 };

        var result = controller.Create(invalidDevice);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Delete_WithExistingId_ReturnsNoContent()
    {
        var controller = new DevicesController();
        var newDevice = new Device { Name = "Temp Sensor", Location = "Temp Room", Value = 5 };
        var createResult = controller.Create(newDevice) as OkObjectResult;
        var created = createResult?.Value as Device;

        var result = controller.Delete(created!.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Delete_WithNonExistingId_ReturnsNotFound()
    {
        var controller = new DevicesController();

        var result = controller.Delete(999999);

        Assert.IsType<NotFoundResult>(result);
    }
}