# Posts API - SECLOUS Coding Assessment

A simple REST API for managing blog posts, built with .NET 8 and containerized with Docker.

## 📋 Overview

This project implements a RESTful API that manages blog posts according to the provided OpenAPI specification. It includes:

- **.NET 8 Web API** with full CRUD operations
- **In-memory data storage** using ConcurrentDictionary
- **Docker containerization** with multi-stage builds
- **Comprehensive test client** that validates all endpoints
- **Docker Compose** orchestration for easy deployment

## 🏗️ Architecture

```
PostsAPI/
├── src/PostsAPI/          # Main API application
│   ├── Controllers/       # REST controllers
│   ├── Models/           # Data models (Post, PostInput)
│   ├── Services/         # Business logic layer
│   ├── Data/            # Repository pattern implementation
│   └── Program.cs       # Application startup
├── client/TestClient/    # Test client application
└── docker-compose.yml   # Container orchestration
```

## 🚀 Quick Start

### Prerequisites
- Docker Desktop
- .NET 8 SDK (for local development only)

### Running with Docker Compose (Recommended)

1. **Clone/Extract the project**
2. **Navigate to the project directory**
   ```bash
   cd PostsAPI
   ```

3. **Start the services**
   ```bash
   docker-compose up --build
   ```

4. **Watch the test results**
   The test client will automatically run and display results in the console.

5. **Clean up**
   ```bash
   docker-compose down
   ```

## 🔍 API Endpoints

The API implements the following endpoints as per the OpenAPI specification:

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| GET | `/posts` | List all posts | 200 + Post array |
| POST | `/posts` | Create new post | 201 + Created post |
| GET | `/posts/{id}` | Get post by ID | 200 + Post / 404 |
| PUT | `/posts/{id}` | Update post | 200 + Updated post / 404 |
| DELETE | `/posts/{id}` | Delete post | 204 / 404 |
| GET | `/health` | Health check | 200 + Status |

### Example API Usage

**Create a post:**
```bash
curl -X POST http://localhost:8080/posts \
  -H "Content-Type: application/json" \
  -d '{
    "author": "John Doe",
    "date": "2025-05-16T12:00:00Z",
    "content": "My first blog post"
  }'
```

**Get all posts:**
```bash
curl http://localhost:8080/posts
```

## 🧪 Test Client

The test client performs comprehensive validation:

- ✅ **CRUD Operations** - Tests all endpoints
- ✅ **Error Handling** - Validates 404 responses
- ✅ **Data Integrity** - Verifies request/response data
- ✅ **Status Codes** - Confirms proper HTTP responses
- ✅ **JSON Format** - Validates response structure

### Test Scenarios

1. Get initial seed data
2. Create a new post
3. Retrieve post by ID
4. Update the post
5. Verify updates
6. Delete the post
7. Confirm deletion (404)
8. Test non-existent post scenarios

## 🐳 Docker Details

### API Service
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:8.0`
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:8.0`
- **Port**: 8080
- **Health Check**: `/health` endpoint

### Test Client Service
- **Base Image**: `mcr.microsoft.com/dotnet/runtime:8.0`
- **Dependency**: Waits for API health check
- **Environment**: `API_BASE_URL=http://posts-api:8080`

## 💻 Local Development

### Running API Locally
```bash
cd src/PostsAPI
dotnet run
```
API will be available at `http://localhost:5000`

### Running Test Client Locally
```bash
cd client/TestClient
dotnet run
```
Set `API_BASE_URL` environment variable if needed.

### Swagger UI
When running locally in development mode, Swagger UI is available at:
`http://localhost:5000/swagger`

## 📊 Features

### API Features
- **Dependency Injection** - Clean architecture
- **Repository Pattern** - Abstracted data access
- **Structured Logging** - Request/response tracking
- **CORS Support** - Cross-origin requests
- **Health Checks** - Container monitoring
- **Swagger Documentation** - Interactive API docs

### Data Management
- **In-Memory Storage** - Thread-safe ConcurrentDictionary
- **GUID-based IDs** - Unique post identifiers
- **Seed Data** - Pre-populated sample posts
- **Data Validation** - Model validation attributes

### Error Handling
- **Consistent Responses** - Standardized error format
- **Proper Status Codes** - HTTP specification compliance
- **Exception Handling** - Graceful error recovery
- **Validation** - Input data validation

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | App environment | `Production` |
| `ASPNETCORE_URLS` | Binding URLs | `http://+:8080` |
| `API_BASE_URL` | Client target URL | `http://localhost:8080` |

### Logging Configuration
Configured in `appsettings.json`:
- **Information** level for application logs
- **Warning** level for ASP.NET Core framework
- Console and Debug providers enabled

## 🏆 Design Decisions

### In-Memory Storage
- **Reasoning**: Simplicity and no external dependencies
- **Implementation**: Thread-safe ConcurrentDictionary
- **Trade-offs**: Data doesn't persist between restarts

### Repository Pattern
- **Reasoning**: Separation of concerns and testability
- **Benefits**: Easy to swap storage implementations
- **Interface**: `IPostRepository` for abstraction

### Docker Multi-Stage Builds
- **Benefits**: Smaller runtime images
- **Security**: No SDK tools in production image
- **Efficiency**: Optimized layer caching

### Service Architecture
- **Controllers**: Handle HTTP concerns
- **Services**: Business logic and validation
- **Repositories**: Data access abstraction
- **Models**: Data transfer objects

## 🛠️ Troubleshooting

### Common Issues

**Port already in use:**
```bash
# Check what's using port 8080
netstat -ano | findstr :8080  # Windows
lsof -i :8080                 # Mac/Linux

# Use different port in docker-compose.yml
ports:
  - "8081:8080"
```

**API not ready:**
- Wait for health check to pass
- Check Docker logs: `docker-compose logs posts-api`
- Increase health check timeout if needed

**Test client fails:**
- Verify API is healthy: `curl http://localhost:8080/health`
- Check network connectivity between containers
- Review test client logs: `docker-compose logs test-client`

### Debug Commands

```bash
# View all logs
docker-compose logs

# View specific service logs
docker-compose logs posts-api
docker-compose logs test-client

# Check container status
docker-compose ps

# Restart services
docker-compose restart

# Rebuild and restart
docker-compose up --build --force-recreate
```

## 📝 Notes

### Submission Compliance
- ✅ Only Docker Hub official images used
- ✅ No custom Docker registries
- ✅ Source files only (no binaries)
- ✅ Complete docker-compose setup
- ✅ All OpenAPI endpoints implemented
- ✅ Comprehensive test client

### OpenAPI Specification Compliance
- ✅ All endpoints match specification exactly
- ✅ Request/response models match schema
- ✅ HTTP status codes as specified
- ✅ JSON examples match format
- ✅ Content-Type headers correct

## 🎯 Success Criteria Met

- ✅ **Step 1**: REST server with all OpenAPI endpoints
- ✅ **Step 2**: Test script validating all requests
- ✅ **Step 3**: Docker compose with API + client services
- ✅ **Final Requirements**: Source files only, Docker Hub images, comprehensive documentation

---

**Author**: Ammar Fouda  
**Assessment**: SECLOUS Coding Case 2025-05