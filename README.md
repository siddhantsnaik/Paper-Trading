# Paper-Trading

This repository contains the microservices middleware built using .NET Web API for the OnPaper paper trading application. It includes two primary microservices:

1. **User Management and Authentication Service**
2. **Trade Management Service**

## Table of Contents

- [Overview](#overview)
- [Microservices](#microservices)
  - [User Management and Authentication Service](#user-management-and-authentication-service)
  - [Trade Management Service](#trade-management-service)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Services](#running-the-services)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## Overview

The OnPaper application simulates a trading environment where users can conduct paper trades without any real financial risk on Indian Stock Exchanges like BSE  and NSE. This repository hosts the back-end microservices which are responsible for handling user authentication, user management, and trade operations.

## Microservices

### User Management and Authentication Service

This service is responsible for:

- User registration and authentication.
- Managing user profiles.
- Handling user sessions and authorization.

### Trade Management Service

This service handles:

- Recording trades conducted by users.
- Managing trade data and history.
- Providing trade-related insights and analytics.

## Technologies Used

- **C#**: The primary programming language for microservice development.
- **.NET Web API**: Framework used to build the microservices.
- **Docker**: For containerizing the microservices.
- **Azure**: For deploying the microservice container in a Container App.
-  **GitHub Actions**: Establishing CI/CD pipeline for Continuous deployment of the microservices on Azure Container Apps

## Getting Started

### Prerequisites

Ensure that you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Visual Studio IDE](https://visualstudio.microsoft.com/downloads/)
- [Docker (Optional)](https://www.docker.com/products/docker-desktop)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/siddhantsnaik/Paper-Trading.git
   cd Paper-Trading
   ```

### Running the Services

1. Navigate to the directory of the microservice you want to run:
   ```bash
   cd UserManagementService
   # or
   cd TradeManagementService
   ```

2. Build and run the microservice using the .NET CLI:
   ```bash
   dotnet build
   dotnet run
   ```

3. Alternatively, you can use Docker to run the services:
   ```bash
   docker build -t user-management-service .
   docker run -p 5000:80 user-management-service
   ```

## API Endpoints

### User Management and Authentication Service

- `POST /api/register`: Register a new user.
- `POST /api/login`: Authenticate a user and start a session.
- `GET /api/users/{id}`: Get user profile details.
- `PUT /api/users/{id}`: Update user profile.

### Trade Management Service

- `POST /api/trades`: Record a new trade.
- `GET /api/trades/{userId}`: Get trade history for a user.
- `GET /api/trades/{tradeId}`: Get details of a specific trade.
- `DELETE /api/trades/{tradeId}`: Delete a trade.

## Contributing

We welcome contributions! Please read our [contributing guidelines](CONTRIBUTING.md) to get started.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
