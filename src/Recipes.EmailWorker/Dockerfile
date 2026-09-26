FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Recipes.Messaging/Recipes.Messaging.csproj src/Recipes.Messaging/
COPY src/Recipes.EmailWorker/Recipes.EmailWorker.csproj src/Recipes.EmailWorker/
RUN dotnet restore src/Recipes.EmailWorker/Recipes.EmailWorker.csproj

COPY . .
RUN dotnet publish src/Recipes.EmailWorker/Recipes.EmailWorker.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Recipes.EmailWorker.dll"]
