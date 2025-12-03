# Banking API - DDD/CQRS Implementation

A comprehensive REST banking API built with ASP.NET Core 9, implementing Domain-Driven Design (DDD) and CQRS patterns.

## 🏗️ Architecture

### Clean Architecture Layers

```
├── Domain Layer          - Core business logic and rules
│   ├── Aggregates       - AccountHolder, Account, Card, Transaction
│   ├── Value Objects    - Money, CardNumber, EmailAddress, etc.
│   ├── Domain Events    - Business event notifications
│   ├── Specifications   - Business rule specifications
│   └── Repositories     - Persistence interfaces
│
├── Application Layer     - Use cases and orchestration
│   ├── Commands         - CQRS write operations
│   ├── Queries          - CQRS read operations
│   ├── Handlers         - MediatR request handlers
│   ├── DTOs             - Data transfer objects
│   └── Validators       - FluentValidation rules
│
├── Infrastructure Layer  - External concerns
│   ├── Persistence      - EF Core, SQLite
│   ├── Repositories     - Repository implementations
│   └── Authentication   - JWT token service with IP/UA binding
│
└── API Layer            - HTTP endpoints
    ├── Minimal APIs     - ASP.NET Core endpoints
    ├── Middleware       - Exception handling, logging
    └── Configuration    - Swagger, CORS, health checks
```

## 🚀 Features

### Core Functionality
- **Account Management**: Create and manage account holders and their accounts
- **Transaction Processing**: Transfer money between accounts with full audit trail
- **Card Management**: Issue, block, and validate payment cards
- **Authentication**: OAuth 2.0 / JWT with IP and User-Agent binding for enhanced security
- **Statement Generation**: Query transaction history with filtering

### Technical Features
- ✅ Domain-Driven Design with rich domain models
- ✅ CQRS pattern with MediatR
- ✅ Repository pattern with Unit of Work
- ✅ Domain events for audit trail
- ✅ Value objects with business validation
- ✅ Specification pattern for business rules
- ✅ JWT authentication with enhanced security (IP/User-Agent binding)
- ✅ Structured logging with Serilog
- ✅ Health checks for monitoring
- ✅ Swagger/OpenAPI documentation
- ✅ Docker containerization
- ✅ Unit tests with proper naming conventions
- ✅ SQLite database with EF Core migrations

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Language**: C# 14
- **Database**: SQLite with Entity Framework Core 9
- **Architecture**: DDD + CQRS
- **Patterns**: Repository, Unit of Work, Specification
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation
- **Mapping**: Mapster
- **Mediator**: MediatR
- **Logging**: Serilog
- **Testing**: xUnit, FluentAssertions
- **Containerization**: Docker

## 📦 Project Structure

```
bankapi_ai/
├── src/
│   ├── BankApi.Domain/
│   │   ├── Aggregates/
│   │   │   ├── AccountHolders/
│   │   │   ├── Accounts/
│   │   │   ├── Cards/
│   │   │   └── Transactions/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Specifications/
│   │   ├── Repositories/
│   │   └── Exceptions/
│   ├── BankApi.Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── DTOs/
│   │   └── Validators/
│   ├── BankApi.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Repositories/
│   │   └── Authentication/
│   └── BankApi.Api/
│       ├── Endpoints/
│       └── Middleware/
├── tests/
│   ├── BankApi.Domain.Tests/
│   ├── BankApi.Application.Tests/
│   └── BankApi.Integration.Tests/
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker (optional, for containerized deployment)

### Running Locally

1. **Clone the repository**
```bash
git clone https://github.com/yegorl/bankapi_ai.git
cd bankapi_ai
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Build the solution**
```bash
dotnet build
```

4. **Run the API**
```bash
cd src/BankApi.Api
dotnet run
```

5. **Access Swagger UI**
```
http://localhost:5000/swagger
```

### Running with Docker

1. **Build and run with Docker Compose**
```bash
docker-compose up --build
```

2. **Access the API**
```
http://localhost:5000
```

3. **Stop the containers**
```bash
docker-compose down
```

## 🧪 Testing

### Run all tests
```bash
dotnet test
```

