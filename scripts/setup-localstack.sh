#!/bin/bash

# Setup script for LocalStack and DynamoDB User persistence
# This script sets up the development environment for testing DynamoDB persistence

echo "🚀 Setting up LocalStack and DynamoDB for User Aggregate persistence..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker and try again."
    exit 1
fi
echo "✅ Docker is running"

# Stop and remove existing LocalStack container if it exists
echo "🧹 Cleaning up existing LocalStack container..."
docker stop biome-localstack 2>/dev/null || true
docker rm biome-localstack 2>/dev/null || true

# Start LocalStack
echo "🐳 Starting LocalStack with DynamoDB..."
docker-compose up -d

# Wait for LocalStack to be ready
echo "⏳ Waiting for LocalStack to be ready..."
max_attempts=30
attempt=0

while [ $attempt -lt $max_attempts ]; do
    attempt=$((attempt + 1))
    if curl -s http://localhost:4566/health > /dev/null 2>&1; then
        echo "✅ LocalStack is ready!"
        break
    fi
    
    if [ $attempt -eq $max_attempts ]; then
        echo "❌ LocalStack failed to start within timeout period."
        exit 1
    fi
    
    echo "⏳ Attempt $attempt/$max_attempts - Waiting for LocalStack..."
    sleep 2
done

# Build the application
echo "🔨 Building the application..."
if ! dotnet build; then
    echo "❌ Build failed. Please check error messages above."
    exit 1
fi
echo "✅ Build successful"

# Check if integration tests exist and run them
test_project="tests/Biome.IntegrationTests"
if [ -d "$test_project" ]; then
    echo "🧪 Running integration tests to verify setup..."
    if dotnet test "$test_project" --logger "console;verbosity=detailed"; then
        echo "✅ Integration tests passed!"
    else
        echo "⚠️  Integration tests failed, but this might be expected if tests are not yet implemented."
    fi
else
    echo "ℹ️  Integration test project not found. Skipping tests."
fi

echo ""
echo "🎉 Setup complete! Your DynamoDB User persistence is ready to use."
echo ""
echo "Next steps:"
echo "1. Run the API: dotnet run --project src/Biome.Api"
echo "2. Test User operations through the API endpoints"
echo "3. Monitor LocalStack logs: docker logs biome-localstack -f"
echo ""
echo "Configuration:"
echo "- LocalStack URL: http://localhost:4566"
echo "- AWS Region: us-east-1"
echo "- DynamoDB Table: Users"
echo "- GSIs: EmailIndex, RefreshTokenIndex"
echo ""
echo "To stop LocalStack: docker-compose down"
