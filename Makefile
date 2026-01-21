# ============================================================
# MAKEFILE FOR MERCHANT PAYMENT SYSTEM
# ============================================================
#
# A Makefile contains "recipes" for common tasks.
# Run them using: make <command>
#
# Example: make build
#          make run
#          make test
#
# TIP: Run `make help` to see all available commands!
# ============================================================

# Variables (can be overridden: make run PORT=3000)
PROJECT_NAME = MerchantPaymentSystem
API_PROJECT = src/MerchantPayment.API/MerchantPayment.API.csproj
TEST_PROJECT = tests/MerchantPayment.Tests/MerchantPayment.Tests.csproj
DOCKER_IMAGE = merchant-payment-api
PORT = 5000

# Colors for pretty output
GREEN = \033[0;32m
YELLOW = \033[0;33m
BLUE = \033[0;34m
RED = \033[0;31m
NC = \033[0m # No Color

# ============================================================
# HELP (Default target)
# ============================================================
.PHONY: help
help:
	@echo ""
	@echo "$(BLUE)╔══════════════════════════════════════════════════════════════╗$(NC)"
	@echo "$(BLUE)║     MERCHANT PAYMENT SYSTEM - MAKEFILE COMMANDS              ║$(NC)"
	@echo "$(BLUE)╚══════════════════════════════════════════════════════════════╝$(NC)"
	@echo ""
	@echo "$(GREEN)BUILD & RUN$(NC)"
	@echo "  make build          - Build the entire solution"
	@echo "  make run            - Run the API locally (http://localhost:5000)"
	@echo "  make watch          - Run with hot reload (auto-restart on changes)"
	@echo "  make run-release    - Run in Release mode (optimized)"
	@echo ""
	@echo "$(GREEN)TESTING$(NC)"
	@echo "  make test           - Run all unit tests"
	@echo "  make test-verbose   - Run tests with detailed output"
	@echo "  make test-coverage  - Run tests with code coverage report"
	@echo ""
	@echo "$(GREEN)DOCKER$(NC)"
	@echo "  make docker-build   - Build Docker image only"
	@echo "  make docker-up      - Start all containers (MySQL + API)"
	@echo "  make docker-down    - Stop and remove all containers"
	@echo "  make docker-logs    - View container logs"
	@echo "  make docker-restart - Restart all containers"
	@echo "  make docker-clean   - Remove containers, images, and volumes"
	@echo ""
	@echo "$(GREEN)DATABASE$(NC)"
	@echo "  make db-start       - Start MySQL container only"
	@echo "  make db-stop        - Stop MySQL container"
	@echo "  make db-connect     - Connect to MySQL with CLI"
	@echo "  make migrate        - Apply EF Core migrations"
	@echo "  make migration      - Create a new migration (NAME=YourMigration)"
	@echo ""
	@echo "$(GREEN)DEVELOPMENT$(NC)"
	@echo "  make restore        - Restore NuGet packages"
	@echo "  make clean          - Clean build artifacts"
	@echo "  make format         - Format code using dotnet format"
	@echo "  make swagger        - Open Swagger UI in browser"
	@echo "  make health         - Check if API is healthy"
	@echo ""
	@echo "$(GREEN)UTILITIES$(NC)"
	@echo "  make info           - Show project information"
	@echo "  make ports          - Show which ports are in use"
	@echo "  make logs           - Show API logs (when running in Docker)"
	@echo ""
	@echo "$(YELLOW)TIP: Run 'make docker-up' then 'make swagger' to start testing!$(NC)"
	@echo ""

# ============================================================
# BUILD & RUN COMMANDS
# ============================================================

.PHONY: build
build:
	@echo "$(GREEN)🔨 Building solution...$(NC)"
	dotnet build

.PHONY: run
run:
	@echo "$(GREEN)🚀 Starting API on http://localhost:$(PORT)...$(NC)"
	@echo "$(YELLOW)📖 Swagger UI: http://localhost:$(PORT)/swagger$(NC)"
	@echo "$(YELLOW)Press Ctrl+C to stop$(NC)"
	dotnet run --project $(API_PROJECT) --urls "http://localhost:$(PORT)"

.PHONY: watch
watch:
	@echo "$(GREEN)👀 Starting API with hot reload...$(NC)"
	@echo "$(YELLOW)📖 Swagger UI: http://localhost:$(PORT)/swagger$(NC)"
	@echo "$(YELLOW)Code changes will auto-restart the app$(NC)"
	dotnet watch run --project $(API_PROJECT) --urls "http://localhost:$(PORT)"

.PHONY: run-release
run-release:
	@echo "$(GREEN)🚀 Starting API in Release mode...$(NC)"
	dotnet run --project $(API_PROJECT) -c Release --urls "http://localhost:$(PORT)"

# ============================================================
# TESTING COMMANDS
# ============================================================

.PHONY: test
test:
	@echo "$(GREEN)🧪 Running all tests...$(NC)"
	dotnet test

.PHONY: test-verbose
test-verbose:
	@echo "$(GREEN)🧪 Running tests with verbose output...$(NC)"
	dotnet test --verbosity normal

.PHONY: test-coverage
test-coverage:
	@echo "$(GREEN)📊 Running tests with coverage...$(NC)"
	dotnet test --collect:"XPlat Code Coverage"
	@echo "$(YELLOW)Coverage report generated in TestResults folder$(NC)"

# ============================================================
# DOCKER COMMANDS
# ============================================================

.PHONY: docker-build
docker-build:
	@echo "$(GREEN)🐳 Building Docker image...$(NC)"
	docker-compose build

.PHONY: docker-up
docker-up:
	@echo "$(GREEN)🐳 Starting Docker containers...$(NC)"
	docker-compose up -d --build
	@echo ""
	@echo "$(GREEN)✅ Containers started!$(NC)"
	@echo "$(YELLOW)📖 API: http://localhost:$(PORT)$(NC)"
	@echo "$(YELLOW)📖 Swagger UI: http://localhost:$(PORT)/swagger$(NC)"
	@echo "$(YELLOW)📖 MySQL: localhost:3306$(NC)"
	@echo ""
	@echo "$(BLUE)Run 'make docker-logs' to view logs$(NC)"

.PHONY: docker-down
docker-down:
	@echo "$(RED)🛑 Stopping Docker containers...$(NC)"
	docker-compose down

.PHONY: docker-logs
docker-logs:
	docker-compose logs -f

.PHONY: docker-restart
docker-restart:
	@echo "$(YELLOW)🔄 Restarting Docker containers...$(NC)"
	docker-compose restart

.PHONY: docker-clean
docker-clean:
	@echo "$(RED)🧹 Removing all containers, images, and volumes...$(NC)"
	docker-compose down -v --rmi all
	@echo "$(GREEN)✅ Cleaned!$(NC)"

# ============================================================
# DATABASE COMMANDS
# ============================================================

.PHONY: db-start
db-start:
	@echo "$(GREEN)🗄️  Starting MySQL container...$(NC)"
	docker-compose up -d mysql
	@echo "$(YELLOW)MySQL available at localhost:3306$(NC)"

.PHONY: db-stop
db-stop:
	@echo "$(RED)🛑 Stopping MySQL container...$(NC)"
	docker-compose stop mysql

.PHONY: db-connect
db-connect:
	@echo "$(GREEN)🔗 Connecting to MySQL...$(NC)"
	@echo "$(YELLOW)Password: rootpassword$(NC)"
	docker exec -it merchant-payment-mysql mysql -u root -p

.PHONY: migrate
migrate:
	@echo "$(GREEN)📦 Applying EF Core migrations...$(NC)"
	dotnet ef database update --project src/MerchantPayment.Infrastructure --startup-project src/MerchantPayment.API

.PHONY: migration
migration:
ifndef NAME
	@echo "$(RED)❌ Please provide a migration name: make migration NAME=YourMigrationName$(NC)"
else
	@echo "$(GREEN)📦 Creating migration: $(NAME)...$(NC)"
	dotnet ef migrations add $(NAME) --project src/MerchantPayment.Infrastructure --startup-project src/MerchantPayment.API
endif

# ============================================================
# DEVELOPMENT COMMANDS
# ============================================================

.PHONY: restore
restore:
	@echo "$(GREEN)📦 Restoring NuGet packages...$(NC)"
	dotnet restore

.PHONY: clean
clean:
	@echo "$(RED)🧹 Cleaning build artifacts...$(NC)"
	dotnet clean
	find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true
	find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true
	@echo "$(GREEN)✅ Cleaned!$(NC)"

.PHONY: format
format:
	@echo "$(GREEN)✨ Formatting code...$(NC)"
	dotnet format

.PHONY: swagger
swagger:
	@echo "$(GREEN)🌐 Opening Swagger UI...$(NC)"
	@echo "$(YELLOW)If browser doesn't open, go to: http://localhost:$(PORT)/swagger$(NC)"
	@xdg-open http://localhost:$(PORT)/swagger 2>/dev/null || open http://localhost:$(PORT)/swagger 2>/dev/null || echo "Please open http://localhost:$(PORT)/swagger in your browser"

.PHONY: health
health:
	@echo "$(GREEN)💓 Checking API health...$(NC)"
	@curl -s http://localhost:$(PORT)/health || echo "$(RED)API is not running$(NC)"

# ============================================================
# UTILITY COMMANDS
# ============================================================

.PHONY: info
info:
	@echo ""
	@echo "$(BLUE)╔══════════════════════════════════════════════════════════════╗$(NC)"
	@echo "$(BLUE)║                  PROJECT INFORMATION                         ║$(NC)"
	@echo "$(BLUE)╚══════════════════════════════════════════════════════════════╝$(NC)"
	@echo ""
	@echo "$(GREEN)Project:$(NC) $(PROJECT_NAME)"
	@echo "$(GREEN).NET Version:$(NC)"
	@dotnet --version
	@echo ""
	@echo "$(GREEN)Projects in Solution:$(NC)"
	@dotnet sln list
	@echo ""
	@echo "$(GREEN)Docker Status:$(NC)"
	@docker-compose ps 2>/dev/null || echo "Docker Compose not running"
	@echo ""

.PHONY: ports
ports:
	@echo "$(GREEN)🔌 Checking ports...$(NC)"
	@echo ""
	@echo "$(YELLOW)Port 5000 (API):$(NC)"
	@lsof -i :5000 2>/dev/null || echo "  Not in use"
	@echo ""
	@echo "$(YELLOW)Port 3306 (MySQL):$(NC)"
	@lsof -i :3306 2>/dev/null || echo "  Not in use"
	@echo ""

.PHONY: logs
logs:
	@echo "$(GREEN)📋 API Logs:$(NC)"
	docker-compose logs -f api

# ============================================================
# COMBINED WORKFLOWS
# ============================================================

.PHONY: dev
dev: db-start run
	@echo "$(GREEN)Development environment ready!$(NC)"

.PHONY: fresh-start
fresh-start: docker-clean docker-up
	@echo "$(GREEN)Fresh start complete!$(NC)"

.PHONY: all
all: clean restore build test
	@echo "$(GREEN)✅ All checks passed!$(NC)"
