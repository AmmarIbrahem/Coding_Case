# PostsAPI/Dockerfile

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY PostsAPI/PostsAPI.csproj ./PostsAPI/
RUN dotnet restore ./PostsAPI/PostsAPI.csproj

# Copy the rest of the source
COPY PostsAPI/. ./PostsAPI/
WORKDIR /src/PostsAPI
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostsAPI.dll"]