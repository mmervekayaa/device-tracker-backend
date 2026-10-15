using System.ComponentModel.DataAnnotations;

namespace DeviceTracker.Api.Models;

/// <summary>Cihaz ekleme ve güncelleme isteklerinin gövdesi.</summary>
public record DeviceRequest
{
    [Required(ErrorMessage = "Name alanı zorunludur.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name 1-100 karakter olmalıdır.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Category alanı zorunludur.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Category 1-50 karakter olmalıdır.")]
    public string Category { get; init; } = string.Empty;

    [EnumDataType(typeof(DeviceStatus), ErrorMessage = "Status 'Active' veya 'Inactive' olmalıdır.")]
    public DeviceStatus Status { get; init; } = DeviceStatus.Active;

    [Range(0, double.MaxValue, ErrorMessage = "Value negatif olamaz.")]
    public double Value { get; init; }
}
