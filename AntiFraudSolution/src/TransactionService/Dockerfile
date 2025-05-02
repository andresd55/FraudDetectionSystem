# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY src/TransactionService/*.csproj ./src/TransactionService/
RUN dotnet restore ./src/TransactionService/TransactionService.csproj

COPY . .
WORKDIR /app/src/TransactionService
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/TransactionService/out ./
EXPOSE 80

ENTRYPOINT ["dotnet", "TransactionService.dll"]
