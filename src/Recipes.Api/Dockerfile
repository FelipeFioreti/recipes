FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Recipes.Domain/Recipes.Domain.csproj src/Recipes.Domain/
COPY src/Recipes.Application/Recipes.Application.csproj src/Recipes.Application/
COPY src/Recipes.Infrastructure/Recipes.Infrastructure.csproj src/Recipes.Infrastructure/
COPY src/Recipes.Api/Recipes.Api.csproj src/Recipes.Api/
RUN dotnet restore src/Recipes.Api/Recipes.Api.csproj

COPY . .
RUN dotnet publish src/Recipes.Api/Recipes.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Recipes.Api.dll"]
