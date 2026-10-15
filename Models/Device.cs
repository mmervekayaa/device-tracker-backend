namespace DeviceTracker.Api.Models;

/// <summary>Cihazın çalışma durumu.</summary>
public enum DeviceStatus
{
    Active,
    Inactive
}

/// <summary>Sistemde takip edilen donanım / sensör kaydı.</summary>
public class Device
{
    /// <summary>Benzersiz kimlik.</summary>
    public int Id { get; set; }

    /// <summary>Cihaz adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Cihaz kategorisi (ör. Sensor, Gateway).</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Aktif / pasif çalışma durumu.</summary>
    public DeviceStatus Status { get; set; } = DeviceStatus.Active;

    /// <summary>Sayısal ölçüm verisi.</summary>
    public double Value { get; set; }

    /// <summary>Kaydın son güncellenme zamanı (UTC).</summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
