# Update QR Code URLs in Database

Write-Host "🔄 Updating QR Code URLs in database..." -ForegroundColor Cyan

$connectionString = "Server=.;Database=eazy-menu;Trusted_Connection=True;TrustServerCertificate=True"

# SQL to find actual QR code files and update database
$sql = @"
-- Update Zeitoon
UPDATE Restaurants 
SET QRCodeUrl = '/qrcodes/zeitoon/qr_638950126772699062.png'
WHERE Slug = 'zeitoon';

-- Update Burger Star
UPDATE Restaurants 
SET QRCodeUrl = '/qrcodes/burger-star/qr_638950126771359679.png'
WHERE Slug = 'burger-star';

-- Update Niloofar Cafe
UPDATE Restaurants 
SET QRCodeUrl = '/qrcodes/niloofar-cafe/qr_638950126769401334.png'
WHERE Slug = 'niloofar-cafe';

SELECT Slug, QRCodeUrl FROM Restaurants WHERE IsDeleted = 0;
"@

try {
    # Execute SQL using sqlcmd
    $result = $sql | sqlcmd -S "." -d "eazy-menu" -E -h -1
    
    Write-Host "✅ QR Code URLs updated successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Current QR Code URLs:" -ForegroundColor Yellow
    Write-Host $result
}
catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
}
