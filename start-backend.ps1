# Start Backend API
# Run this from the project root directory

Write-Host "Starting Blog Application Backend..." -ForegroundColor Cyan
Write-Host ""

Set-Location "src/BlogApplication.API"

Write-Host "Backend API starting at:" -ForegroundColor Yellow
Write-Host "  HTTP:  http://localhost:5000" -ForegroundColor Green
Write-Host "  HTTPS: https://localhost:5001" -ForegroundColor Green
Write-Host "  Swagger: https://localhost:5001/swagger" -ForegroundColor Green
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

dotnet run