### Run domain tests only
```bash
dotnet test tests/BankApi.Domain.Tests
```

### Test Coverage
- Domain unit tests following `MethodName_Condition_ExpectedResult()` naming convention
- 16/17 tests passing (94% pass rate)
- Tests cover:
  - AccountHolder aggregate
  - Account aggregate
  - Money value object
  - Business rule validation

## 📝 API Endpoints

### Authentication
- `POST /api/auth/signup` - Register new account holder
- `POST /api/auth/login` - Login and get JWT token

### Account Holders
- `POST /api/accountholders` - Create account holder
- `GET /api/accountholders/{id}` - Get account holder
- `PUT /api/accountholders/{id}` - Update account holder
- `DELETE /api/accountholders/{id}` - Delete account holder

### Accounts
- `POST /api/accounts` - Create account
- `GET /api/accounts/{id}` - Get account
- `GET /api/accounts/holder/{holderId}` - Get accounts by holder
- `PUT /api/accounts/{id}` - Update account
- `DELETE /api/accounts/{id}` - Delete account

### Cards
- `POST /api/cards` - Request new card
- `PUT /api/cards/{id}/block` - Block card
- `PUT /api/cards/{id}/block-temporary` - Temporarily block card
- `PUT /api/cards/{id}/unblock` - Unblock card
- `POST /api/cards/validate-cvv` - Validate CVV

### Transactions
- `POST /api/transactions` - Create transaction
- `POST /api/transactions/transfer` - Execute transfer
- `GET /api/transactions/{id}` - Get transaction
- `GET /api/statements` - Get account statement

### Health & Monitoring
- `GET /health` - Health check
- `GET /health/ready` - Readiness probe

## 🔒 Security Features

### JWT Authentication with Enhanced Binding
- OAuth 2.0 compliant JWT tokens
- Token includes: user ID, email, IP hash, User-Agent hash
- Middleware validates IP and User-Agent on each request
- Prevents token sharing across different devices/locations
- BCrypt/SHA256 for password and data hashing

### Data Protection
- CVV stored as hash only (never plain text)
- Sensitive data validation at domain level
- Decimal precision for monetary values (no data loss)
- Domain-level authorization checks

## 📊 Domain Model

### Aggregates

**AccountHolder** (AH-XXXXX)
- Personal information
- Contact details
- Multiple accounts

**Account** (ACC-XXXXXXXX)
- Links to account holder
- Balance (decimal precision)
- Currency
- Transaction history

**Card**
- 16-digit card number (Luhn validated)
- Links to account
- Expiration date
- Hashed CVV
- Block status

**Transaction**
- Source and target accounts
- Amount (decimal precision)
- Transaction type (Deposit, Withdrawal, Transfer)
- Status tracking
- Audit trail

## 🎯 Design Patterns

### Domain-Driven Design (DDD)
- Aggregates as consistency boundaries
- Value objects for business concepts
- Domain events for side effects
- Specifications for business rules
- Ubiquitous language throughout

### CQRS
- Separate command and query models
- MediatR for command/query bus
- Optimized read and write paths

### Repository Pattern
- Abstraction over data access
- Unit of Work for transactions
- Specification pattern for queries

## 🔧 Configuration

Key configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bankapi.db"
  },
  "JwtSettings": {
    "Secret": "YourSecretKey",
    "Issuer": "BankingAPI",
    "Audience": "BankingAPIUsers",
    "ExpirationMinutes": 60
  }
}
```

## 📝 Logging

Structured logging with Serilog:
- Console and file sinks
- Request logging with correlation IDs
- Structured event data
- Log levels: Debug, Information, Warning, Error

## 🐛 Error Handling

- Global exception middleware
- Domain exceptions for business rule violations
- Validation exceptions with detailed messages
- Problem Details (RFC 7807) responses

## 🤝 Contributing

1. Follow DDD and SOLID principles
2. Use `MethodName_Condition_ExpectedResult()` test naming
3. Maintain 85%+ test coverage
4. Follow existing code style
5. Update documentation

## 📄 License

This project is for educational purposes.

## 👥 Authors

- Implementation following DDD/CQRS best practices
- Comprehensive banking domain model
- Production-ready architecture patterns
