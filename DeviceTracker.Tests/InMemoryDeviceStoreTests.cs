using DeviceTracker.Api.Data;
using DeviceTracker.Api.Models;
using FluentAssertions;

namespace DeviceTracker.Tests;

public class InMemoryDeviceStoreTests
{
    [Fact]
    public async Task AddAsync_ConcurrentRequests_ProduceUniqueIds()
    {
        var store = new InMemoryDeviceStore(seed: false);

        var tasks = Enumerable.Range(0, 500)
            .Select(i => Task.Run(() => store.AddAsync(new Device
            {
                Name = $"Device {i}",
                Category = "Sensor",
                Value = i
            })));
        var devices = await Task.WhenAll(tasks);

        devices.Select(d => d.Id).Should().OnlyHaveUniqueItems();
        (await store.GetAllAsync()).Should().HaveCount(500);
    }

    [Fact]
    public async Task Seed_LoadsInitialDevices()
    {
        var store = new InMemoryDeviceStore();

        (await store.GetAllAsync()).Should().NotBeEmpty();
    }
}
