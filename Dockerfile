# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/BankApi.Api/BankApi.Api.csproj", "src/BankApi.Api/"]
COPY ["src/BankApi.Application/BankApi.Application.csproj", "src/BankApi.Application/"]
COPY ["src/BankApi.Domain/BankApi.Domain.csproj", "src/BankApi.Domain/"]
COPY ["src/BankApi.Infrastructure/BankApi.Infrastructure.csproj", "src/BankApi.Infrastructure/"]
RUN dotnet restore "src/BankApi.Api/BankApi.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/BankApi.Api"
RUN dotnet build "BankApi.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "BankApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published files
COPY --from=publish /app/publish .

# Health check configuration
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "BankApi.Api.dll"]
