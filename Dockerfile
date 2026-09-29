# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files first
COPY ["SchoolManagement.Api/SchoolManagement.Api.csproj", "SchoolManagement.Api/"]
COPY ["SchoolManagement.Application/SchoolManagement.Application.csproj", "SchoolManagement.Application/"]
COPY ["SchoolManagement.Domain/SchoolManagement.Domain.csproj", "SchoolManagement.Domain/"]
COPY ["SchoolManagement.Infrastructure/SchoolManagement.Infrastructure.csproj", "SchoolManagement.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "SchoolManagement.Api/SchoolManagement.Api.csproj"

# Copy everything else
COPY . .

# Build/publish Web project
WORKDIR "/src/SchoolManagement.Api"

RUN dotnet publish "SchoolManagement.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# =========================
# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Render provides the PORT environment variable
ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "SchoolManagement.Api.dll"]