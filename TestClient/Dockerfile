# TestClient/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY TestClient/*.csproj ./TestClient/
RUN dotnet restore ./TestClient/*.csproj

# Copy the rest of the source
COPY TestClient/. ./TestClient/
WORKDIR /src/TestClient
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Accept build-time variable and set as environment variable
ARG API_BASE_URL
ENV API_BASE_URL=${API_BASE_URL}

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TestClient.dll"]