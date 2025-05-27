# Posts API - SECLOUS Coding Assessment

A simple REST API for managing blog posts, built with .NET 8 and containerized with Docker.

## 📋 Overview

This project implements a RESTful API that manages blog posts. It includes:

- **.NET 8 Web API** with full CRUD operations
- **In-memory data storage** using ConcurrentDictionary
- **Docker containerization** with multi-stage builds
- **Test client** that validates all endpoints
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

### Test Scenarios

1. Get initial seed data
2. Create a new post
3. Retrieve post by ID
4. Update the post
5. Verify updates
6. Delete the post
7. Confirm deletion (404)
8. Test non-existent post scenarios

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