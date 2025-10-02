using EazyMenu.Application.Common.Interfaces;
using QRCoder;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// سرویس تولید QR Code
/// </summary>
public class QRCodeService : IQRCodeService
{
    public Task<string> GenerateQRCodeAsync(string url, int size = 300, CancellationToken cancellationToken = default)
    {
        var qrCodeBytes = GenerateQRCodeBytes(url, size);
        var base64String = Convert.ToBase64String(qrCodeBytes);
        return Task.FromResult($"data:image/png;base64,{base64String}");
    }

    public Task<byte[]> GenerateQRCodeBytesAsync(string url, int size = 300, CancellationToken cancellationToken = default)
    {
        var bytes = GenerateQRCodeBytes(url, size);
        return Task.FromResult(bytes);
    }

    public async Task<string> SaveQRCodeAsync(string restaurantId, string url, int size = 300, CancellationToken cancellationToken = default)
    {
        var qrCodeBytes = GenerateQRCodeBytes(url, size);
        
        // ایجاد پوشه اگر وجود ندارد
        var directory = Path.Combine("wwwroot", "qrcodes", restaurantId);
        Directory.CreateDirectory(directory);
        
        // ذخیره فایل
        var fileName = $"qr_{DateTime.UtcNow.Ticks}.png";
        var filePath = Path.Combine(directory, fileName);
        await File.WriteAllBytesAsync(filePath, qrCodeBytes, cancellationToken);
        
        return $"/qrcodes/{restaurantId}/{fileName}";
    }

    private byte[] GenerateQRCodeBytes(string content, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        
        return qrCode.GetGraphic(pixelsPerModule);
    }
}
