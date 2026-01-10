# Customer Create Test Guide - Complete Implementation

## Overview

This guide provides comprehensive testing strategies for the Customer/User creation functionality in the Biome Innovation Backend, following Clean Architecture principles with DynamoDB persistence using LocalStack.

## Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   API Layer     │    │  Application    │    │    Domain       │
│                 │    │     Layer       │    │     Layer       │
│ - Controllers   │    │ - Commands      │    │ - Entities      │
│ - HTTP Endpoints│    │ - Validators    │    │ - Value Objects │
│ - DTOs          │    │ - Handlers      │    │ - Domain Events │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                │
                       ┌─────────────────┐    ┌─────────────────┐
                       │ Infrastructure   │    │   SharedKernel  │
                       │     Layer       │    │                 │
                       │ - Repositories  │    │ - Primitives    │
                       │ - DynamoDB      │    │ - Abstractions  │
                       │ - External Svc  │    │ - Base Classes  │
                       └─────────────────┘    └─────────────────┘
```

## Testing Strategy

### 1. Unit Tests

#### Domain Layer Tests
- **Purpose**: Test business logic, invariants, and domain rules
- **Location**: `tests/Biome.UnitTests/Users/Domain/UserTests.cs`
- **Coverage**: 
  - Entity creation and validation
  - Business methods (UpdateProfile, Ban, VerifyEmail, etc.)
  - Value object creation
  - Domain events

#### Application Layer Tests
- **Purpose**: Test command/query handlers and validation
- **Location**: `tests/Biome.UnitTests/Users/Commands/RegisterUserCommandTests.cs`
- **Coverage**:
  - Command validation
  - Handler behavior
  - Error scenarios
  - Business rule enforcement

### 2. Integration Tests

#### API Integration Tests
- **Purpose**: Test end-to-end API functionality
- **Location**: `tests/Biome.IntegrationTests/Users/RegisterUserIntegrationTests.cs`
- **Coverage**:
  - HTTP endpoints
  - Request/response validation
  - Error handling
  - Database integration

#### Infrastructure Tests
- **Purpose**: Test persistence layer and external services
- **Location**: `tests/Biome.IntegrationTests/Infrastructure/`
- **Coverage**:
  - DynamoDB operations
  - Repository implementations
  - Configuration validation

## Test Data Management

### TestHelpers Project
- **Location**: `tests/Biome.TestHelpers/`
- **Purpose**: Reusable test data builders and factories
- **Key Components**:
  - `UserBuilder`: Builder pattern for User entities
  - `TestUserFactory`: Factory methods for common scenarios

### Example Usage:

```csharp
// Create a valid user
var user = TestUserFactory.CreateStandardUser();

// Create a custom user
var user = UserBuilder.ValidUser()
    .WithEmail("test@example.com")
    .WithFirstName("John")
    .WithLastName("Doe")
    .WithEmailVerified(true)
    .Build();
```

## LocalStack Configuration

### Setup Requirements
1. **Install LocalStack**:
   ```bash
   pip install localstack
   pip install localstack[aws]
   ```

2. **Start LocalStack**:
   ```bash
   localstack start -d
   ```

3. **Verify Services**:
   ```bash
   localstack status services
   ```

### Configuration
```json
{
  "Persistence": {
    "Provider": "DynamoDb",
    "LocalStack": {
      "EnsureTablesCreated": "true"
    }
  },
  "AWS": {
    "Region": "us-east-1",
    "AccessKey": "test",
    "SecretKey": "test",
    "ServiceURL": "http://localhost:8000"
  }
}
```

## Manual Testing

### HTTP Examples

#### Successful User Registration
```http
POST /api/users/register
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Expected Response (200 OK)**:
```json
{
  "success": true,
  "data": {
    "id": "guid-here",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isEmailVerified": false,
    "isBanned": false,
    "twoFactorEnabled": false
  },
  "message": "User registered successfully"
}
```

#### Validation Error - Invalid Email
```http
POST /api/users/register
Content-Type: application/json

{
  "email": "invalid-email",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Expected Response (400 Bad Request)**:
```json
{
  "success": false,
  "errors": [
    {
      "code": "ValidationFailed",
      "message": "Email is not a valid email address",
      "property": "Email"
    }
  ]
}
```

#### Duplicate Email
```http
POST /api/users/register
Content-Type: application/json

