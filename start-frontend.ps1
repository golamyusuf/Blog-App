# Start Frontend Development Server
# Run this from the project root directory

Write-Host "Starting Blog Application Frontend..." -ForegroundColor Cyan
Write-Host ""

if (Test-Path "client") {
    Set-Location "client"
    
    Write-Host "Frontend starting at:" -ForegroundColor Yellow
    Write-Host "  http://localhost:3000" -ForegroundColor Green
    Write-Host ""
    Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
    Write-Host ""
    
    npm run dev
} else {
    Write-Host "Error: client directory not found!" -ForegroundColor Red
    Write-Host "Make sure you're running this from the project root directory." -ForegroundColor Yellow
}
