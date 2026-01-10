# Biome Innovation Backend - Test Suite

This directory contains comprehensive test suites for the Biome Innovation backend system, focusing on Customer/User creation functionality.

## 📁 Test Structure

```
tests/
├── Biome.UnitTests/                    # Unit tests for individual components
│   └── Users/
│       ├── Commands/
│       │   └── RegisterUserCommandTests.cs
│       └── Domain/
│           └── UserTests.cs
├── Biome.IntegrationTests/              # Integration tests with real dependencies
│   └── Users/
│       └── RegisterUserIntegrationTests.cs
├── Biome.TestHelpers/                  # Test utilities and builders
│   └── UserBuilder.cs
└── Customer-Create-Test-Guide.md       # Comprehensive testing guide
```

## 🚀 Quick Start

### Prerequisites
1. **Docker** - For LocalStack (DynamoDB emulation)
2. **.NET 10.0** - Runtime and SDK
3. **Test Explorer** - VS Code, Visual Studio, or JetBrains Rider

### Running Tests

#### All Tests
```bash
dotnet test
```

#### Specific Test Projects
```bash
# Unit tests only
dotnet test tests/Biome.UnitTests

# Integration tests only
dotnet test tests/Biome.IntegrationTests
```

#### With Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

#### Specific Test
```bash
dotnet test --filter "Should_Create_User_With_Valid_Data"
```

### Local Development Setup

#### Start LocalStack (for integration tests)
```bash
# From project root
docker-compose up -d

# Verify it's running
curl http://localhost:4566/health
```

#### Run Tests with LocalStack
```bash
# Integration tests will automatically use LocalStack
dotnet test tests/Biome.IntegrationTests
```

## 📋 Test Coverage Areas

### ✅ Implemented Tests

#### Unit Tests
- **Command Validation**: RegisterUserCommand validation rules
- **Domain Logic**: User entity behavior and business rules
- **Value Objects**: Email validation and creation

#### Integration Tests
- **API Endpoints**: HTTP request/response testing
- **Database Operations**: DynamoDB persistence with LocalStack
- **Error Handling**: Validation and error scenarios

#### Manual Testing
- **HTTP Requests**: Comprehensive .http file with 25+ test scenarios
- **Edge Cases**: Security, performance, and boundary testing
- **Authentication**: Authorization and token-based scenarios

### 🎯 Test Scenarios

#### Happy Path
- ✅ Valid user registration
- ✅ Admin user creation
- ✅ Profile updates
- ✅ Password changes

#### Validation
- ✅ Invalid email formats
- ✅ Weak passwords
- ✅ Missing required fields
- ✅ Invalid role assignments

#### Security
- ✅ SQL injection attempts
- ✅ XSS prevention
- ✅ Authorization checks
- ✅ Token validation

#### Edge Cases
- ✅ Unicode characters
- ✅ Long input values
- ✅ Special characters
- ✅ Duplicate email registration

## 🔧 Configuration

### Test Settings
Tests use in-memory configuration for LocalStack:
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

### Environment Variables
```bash
# Optional: Override LocalStack URL
AWS_SERVICE_URL=http://localhost:8000

# Optional: Use real AWS for testing
AWS_ACCESS_KEY_ID=your-key
AWS_SECRET_ACCESS_KEY=your-secret
AWS_REGION=us-east-1
```

## 📊 Test Data Management

### UserBuilder Pattern
```csharp
// Create test users easily
var user = new UserBuilder()
    .WithEmail("test@example.com")
    .WithFirstName("John")
    .WithLastName("Doe")
    .WithEmailVerified(true)
    .Build();

// Factory methods for common scenarios
var admin = TestUserFactory.CreateAdminUser();
var banned = TestUserFactory.CreateBannedUser();
```

### Database Cleanup
Integration tests automatically clean up after each test to ensure isolation.

## 🔍 Test Categories

### Unit Tests (`Biome.UnitTests`)
- **Purpose**: Test individual components in isolation
- **Dependencies**: Mocked using Moq/FakeItEasy
- **Speed**: Fast (milliseconds)
- **Scope**: Business logic, validation, domain rules

### Integration Tests (`Biome.IntegrationTests`)
- **Purpose**: Test component interactions
- **Dependencies**: Real (LocalStack, Testcontainers)
- **Speed**: Medium (seconds)
- **Scope**: API endpoints, database operations

### Manual Tests (`.http` file)
- **Purpose**: Human validation and exploratory testing
- **Dependencies**: Running application
- **Speed**: Manual
- **Scope**: User experience, edge cases

## 🚨 Troubleshooting

### Common Issues

#### LocalStack Connection Issues
```bash
# Check if running
docker ps | grep localstack

# Restart
docker-compose down && docker-compose up -d

# Check logs
docker-compose logs localstack
```

#### DynamoDB Table Issues
```bash
# List tables
aws dynamodb list-tables --endpoint-url http://localhost:4566

# Create manually if needed
aws dynamodb create-table \
  --table-name Users \
  --attribute-definitions AttributeName=Id,AttributeType=S \
  --key-schema AttributeName=Id,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --endpoint-url http://localhost:4566
```

#### Test Isolation Issues
- Tests should clean up after themselves
- Use unique test data (different emails)
- Check for parallel test execution conflicts

## 📈 Best Practices

1. **Test Naming**: Use descriptive names that explain the scenario
2. **AAA Pattern**: Arrange, Act, Assert structure
3. **Test Data**: Use builders and factories for consistency
4. **Cleanup**: Always clean up test data
5. **Isolation**: Tests should not depend on each other
6. **Assertions**: Be specific about what you're testing

## 🔄 CI/CD Integration

### GitHub Actions Example
```yaml
- name: Run Tests
  run: |
    docker-compose up -d
    dotnet test --collect:"XPlat Code Coverage"
    docker-compose down
```

### Test Reports
- Coverage reports generated in `TestResults/`
- Use tools like ReportGenerator for HTML reports
- Integrate with code coverage badges

## 📚 Additional Resources

- [Customer Create Test Guide](Customer-Create-Test-Guide.md) - Comprehensive testing guide
- [LocalStack Documentation](https://docs.localstack.cloud/) - Local AWS services
- [xUnit Documentation](https://xunit.net/docs/) - Testing framework
- [FluentAssertions](https://fluentassertions.com/) - Assertion library

## 🤝 Contributing

When adding new tests:

1. Follow the existing folder structure
2. Use the UserBuilder for test data
3. Include both positive and negative test cases
4. Add cleanup for integration tests
5. Update this README if adding new test categories

## 📞 Support

For testing-related questions:
1. Check the [Comprehensive Test Guide](Customer-Create-Test-Guide.md)
2. Review existing test examples
3. Check troubleshooting section
4. Contact the development team

---

**Last Updated**: January 2026
**Test Framework**: xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing
**Infrastructure**: LocalStack, Docker, DynamoDB
