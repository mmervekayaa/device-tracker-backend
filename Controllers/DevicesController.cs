using DeviceTracker.Api.Data;
using DeviceTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceStore _store;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(IDeviceStore store, ILogger<DevicesController> logger)
    {
        _store = store;
        _logger = logger;
    }

    /// <summary>Tüm cihazları listeler.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Device>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _store.GetAllAsync();
        return Ok(devices);
    }

    /// <summary>Kimliğe göre tek bir cihazı getirir.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Device), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var device = await _store.GetByIdAsync(id);
        if (device is null)
        {
            return NotFound(new { error = $"{id} kimlikli cihaz bulunamadı." });
        }

        return Ok(device);
    }

    /// <summary>Yeni cihaz ekler. Negatif ölçüm değeri reddedilir.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Device), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] DeviceRequest request)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return BadRequest(new { error = validationError });
        }

        try
        {
            var created = await _store.AddAsync(ToDevice(request));
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cihaz eklenirken hata oluştu.");
            return Problem("Cihaz eklenirken beklenmeyen bir hata oluştu.");
        }
    }

    /// <summary>Var olan cihazı günceller.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Device), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] DeviceRequest request)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return BadRequest(new { error = validationError });
        }

        try
        {
            var updated = await _store.UpdateAsync(id, ToDevice(request));
            if (updated is null)
            {
                return NotFound(new { error = $"{id} kimlikli cihaz bulunamadı." });
            }

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cihaz güncellenirken hata oluştu.");
            return Problem("Cihaz güncellenirken beklenmeyen bir hata oluştu.");
        }
    }

    /// <summary>Cihazı siler.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _store.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { error = $"{id} kimlikli cihaz bulunamadı." });
        }

        return NoContent();
    }

    // İş kuralları: model doğrulamasına ek olarak controller seviyesinde de kontrol edilir.
    private static string? Validate(DeviceRequest? request)
    {
        if (request is null)
        {
            return "İstek gövdesi boş olamaz.";
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Name alanı boş olamaz.";
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            return "Category alanı boş olamaz.";
        }

        if (!Enum.IsDefined(request.Status))
        {
            return "Status 'Active' veya 'Inactive' olmalıdır.";
        }

        if (request.Value < 0)
        {
            return "Value negatif olamaz.";
        }

        return null;
    }

    private static Device ToDevice(DeviceRequest request) => new()
    {
        Name = request.Name.Trim(),
        Category = request.Category.Trim(),
        Status = request.Status,
        Value = request.Value
    };
}
