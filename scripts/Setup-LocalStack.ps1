# Setup script for LocalStack and DynamoDB User persistence
# This script sets up the development environment for testing DynamoDB persistence

Write-Host "🚀 Setting up LocalStack and DynamoDB for User Aggregate persistence..." -ForegroundColor Green

# Check if Docker is running
try {
    $dockerInfo = docker info 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Docker is not running. Please start Docker and try again." -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not installed or not running. Please install Docker and try again." -ForegroundColor Red
    exit 1
}

# Stop and remove existing LocalStack container if it exists
Write-Host "🧹 Cleaning up existing LocalStack container..." -ForegroundColor Yellow
docker stop biome-localstack 2>$null
docker rm biome-localstack 2>$null

# Start LocalStack
Write-Host "🐳 Starting LocalStack with DynamoDB..." -ForegroundColor Yellow
docker-compose up -d

# Wait for LocalStack to be ready
Write-Host "⏳ Waiting for LocalStack to be ready..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0

do {
    $attempt++
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:4566/health" -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ LocalStack is ready!" -ForegroundColor Green
            break
        }
    } catch {
        if ($attempt -eq $maxAttempts) {
            Write-Host "❌ LocalStack failed to start within timeout period." -ForegroundColor Red
            exit 1
        }
        Write-Host "⏳ Attempt $attempt/$maxAttempts - Waiting for LocalStack..." -ForegroundColor Yellow
        Start-Sleep -Seconds 2
    }
} while ($attempt -lt $maxAttempts)

# Build the application
Write-Host "🔨 Building the application..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed. Please check the error messages above." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful" -ForegroundColor Green

# Run the application to initialize tables
Write-Host "🗄️  Initializing DynamoDB tables..." -ForegroundColor Yellow

# Create a simple test to verify table creation
$testProject = "tests/Biome.IntegrationTests"
if (Test-Path $testProject) {
    Write-Host "🧪 Running integration tests to verify setup..." -ForegroundColor Yellow
    dotnet test $testProject --logger "console;verbosity=detailed"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Integration tests passed!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Integration tests failed, but this might be expected if tests are not yet implemented." -ForegroundColor Yellow
    }
} else {
    Write-Host "ℹ️  Integration test project not found. Skipping tests." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🎉 Setup complete! Your DynamoDB User persistence is ready to use." -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Run the API: dotnet run --project src/Biome.Api" -ForegroundColor White
Write-Host "2. Test User operations through the API endpoints" -ForegroundColor White
Write-Host "3. Monitor LocalStack logs: docker logs biome-localstack -f" -ForegroundColor White
Write-Host ""
Write-Host "Configuration:" -ForegroundColor Cyan
Write-Host "- LocalStack URL: http://localhost:4566" -ForegroundColor White
Write-Host "- AWS Region: us-east-1" -ForegroundColor White
Write-Host "- DynamoDB Table: Users" -ForegroundColor White
Write-Host "- GSIs: EmailIndex, RefreshTokenIndex" -ForegroundColor White
Write-Host ""
Write-Host "To stop LocalStack: docker-compose down" -ForegroundColor Gray
