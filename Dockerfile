FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

COPY *.sln .
COPY *.csproj .
RUN dotnet restore raBudget-Api.sln

COPY . .
RUN dotnet publish raBudget-Api.sln -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]