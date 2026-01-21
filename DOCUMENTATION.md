# Merchant Payment & Onboarding System

## 1. Project Overview
The **Merchant Payment & Onboarding System** is a robust RESTful API built with **.NET 10**. It facilitates the complete lifecycle of merchant management and payment processing, including user registration, merchant onboarding, secure payment transaction handling, and fraud detection.

The system is designed using **Clean Architecture** principles to ensuring maintainability, testability, and separation of concerns.

## 2. Key Features
- **Merchant Onboarding**: Complete workflow for registering, updating, and approving merchants.
- **Payment Processing**: Secure transaction processing with idempotency checks to prevent duplicate charges.
- **Fraud Detection**: Built-in velocity checks to flag suspicious transaction patterns.
- **Security**: 
  - JWT (JSON Web Token) authentication for secure API access.
  - BCrypt password hashing for user credential security.
- **Audit Logging**: Detailed tracking of all changes to transaction statuses.
- **Containerization**: Fully Dockerized for easy deployment and consistency across environments.

## 3. Architecture
The solution follows the **Clean Architecture** pattern, divided into four distinct layers:

1.  **Domain Layer**
    - The core of the application containing business entities (`Merchant`, `PaymentTransaction`, `User`) and enums.
    - Has **zero** external dependencies.

2.  **Application Layer**
    - Contains business logic, service implementations, DTOs (Data Transfer Objects), and interface definitions.
    - Orchestrates data flow between the UI and the underlying infrastructure.

3.  **Infrastructure Layer**
    - Implements interfaces defined in the Application layer.
    - Manages data access using **Entity Framework Core** and **MySQL**.
    - Handles database migrations and repository implementations.

4.  **API Layer**
    - The entry point of the application (REST Controllers).
    - Handles HTTP requests, middleware (exception handling), and dependency injection configuration.

## 4. Technology Stack
- **Framework**: .NET 10 (C#)
- **Database**: MySQL 8.0
- **ORM**: Entity Framework Core 10.0.2 (using `MySql.EntityFrameworkCore` provider)
- **Authentication**: JWT Bearer Authentication
- **Documentation**: Swagger / OpenApi
- **Testing**: xUnit, Moq, FluentAssertions
- **Deployment**: Docker & Docker Compose

## 5. API Endpoints Overview
The API is organized into three main controllers:

### Authentication (`/api/auth`)
- `POST /register`: Register a new user.
- `POST /login`: Authenticate and receive a JWT token.

### Merchants (`/api/merchants`)
- `POST /`: Create a new merchant application.
- `GET /`: List all merchants.
- `GET /{id}`: Retrieve merchant details.
- `PUT /{id}`: Update merchant info.
- `DELETE /{id}`: Remove a merchant.

### Payments (`/api/payments`)
- `POST /process`: Initiate a new payment transaction.
- `GET /{id}`: Get status and details of a specific payment.

> **Note**: Full API details (request/response schemas) are available in the Swagger UI.

## 6. Getting Started

### Prerequisites
- **Docker Desktop** (recommended)
- Or **.NET 10 SDK** and a running **MySQL** instance.

### Running with Docker
The easiest way to run the application is using Docker Compose:

```bash
# Build and start the services
docker-compose up --build
```

- **API URL**: `http://localhost:8080`
- **Swagger UI**: `http://localhost:8080/docs`

### Running Locally (Manual)
1.  **Configure Database**: Update the connection string in `src/MerchantPayment.API/appsettings.json`.
2.  **Apply Migrations**:
    ```bash
    dotnet ef database update --project src/MerchantPayment.Infrastructure --startup-project src/MerchantPayment.API
    ```
3.  **Run Application**:
    ```bash
    dotnet run --project src/MerchantPayment.API
    ```

## 7. Testing
The solution includes a comprehensive unit test suite covering services and business logic.

```bash
# Run all tests
dotnet test
```
