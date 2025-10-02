namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// Interface سرویس QR Code
/// </summary>
public interface IQRCodeService
{
    Task<string> GenerateQRCodeAsync(string url, int size = 300, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateQRCodeBytesAsync(string url, int size = 300, CancellationToken cancellationToken = default);
    Task<string> SaveQRCodeAsync(string restaurantId, string url, int size = 300, CancellationToken cancellationToken = default);
}
