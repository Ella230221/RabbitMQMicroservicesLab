# RabbitMQ Microservices Lab

This project is a simple e-commerce microservices system.

It includes:

- Blazor Frontend
- API Gateway
- Customer Service
- Product Service
- Order Service
- Payment Service
- RabbitMQ

## Main Features

- View customers
- View products
- Create customers
- Create products
- Create orders
- Process payments
- Update product stock after an order
- Gateway routing and aggregation

## Technologies

- ASP.NET Core
- Blazor WebAssembly
- Ocelot API Gateway
- RabbitMQ
- SQLite
- Docker Compose

## Project Structure

- `Frontend` : Blazor UI
- `ApiGateway` : Gateway for routing and aggregation
- `CustomerService` : customer data
- `ProductService` : product data and stock
- `OrderService` : order creation
- `PaymentService` : payment records
- `docker-compose.yml` : run all services together

## How to Run

Make sure Docker Desktop is running.

Run:

```bash
docker compose up --build
```

## Ports

- `5000` : API Gateway
- `5001` : Customer Service
- `5002` : Order Service
- `5003` : Product Service
- `5004` : Payment Service
- `5262` : Frontend
- `15672` : RabbitMQ Management

## Example URLs

- Gateway Swagger: `http://localhost:5000/swagger`
- Frontend: `http://localhost:5262`
- Customers: `http://localhost:5001/api/customers`
- Products: `http://localhost:5003/api/products`

## Workflow Example

1. User creates an order from the frontend
2. Frontend sends request to API Gateway
3. Gateway sends request to Order Service
4. Order Service checks customer and product
5. Order Service saves the order
6. Order Service sends an event to RabbitMQ
7. Product Service updates stock
8. Payment Service creates a payment record

## Author

- Ruxue Yang
