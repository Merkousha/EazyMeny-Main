# 🔄 Reset Database Script
# این اسکریپت دیتابیس را حذف و مجدد با Seed ایجاد می‌کند

Write-Host "🗑️  Dropping database..." -ForegroundColor Yellow
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Infrastructure"
dotnet ef database drop --startup-project ../EazyMenu.Web --force

Write-Host "✅ Database dropped!" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Starting application (Database will be created and seeded automatically)..." -ForegroundColor Cyan
Write-Host ""

cd ../EazyMenu.Web
dotnet run

# بعد از Ctrl+C
Write-Host ""
Write-Host "✅ Database reset complete!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Login credentials:" -ForegroundColor Cyan
Write-Host "   Admin:    admin@eazymenu.ir / Admin@123" -ForegroundColor White
Write-Host "   Owner:    owner@restaurant.com / Owner@123" -ForegroundColor White
Write-Host "   Customer: customer@test.com / Customer@123" -ForegroundColor White
