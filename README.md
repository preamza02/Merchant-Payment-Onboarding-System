# 🏪 Merchant Payment & Onboarding System

A .NET 10 Web API for merchant registration and payment processing.

## 🚀 Quick Start

### Option 1: Docker (Recommended)
```bash
# Start everything (MySQL + API)
make docker-up

# Open Swagger UI
make swagger
```

### Option 2: Local Development
```bash
# Start MySQL only
make db-start

# Run the API locally
make run

# Open Swagger UI
make swagger
```

## 📖 Accessing Swagger UI

Once the API is running, open your browser and go to:

**http://localhost:5000/swagger**

Swagger UI lets you:
- See all available endpoints
- Try API calls directly from the browser
- View request/response schemas

## 🧪 Testing the API

### Using VS Code REST Client
1. Install the "REST Client" extension in VS Code
2. Open files in `http-tests/` folder
3. Click "Send Request" above each request

Test files:
- `01-auth.http` - Register and login
- `02-merchants.http` - Merchant operations
- `03-payments.http` - Payment operations
- `04-complete-e2e-workflow.http` - Full end-to-end test

### Using curl
```bash
# Health check
curl http://localhost:5000/health

# Register user
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"Password123!"}'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Password123!"}'
```

## 📋 Available Make Commands

Run `make help` to see all commands:

```bash
make help           # Show all commands
make build          # Build the project
make run            # Run locally
make test           # Run tests
make docker-up      # Start with Docker
make docker-down    # Stop Docker
make swagger        # Open Swagger UI
make logs           # View logs
```

## 📁 Project Structure

```
├── src/
│   ├── MerchantPayment.Domain/        # Entities
│   ├── MerchantPayment.Application/   # Business Logic
│   ├── MerchantPayment.Infrastructure/# Database
│   └── MerchantPayment.API/           # Web API
├── tests/
│   └── MerchantPayment.Tests/         # Unit Tests
├── http-tests/                        # API Test Files
├── Dockerfile
├── docker-compose.yml
└── Makefile
```

## 🔑 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get token |

### Merchants
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/merchants` | Register merchant |
| GET | `/api/merchants` | List all merchants |
| GET | `/api/merchants/{id}` | Get merchant by ID |
| PUT | `/api/merchants/{id}/status` | Update status |

### Payments
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/payments` | Initiate payment |
| GET | `/api/payments/{id}` | Get transaction |
| GET | `/api/payments/merchant/{id}` | Get merchant transactions |
| POST | `/api/payments/callback` | Process callback |

### Health
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | API health check |

## 🐳 Docker

### Start
```bash
docker-compose up -d --build
```

### Stop
```bash
docker-compose down
```

### View Logs
```bash
docker-compose logs -f api
```

### Clean Everything
```bash
docker-compose down -v --rmi all
```

## ⚙️ Configuration

Environment variables (in docker-compose.yml):
- `ConnectionStrings__DefaultConnection` - Database connection
- `Jwt__Secret` - JWT signing key (min 32 chars)
- `Jwt__Issuer` - Token issuer
- `Jwt__Audience` - Token audience

## 🧪 Running Tests

```bash
# Run all tests
make test

# Run with verbose output
make test-verbose

# Run with coverage
make test-coverage
```