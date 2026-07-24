using Microsoft.AspNetCore.Mvc;
using DeviceTrackerAPI.Models;

namespace DeviceTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private static readonly List<Device> _devices = new()
    {
        new Device { Id = 1, Name = "Temperature Sensor", Location = "Server Room", Value = 24.5, CreatedAt = DateTime.UtcNow },
        new Device { Id = 2, Name = "Humidity Sensor", Location = "Warehouse", Value = 55.0, CreatedAt = DateTime.UtcNow }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_devices);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Device device)
    {
        if (device.Value < 0)
        {
            return BadRequest("Value cannot be negative.");
        }

        device.Id = _devices.Count > 0 ? _devices.Max(d => d.Id) + 1 : 1;
        if (device.CreatedAt == default)
        {
            device.CreatedAt = DateTime.UtcNow;
        }

        _devices.Add(device);
        return Ok(device);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existingDevice = _devices.FirstOrDefault(d => d.Id == id);
        if (existingDevice == null)
        {
            return NotFound();
        }

        _devices.Remove(existingDevice);
        return NoContent();
    }
}
