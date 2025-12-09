# Blog Application Setup Script
# Run this script from the project root directory

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Blog Application Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check Docker
Write-Host "Checking Docker..." -ForegroundColor Yellow
try {
    docker --version | Out-Null
    Write-Host "✓ Docker is installed" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker is not installed. Please install Docker Desktop." -ForegroundColor Red
    exit 1
}

# Check .NET
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK $dotnetVersion is installed" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK is not installed. Please install .NET 10 SDK." -ForegroundColor Red
    exit 1
}

# Check Node.js
Write-Host "Checking Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✓ Node.js $nodeVersion is installed" -ForegroundColor Green
} catch {
    Write-Host "✗ Node.js is not installed. Please install Node.js 18+." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Docker Containers" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Start Docker containers
docker-compose up -d

Write-Host "Waiting for databases to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check if containers are running
$mysqlRunning = docker ps | Select-String "mysql-db-new"
$mongoRunning = docker ps | Select-String "mongo-dev"

if ($mysqlRunning -and $mongoRunning) {
    Write-Host "✓ Docker containers are running" -ForegroundColor Green
} else {
    Write-Host "✗ Docker containers failed to start. Check docker-compose.yml" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setting Up Backend" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Restore backend packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
Set-Location "src"
dotnet restore
Write-Host "✓ Packages restored" -ForegroundColor Green

# Build backend
Write-Host "Building backend..." -ForegroundColor Yellow
dotnet build
Write-Host "✓ Backend built successfully" -ForegroundColor Green

# Run migrations
Write-Host "Running database migrations..." -ForegroundColor Yellow
Set-Location "BlogApplication.API"
try {
    dotnet ef database update
    Write-Host "✓ Database migrations completed" -ForegroundColor Green
} catch {
    Write-Host "⚠ Migration failed. You may need to create migrations first:" -ForegroundColor Yellow
    Write-Host "  dotnet ef migrations add InitialCreate --project ../BlogApplication.Infrastructure" -ForegroundColor Yellow
}

Set-Location "../.."

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setting Up Frontend" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Install frontend dependencies
if (Test-Path "client/package.json") {
    Write-Host "Installing frontend dependencies..." -ForegroundColor Yellow
    Set-Location "client"
    npm install
    Write-Host "✓ Frontend dependencies installed" -ForegroundColor Green
    Set-Location ".."
} else {
    Write-Host "⚠ Frontend directory not found or package.json missing" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Update configuration in src/BlogApplication.API/appsettings.json" -ForegroundColor White
Write-Host "   - Set your MySQL password" -ForegroundColor White
Write-Host "   - Set a secure JWT secret key" -ForegroundColor White
Write-Host ""
Write-Host "2. Start the backend API:" -ForegroundColor White
Write-Host "   cd src/BlogApplication.API" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. Start the frontend (in a new terminal):" -ForegroundColor White
Write-Host "   cd client" -ForegroundColor Cyan
Write-Host "   npm run dev" -ForegroundColor Cyan
Write-Host ""
Write-Host "4. Access the application:" -ForegroundColor White
Write-Host "   Frontend: http://localhost:3000" -ForegroundColor Cyan
Write-Host "   API: https://localhost:5001" -ForegroundColor Cyan
Write-Host "   Swagger: https://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "For detailed instructions, see README.md or QUICK_START.md" -ForegroundColor Yellow
Write-Host ""
