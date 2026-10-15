using DeviceTracker.Api.Controllers;
using DeviceTracker.Api.Data;
using DeviceTracker.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace DeviceTracker.Tests;

public class DevicesControllerTests
{
    // Her test kendi bağımsız deposuyla çalışır (Arrange-Act-Assert).
    private static DevicesController CreateController(out InMemoryDeviceStore store, bool seed = true)
    {
        store = new InMemoryDeviceStore(seed);
        return new DevicesController(store, NullLogger<DevicesController>.Instance);
    }

    private static DeviceRequest ValidRequest(double value = 10) => new()
    {
        Name = "Test Sensor",
        Category = "Sensor",
        Status = DeviceStatus.Active,
        Value = value
    };

    // ---------- GET ----------

    [Fact]
    public async Task GetAll_ReturnsOkWithSeedDevices()
    {
        var controller = CreateController(out _);

        var result = await controller.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IEnumerable<Device>>()
            .Which.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_WithExistingId_ReturnsOkAndCorrectDevice()
    {
        var controller = CreateController(out var store);
        var existing = (await store.GetAllAsync()).First();

        var result = await controller.GetById(existing.Id);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeOfType<Device>().Which.Id.Should().Be(existing.Id);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(out _);

        var result = await controller.GetById(999999);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ---------- POST ----------

    [Fact]
    public async Task Create_WithValidDevice_ReturnsCreatedWithLocation()
    {
        var controller = CreateController(out var store, seed: false);

        var result = await controller.Create(ValidRequest());

        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        created.ActionName.Should().Be(nameof(DevicesController.GetById));
        var device = created.Value.Should().BeOfType<Device>().Subject;
        device.Id.Should().BePositive();
        device.Name.Should().Be("Test Sensor");
        (await store.GetAllAsync()).Should().ContainSingle();
    }

    [Fact]
    public async Task Create_WithNegativeValue_ReturnsBadRequest()
    {
        var controller = CreateController(out var store, seed: false);

        var result = await controller.Create(ValidRequest(value: -5));

        result.Should().BeOfType<BadRequestObjectResult>();
        (await store.GetAllAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task Create_WithZeroValue_IsAccepted()
    {
        var controller = CreateController(out _);

        var result = await controller.Create(ValidRequest(value: 0));

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Theory]
    [InlineData("", "Sensor")]
    [InlineData("   ", "Sensor")]
    [InlineData("Sensor A", "")]
    public async Task Create_WithEmptyRequiredFields_ReturnsBadRequest(string name, string category)
    {
        var controller = CreateController(out _);
        var request = ValidRequest() with { Name = name, Category = category };

        var result = await controller.Create(request);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WithInvalidStatus_ReturnsBadRequest()
    {
        var controller = CreateController(out _);
        var request = ValidRequest() with { Status = (DeviceStatus)99 };

        var result = await controller.Create(request);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---------- PUT ----------

    [Fact]
    public async Task Update_WithValidData_ReturnsOkAndUpdatesDevice()
    {
        var controller = CreateController(out var store);
        var existing = (await store.GetAllAsync()).First();
        var request = ValidRequest(42) with { Name = "Updated", Status = DeviceStatus.Inactive };

        var result = await controller.Update(existing.Id, request);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var device = ok.Value.Should().BeOfType<Device>().Subject;
        device.Name.Should().Be("Updated");
        device.Value.Should().Be(42);
        device.Status.Should().Be(DeviceStatus.Inactive);
    }

    [Fact]
    public async Task Update_WithNegativeValue_ReturnsBadRequestAndKeepsOriginal()
    {
        var controller = CreateController(out var store);
        var existing = (await store.GetAllAsync()).First();
        var originalValue = existing.Value;

        var result = await controller.Update(existing.Id, ValidRequest(-1));

        result.Should().BeOfType<BadRequestObjectResult>();
        (await store.GetByIdAsync(existing.Id))!.Value.Should().Be(originalValue);
    }

    [Fact]
    public async Task Update_WithNonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(out _);

        var result = await controller.Update(999999, ValidRequest());

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_WithExistingId_ReturnsNoContentAndRemoves()
    {
        var controller = CreateController(out var store);
        var existing = (await store.GetAllAsync()).First();

        var result = await controller.Delete(existing.Id);

        result.Should().BeOfType<NoContentResult>();
        (await store.GetByIdAsync(existing.Id)).Should().BeNull();
    }

    [Fact]
    public async Task Delete_WithNonExistingId_ReturnsNotFound()
    {
        var controller = CreateController(out _);

        var result = await controller.Delete(999999);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
