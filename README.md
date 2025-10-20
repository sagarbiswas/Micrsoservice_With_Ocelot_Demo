# Microservices Solution with Event-Driven Architecture

This solution demonstrates a microservices architecture using:
- **CQRS** with MediatR
- **Event-Driven Architecture** with Kafka
- **API Gateway** with Ocelot
- **SQL Server** for data persistence
- **WebAPI** for REST endpoints

## Architecture

```
API Gateway (Port 7000)
├── Order Service (Port 7001)
│   ├── CQRS Commands/Queries
│   ├── SQL Server Database
│   └── Kafka Event Publishing
└── Product Service (Port 7002)
    ├── CQRS Queries
    └── SQL Server Database
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker Desktop

### 1. Start Infrastructure
```bash
docker-compose up -d
```

### 2. Update Connection Strings
Update the SQL Server connection strings in appsettings.json files:
```json
"DefaultConnection": "Server=localhost,1433;Database=OrderServiceDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
```

### 3. Run Services
```bash
# Terminal 1 - API Gateway
cd src/ApiGateway
dotnet run --urls="https://localhost:7000"

# Terminal 2 - Order Service
cd src/Services/OrderService
dotnet run --urls="https://localhost:7001"

# Terminal 3 - Product Service
cd src/Services/ProductService
dotnet run --urls="https://localhost:7002"
```

### 4. Test the API
- API Gateway: https://localhost:7000/swagger
- Order Service: https://localhost:7001/swagger
- Product Service: https://localhost:7002/swagger

## API Endpoints

### Through API Gateway (Port 7000)
- `POST /api/orders` - Create order
- `GET /api/orders/{id}` - Get order
- `GET /api/products` - Get products

### Example: Create Order
```json
POST https://localhost:7000/api/orders
{
  "customerName": "John Doe",
  "items": [
    {
      "productId": "guid-here",
      "productName": "Product 1",
      "quantity": 2,
      "price": 29.99
    }
  ]
}
```

## Key Features

- **CQRS Pattern**: Separate read/write models using MediatR
- **Event-Driven**: Kafka integration for inter-service communication
- **API Gateway**: Single entry point with Ocelot routing
- **Microservices**: Independent, deployable services
- **SQL Server**: Persistent data storage per service