# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy toàn bộ project
COPY . .

# Restore + publish
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy file đã build
COPY --from=build /out .

# Chạy app
ENTRYPOINT ["dotnet", "ConnectDB.dll"]