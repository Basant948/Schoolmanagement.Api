# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first (note: folder is "Schoolmanagement.Api" with lowercase m)
COPY ["Schoolmanagement.Api/SchoolManagement.Api.csproj", "SchoolManagement.Api/"]
COPY ["SchoolManagement.Application/SchoolManagement.Application.csproj", "SchoolManagement.Application/"]
COPY ["SchoolManagement.Domain/SchoolManagement.Domain.csproj", "SchoolManagement.Domain/"]
COPY ["SchoolManagement.Infrastructure/SchoolManagement.Infrastructure.csproj", "SchoolManagement.Infrastructure/"]

RUN dotnet restore "SchoolManagement.Api/SchoolManagement.Api.csproj"

COPY . .

WORKDIR "/src/SchoolManagement.Api"
RUN dotnet publish "SchoolManagement.Api.csproj" \
    -c Release \
    -o /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SchoolManagement.Api.dll"]