{
  "email": "existing@example.com",
  "password": "Password123!",
  "firstName": "Jane",
  "lastName": "Smith"
}
```

**Expected Response (409 Conflict)**:
```json
{
  "success": false,
  "errors": [
    {
      "code": "UserAlreadyExists",
      "message": "A user with this email already exists"
    }
  ]
}
```

## Running Tests

### Unit Tests
```bash
# Run all unit tests
dotnet test tests/Biome.UnitTests

# Run with coverage
dotnet test tests/Biome.UnitTests --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test tests/Biome.UnitTests --filter "FullyQualifiedName~UserTests"
```

### Integration Tests
```bash
# Run all integration tests
dotnet test tests/Biome.IntegrationTests

# Run with LocalStack (ensure LocalStack is running)
dotnet test tests/Biome.IntegrationTests --logger "console;verbosity=detailed"
```

### All Tests
```bash
# Run entire test suite
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

## Test Scenarios

### Positive Test Cases
- ✅ Valid user registration
- ✅ User with maximum length names
- ✅ User with Unicode characters in names
- ✅ Various valid email formats
- ✅ Strong password validation
- ✅ Profile update functionality
- ✅ Email verification
- ✅ Password reset functionality
- ✅ Two-factor authentication enable/disable

### Negative Test Cases
- ❌ Invalid email formats
- ❌ Weak passwords
- ❌ Empty/null names
- ❌ Too long names
- ❌ Duplicate emails
- ❌ Invalid passwords during change
- ❌ Invalid two-factor codes
- ❌ Expired password reset tokens
- ❌ Accessing banned user functionality

### Edge Cases
- 🔄 Concurrent user registrations
- 🔄 Email case sensitivity
- 🔄 Large data handling
- 🔄 Network timeouts
- 🔄 Database connection failures

## Best Practices

### Test Organization
1. **Arrange-Act-Assert** pattern consistently
2. **Descriptive test names** that explain the scenario
3. **Reusable test data** using builders and factories
4. **Independent tests** that don't rely on execution order
5. **Clear assertion messages** for debugging

### Test Data Management
1. **Use builders** for complex object creation
2. **Factories** for common test scenarios
3. **Avoid magic strings** - use constants or enums
4. **Clean up test data** in teardown methods
5. **Isolate test databases** for integration tests

### Assertion Strategies
1. **Be specific** - assert exact values, not just non-null
2. **Use FluentAssertions** for readable assertions
3. **Assert error types** and messages, not just failure
4. **Verify side effects** in addition to return values
5. **Check invariants** after operations

## Troubleshooting

### Common Issues

#### LocalStack Connection Issues
```bash
# Check if LocalStack is running
localstack status

# Restart LocalStack
localstack stop
localstack start -d

# Check DynamoDB specifically
aws dynamodb list-tables --endpoint-url http://localhost:8000
```

#### Test Database Issues
```bash
# Clear DynamoDB tables
aws dynamodb delete-table --table-name Users --endpoint-url http://localhost:8000

# Recreate tables
dotnet run --project src/Biome.Api --environment Development
```

#### Package Version Conflicts
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

### Performance Optimization
1. **Use Testcontainers** for isolated test environments
2. **Parallel test execution** where safe
3. **Reuse test fixtures** for expensive setup
4. **Mock external services** in unit tests
5. **Use in-memory databases** for fast unit tests

## Continuous Integration

### GitHub Actions Example
```yaml
name: Tests
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    services:
      localstack:
        image: localstack/localstack
        ports:
          - 4566:4566
          - 8000:8000
        env:
          SERVICES: dynamodb
          
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

## Metrics and Coverage

### Target Coverage Goals
- **Domain Layer**: 95%+ (critical business logic)
- **Application Layer**: 90%+ (commands, handlers, validators)
- **API Layer**: 85%+ (controllers, endpoints)
- **Infrastructure**: 80%+ (repositories, configurations)

### Coverage Reports
```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report (requires additional tools)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"coverage.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Conclusion

This comprehensive testing strategy ensures the User creation functionality is thoroughly tested across all architectural layers, providing confidence in the system's reliability and correctness. The combination of unit tests, integration tests, and manual testing creates a robust quality assurance process that catches issues early and prevents regressions.

The implementation follows Clean Architecture principles, ensuring that tests are maintainable, isolated, and provide fast feedback to developers during the development process.
