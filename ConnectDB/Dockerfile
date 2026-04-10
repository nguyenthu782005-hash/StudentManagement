# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy tệp project và restore trước để tối ưu cache
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code còn lại và build
COPY . ./
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "ConnectDB.dll"]