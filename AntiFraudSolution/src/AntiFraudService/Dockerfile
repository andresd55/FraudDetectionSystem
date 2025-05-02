# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.sln .
COPY src/AntiFraudService/*.csproj ./src/AntiFraudService/
RUN dotnet restore ./src/AntiFraudService/AntiFraudService.csproj

# Copiar todo el proyecto y compilar
COPY . .
WORKDIR /app/src/AntiFraudService
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/AntiFraudService/out ./

# Puerto que usará el servicio
EXPOSE 80

# Ejecutar el servicio
ENTRYPOINT ["dotnet", "AntiFraudService.dll"]
