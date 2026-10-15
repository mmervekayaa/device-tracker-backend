using System.Collections.Concurrent;
using DeviceTracker.Api.Models;

namespace DeviceTracker.Api.Data;

public interface IDeviceStore
{
    Task<IReadOnlyList<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);
    Task<Device> AddAsync(Device device);
    Task<Device?> UpdateAsync(int id, Device device);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Thread-safe bellek içi veri deposu. Eşzamanlı isteklerde veri bütünlüğü
/// ConcurrentDictionary ve Interlocked ile korunur.
/// </summary>
public class InMemoryDeviceStore : IDeviceStore
{
    private readonly ConcurrentDictionary<int, Device> _devices = new();
    private int _lastId;

    public InMemoryDeviceStore(bool seed = true)
    {
        if (seed)
        {
            Seed();
        }
    }

    private void Seed()
    {
        var seedData = new[]
        {
            new Device { Name = "Temperature Sensor", Category = "Sensor", Status = DeviceStatus.Active, Value = 24.5 },
            new Device { Name = "Humidity Sensor", Category = "Sensor", Status = DeviceStatus.Active, Value = 55.0 },
            new Device { Name = "IoT Gateway", Category = "Gateway", Status = DeviceStatus.Inactive, Value = 0 }
        };

        foreach (var device in seedData)
        {
            device.Id = Interlocked.Increment(ref _lastId);
            device.LastUpdated = DateTime.UtcNow;
            _devices[device.Id] = device;
        }
    }

    public Task<IReadOnlyList<Device>> GetAllAsync()
    {
        IReadOnlyList<Device> list = _devices.Values.OrderBy(d => d.Id).ToList();
        return Task.FromResult(list);
    }

    public Task<Device?> GetByIdAsync(int id)
    {
        _devices.TryGetValue(id, out var device);
        return Task.FromResult(device);
    }

    public Task<Device> AddAsync(Device device)
    {
        device.Id = Interlocked.Increment(ref _lastId);
        device.LastUpdated = DateTime.UtcNow;
        _devices[device.Id] = device;
        return Task.FromResult(device);
    }

    public Task<Device?> UpdateAsync(int id, Device device)
    {
        while (_devices.TryGetValue(id, out var existing))
        {
            var updated = new Device
            {
                Id = id,
                Name = device.Name,
                Category = device.Category,
                Status = device.Status,
                Value = device.Value,
                LastUpdated = DateTime.UtcNow
            };

            if (_devices.TryUpdate(id, updated, existing))
            {
                return Task.FromResult<Device?>(updated);
            }
        }

        return Task.FromResult<Device?>(null);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_devices.TryRemove(id, out _));
    }
}
