# DynamoDB Persistence Setup for User Aggregate

This document describes how to set up and use DynamoDB persistence for the User Aggregate Root in the Biome Innovation backend application.

## Architecture Overview

The implementation follows Clean Architecture principles:

- **Domain Layer**: Contains the User aggregate root and repository interface (no DynamoDB dependencies)
- **Infrastructure Layer**: Contains DynamoDB-specific implementations, mappings, and configurations
- **LocalStack Support**: Full local development environment with DynamoDB emulation

## Features Implemented

### 1. DynamoDB Entity Model
- `UserDynamoDbEntity`: Maps User domain entity to DynamoDB table structure
- Proper handling of value objects (Email, RefreshToken, PasswordReset)
- Global Secondary Indexes for efficient queries:
  - `EmailIndex`: For user lookup by email
  - `RefreshTokenIndex`: For token-based authentication

### 2. Repository Implementation
- `DynamoDbUserRepository`: Implements `IUserRepository` interface
- Supports all required operations:
  - `GetByIdAsync`: Retrieve user by ID
  - `GetByEmailAsync`: Retrieve user by email (using GSI)
  - `GetByRefreshTokenAsync`: Retrieve user by refresh token (using GSI)
  - `Add`: Create new user
- Implements `IUnitOfWork` for transaction coordination

### 3. Configuration Management
- Environment-aware configuration (Development vs Production)
- LocalStack support for local development
- Automatic table creation in development environment

## Development Setup

### Prerequisites
- Docker and Docker Compose
- .NET 10.0 SDK
- AWS CLI (optional, for production deployment)

### Step 1: Start LocalStack
```bash
docker-compose up -d
```

This will start LocalStack with DynamoDB service on port 4566.

### Step 2: Update Configuration
The application is already configured for DynamoDB in both environments:

#### Development (appsettings.Development.json)
```json
{
  "Persistence": {
    "Provider": "DynamoDb",
    "LocalStack": {
      "Enabled": true,
      "EnsureTablesCreated": true,
      "ServiceUrl": "http://localhost:4566",
      "Region": "us-east-1"
    }
  },
  "AWS": {
    "Region": "us-east-1",
    "ServiceURL": "http://localhost:4566"
  }
}
```

#### Production (appsettings.json)
```json
{
  "Persistence": {
    "Provider": "DynamoDb",
    "LocalStack": {
      "Enabled": false,
      "EnsureTablesCreated": false
    }
  },
  "AWS": {
    "Region": "us-east-1"
  }
}
```

### Step 3: Run the Application
```bash
dotnet run --project src/Biome.Api
```

The application will:
1. Connect to LocalStack (development) or AWS DynamoDB (production)
2. Automatically create the Users table and GSIs (if `EnsureTablesCreated` is true)
3. Start using DynamoDB for User persistence

## Table Schema

### Users Table
- **Table Name**: `Users`
- **Primary Key**: `Id` (String representation of Guid)

### Global Secondary Indexes

#### EmailIndex
- **Partition Key**: `EmailIndex` (Email value)
- **Projection**: ALL
- **Purpose**: Efficient user lookup by email

#### RefreshTokenIndex
- **Partition Key**: `RefreshTokenIndex` (RefreshToken value)
- **Projection**: ALL
- **Purpose**: Efficient user lookup by refresh token

## Clean Architecture Compliance

### Domain Layer (No Infrastructure Dependencies)
- `User`: Aggregate root with business logic
- `IUserRepository`: Repository interface
- Value objects: `Email`, `FirstName`, `LastName`, `RefreshToken`, `PasswordReset`

### Infrastructure Layer (Technology-Specific)
- `UserDynamoDbEntity`: DynamoDB-specific entity model
- `DynamoDbUserRepository`: DynamoDB implementation
- `UserMapper`: Conversion between domain and persistence models
- Configuration and table initialization

## Testing Strategy

### Unit Tests
- Test `UserMapper` for correct entity conversion
- Test repository methods with mocked `IDynamoDBContext`

### Integration Tests
- Use LocalStack for full integration testing
- Test complete CRUD operations
- Verify GSI functionality

### Example Integration Test Setup
```csharp
// In your test setup
services.AddPersistence(testConfiguration);
var tableInitializer = services.GetService<DynamoDbTableInitializer>();
await tableInitializer!.InitializeAsync();
```

## Production Deployment

### AWS DynamoDB Setup
1. Create DynamoDB table in AWS Console
2. Configure GSIs (EmailIndex, RefreshTokenIndex)
3. Set appropriate read/write capacity
4. Update IAM policies for application access
5. Configure environment variables for production

### Configuration for Production
- Set `Persistence:LocalStack:Enabled` to `false`
- Set `Persistence:LocalStack:EnsureTablesCreated` to `false`
- Configure AWS credentials properly
- Set appropriate AWS region

## Monitoring and Troubleshooting

### Local Logs
Monitor LocalStack container logs:
```bash
docker logs biome-localstack -f
```

### Application Logs
Enable detailed logging:
```json
{
  "Logging": {
    "Amazon": "Debug",
    "Biome.Infrastructure.Persistence": "Debug"
  }
}
```

### Common Issues
1. **Connection Refused**: Ensure LocalStack is running
2. **Table Not Found**: Check `EnsureTablesCreated` setting
3. **Permission Issues**: Verify AWS credentials for production

## Performance Considerations

### Development (LocalStack)
- Suitable for development and testing
- Limited performance compared to AWS DynamoDB
- No cost implications

### Production (AWS DynamoDB)
- Configure appropriate read/write capacity
- Consider DynamoDB On-Demand for variable workloads
- Monitor CloudWatch metrics for performance optimization

## Security Considerations

### Development
- LocalStack provides isolated environment
- No real data at risk

### Production
- Use IAM roles with least privilege
- Enable DynamoDB encryption at rest
- Configure VPC endpoints if required
- Monitor access patterns

## Future Enhancements

1. **Event Sourcing**: Implement domain event persistence
2. **Caching**: Add Redis caching for frequently accessed users
3. **Optimistic Locking**: Implement version checking for concurrent updates
4. **Batch Operations**: Add bulk operations for improved performance
5. **Data Migration**: Implement strategies for data migration between environments

## Support

For issues or questions regarding the DynamoDB implementation:
1. Check LocalStack logs for connection issues
2. Verify AWS configuration for production issues
3. Review application logs for detailed error information
4. Ensure all prerequisites are properly installed and configured
