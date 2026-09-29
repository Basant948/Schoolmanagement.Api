# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first
COPY ["Schoolmanagement.Api/Schoolmanagement.Api.csproj", "Schoolmanagement.Api/"]
COPY ["SchoolManagement.Application/SchoolManagement.Application.csproj", "SchoolManagement.Application/"]
COPY ["SchoolManagement.Domain/SchoolManagement.Domain.csproj", "SchoolManagement.Domain/"]
COPY ["SchoolManagement.Infrastructure/SchoolManagement.Infrastructure.csproj", "SchoolManagement.Infrastructure/"]

RUN dotnet restore "Schoolmanagement.Api/Schoolmanagement.Api.csproj"

COPY . .

WORKDIR "/src/Schoolmanagement.Api"
RUN dotnet publish "Schoolmanagement.Api.csproj" \
    -c Release \
    -o /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Schoolmanagement.Api.dll"]